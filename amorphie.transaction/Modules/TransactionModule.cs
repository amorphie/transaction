using System.Diagnostics.CodeAnalysis;

public static class TransactionModule
{
    const string STATE_STORE = "transaction-cache";
    static WebApplication _app = default!;

    public static void MapTransactionEndpoints(this WebApplication app)
    {
        _app = app;


        _app.MapGet("/transaction/definition", getTransactionDefinition)
        .WithOpenApi()
        .WithSummary("Gets registered transation definitions.")
        .WithDescription("Returns existing transaction definitions with metadata.Query parameter url is can contain request or order url of transaction.")
        .WithTags("Definition")
        .Produces<GetDefinitionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        _app.MapPost("/transaction/definition", postTransactionDefinition)
        .WithOpenApi()
        .WithSummary("Save transaction definition")
        .WithDescription("It is update or creates new transaction defitinion due to templare urls and client parameters. Base Rule request and order urls can use just once per client")
        .WithTags("Definition")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        _app.MapDelete("/transaction/definition/{definition-id}", deleteTransactionDefinition)
       .WithOpenApi()
       .WithSummary("Deletes transaction definition")
       .WithDescription("Deletes transaction definition. But not deletes SignalR Hub, workflow definitions and worklfow instances.")
       .WithTags("Definition")
       .Produces(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status201Created)
       .Produces(StatusCodes.Status409Conflict);

        _app.MapPost("/transaction/definition/{definition-id}/validator", ([FromRoute(Name = "definition-id")] string definitionId) => { })
        .WithOpenApi()
        .WithSummary("Add or updates transaction validator")
        .WithDescription("Add or updates transaction validator.")
        .WithTags("Definition")
        .Produces<GetDefinitionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        _app.MapDelete("/transaction/definition/{definition-id}/validator/{validator-id}", ([FromRoute(Name = "definition-id")] string definitionId, [FromRoute(Name = "validator-id")] string requestDataPath) => { })
       .WithOpenApi()
       .WithSummary("Add or updates transaction validator")
       .WithDescription("Add or updates transaction validator.")
       .WithTags("Definition")
       .Produces<GetDefinitionResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        _app.MapPost("/transaction/instance/{transaction-id}/request", ([FromRoute(Name = "transaction-id")] string requestUrl, PostTransactionRequest body) => { })
        .WithOpenApi()
        .WithSummary("Starts a new transaction.")
        .WithDescription("**For Gateway Integration** Use to start new transaction (consent request or simulate).")
        .WithTags("Gateway Integration")
        .Produces<PostTransactionRequestResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        _app.MapPost("/transaction/instance/{transaction-id}/order", ([FromRoute(Name = "transaction-id")] string requestUrl, PostTransactionOrder body) => { })
        .WithOpenApi()
        .WithSummary("Advance pending transaction.")
        .WithDescription("**For Gateway Integration** Use to complete requested transaction.")
        .WithTags("Gateway Integration")
        .Produces<PostTransactionOrderResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        _app.MapPost("/transaction/instance/{transaction-id}/command", ([FromRoute(Name = "transaction-id")] string requestUrl, PostCommand body) => { })
        .WithOpenApi()
        .WithSummary("Send command messaage to transaction flow.")
        .WithDescription("**For clients and Workflow** Commands are transfered as message to transaction workflow. 3FA, Fraud or external system triggers are implemented as commands.")
        .WithTags("Client")
        .Produces<GetTransactionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        _app.MapGet("/transaction/instance/{transaction-id}", GetTransaction)
        .WithOpenApi()
        .WithSummary("Returns transaction detail.")
        .WithTags("Client")
        .Produces<GetTransactionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        _app.MapGet("/transaction/instance/{transaction-id}/status", GetTransactionStatus)
        .WithOpenApi()
        .WithSummary("Returns transaction detail.")
        .WithTags("Client")
        .Produces<GetTransactionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);
    }


    static async Task<IResult> getTransactionDefinition(
        [FromQuery(Name = "url")] string requestOrOrderUrl,
        HttpRequest request,
        HttpContext httpContext,
        [FromServices] DaprClient client,
        [FromServices] TransactionDBContext context
    )
    {
        _app.Logger.LogInformation($"getTransactionDefinition is queried with {requestOrOrderUrl}");

        var cachedResponse = await client.GetStateAsync<GetDefinitionResponse>(STATE_STORE, requestOrOrderUrl);
        if (cachedResponse is not null)
        {
            httpContext.Response.Headers.Add("X-Cache", "Hit");
            return Results.Ok(cachedResponse);
        }

        var definition = context!.Definitions!
           .Include(d => d.Validators)
           .Where(t => t.RequestUrlTemplate == requestOrOrderUrl || t.OrderUrlTemplate == requestOrOrderUrl)
           .FirstOrDefault();

        if (definition == null)
            return Results.NotFound();

        httpContext.Response.Headers.Add("X-Cache", "Miss");

        var response = new GetDefinitionResponse(definition.Id, definition.RequestUrlTemplate, definition.OrderUrlTemplate, definition.Client, definition.Workflow, definition.TTL, definition.SignalRHub,
            definition.Validators!.Select(c => new GetDefinitionValidatorResponse(c.Id, c.RequestDataPath, c.OrderDataPath, c.Type)).ToArray());


        var metadata = new Dictionary<string, string> { { "ttlInSeconds", "300" } };
        await client.SaveStateAsync(STATE_STORE, response.orderUrlTemplate, response, metadata: metadata);
        await client.SaveStateAsync(STATE_STORE, response.requestUrlTemplate, response, metadata: metadata);

        return Results.Ok(response);
    }


    static async Task<IResult> postTransactionDefinition(
        [FromBody] PostDefinitionRequest data,
        [FromServices] DaprClient client,
        [FromServices] TransactionDBContext context
        )
    {
        // Check any url and client configuration is exists ?
        var definitions = context!.Definitions!
          .Where(t => (t.RequestUrlTemplate == data.requestUrlTemplate || t.OrderUrlTemplate == data.orderUrlTemplate) && t.Client == data.client)
          .ToArray();

        _app.Logger.LogInformation($"Count of definition {definitions.Count()}");


        if (definitions.Count() == 0)
        {
            var newRecord = new TransactionDefinition { Id = Guid.NewGuid(), RequestUrlTemplate = data.requestUrlTemplate, OrderUrlTemplate = data.orderUrlTemplate, Client = data.client, TTL = data.ttl, SignalRHub = data.signalRHub, Workflow = data.workflow };
            context!.Definitions!.Add(newRecord);
            context.SaveChanges();
            return Results.Created($"/transaction/definition/{data.requestUrlTemplate}", newRecord);
        }

        var recordToUpdate = definitions.Where(t => t.RequestUrlTemplate == data.requestUrlTemplate && t.OrderUrlTemplate == data.orderUrlTemplate && t.Client == data.client).FirstOrDefault();

        if (recordToUpdate != null)
        {
            var hasChanges = false;
            // Apply update to only changed fields.
            if (data.workflow != null && data.workflow != recordToUpdate.Workflow) { recordToUpdate.Workflow = data.workflow; hasChanges = true; }
            if (data.ttl != recordToUpdate.TTL) { recordToUpdate.TTL = data.ttl; hasChanges = true; }
            if (data.signalRHub != null && data.signalRHub != recordToUpdate.SignalRHub) { recordToUpdate.SignalRHub = data.signalRHub; hasChanges = true; }


            if (hasChanges)
            {
                await client.DeleteStateAsync(STATE_STORE, data.orderUrlTemplate);
                await client.DeleteStateAsync(STATE_STORE, data.requestUrlTemplate);

                context!.SaveChanges();
                return Results.Ok();
            }
            else
            {
                return Results.Problem("Not Modified.", null, 304);
            }
        }
        return Results.Conflict("Request or Order template is already used for another record.");
    }

    static IResult deleteTransactionDefinition(
        [FromRoute(Name = "definition-id")] Guid id,
        [FromServices] TransactionDBContext context)
    {

        var recordToDelete = context?.Definitions?.FirstOrDefault(t => t.Id == id);

        if (recordToDelete == null)
        {
            return Results.NotFound();
        }
        else
        {
            context!.Remove(recordToDelete);
            context.SaveChanges();
            return Results.Ok();
        }
    }



    public static IResult GetTransaction(
        [FromRoute(Name = "transaction-id")] Guid transactionId
    )
    {
        return Results.NotFound();
    }

    public static IResult GetTransactionStatus(
        [FromRoute(Name = "transaction-id")] Guid transactionId
    )
    {
        return Results.NotFound();
    }
}