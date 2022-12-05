public static class TransactionModule
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        app.MapGet("/transaction/definition", getTransactionDefinition)
        .WithOpenApi()
        .WithSummary("Starts a new transaction.")
        .WithDescription("**For Gateway Integration** Use to start new transaction (consent request or simulate).")
        .WithTags("Definition")
        .Produces<PostTransactionRequestResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        app.MapPost("/transaction/definition", (PostTransactionRequest body) => { })
        .WithOpenApi()
        .WithSummary("Starts a new transaction.")
        .WithDescription("**For Gateway Integration** Use to start new transaction (consent request or simulate).")
        .WithTags("Definition")
        .Produces<PostTransactionRequestResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);


        app.MapPost("/transaction/instance/{transaction-id}/request", ([FromRoute(Name = "transaction-id")] string requestUrl, PostTransactionRequest body) => { })
        .WithOpenApi()
        .WithSummary("Starts a new transaction.")
        .WithDescription("**For Gateway Integration** Use to start new transaction (consent request or simulate).")
        .WithTags("Gateway Integration")
        .Produces<PostTransactionRequestResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        app.MapPost("/transaction/instance/{transaction-id}/order", ([FromRoute(Name = "transaction-id")] string requestUrl, PostTransactionOrder body) => { })
        .WithOpenApi()
        .WithSummary("Advance pending transaction.")
        .WithDescription("**For Gateway Integration** Use to complete requested transaction.")
        .WithTags("Gateway Integration")
        .Produces<PostTransactionOrderResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        app.MapPost("/transaction/instance/{transaction-id}/command", ([FromRoute(Name = "transaction-id")] string requestUrl, PostCommand body) => { })
        .WithOpenApi()
        .WithSummary("Send command messaage to transaction flow.")
        .WithDescription("**For clients and Workflow** Commands are transfered as message to transaction workflow. 3FA, Fraud or external system triggers are implemented as commands.")
        .WithTags("Client")
        .Produces<GetTransactionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        app.MapGet("/transaction/instance/{transaction-id}", GetTransaction)
        .WithOpenApi()
        .WithSummary("Returns transaction detail.")
        .WithTags("Client")
        .Produces<GetTransactionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        app.MapGet("/transaction/instance/{transaction-id}/status", GetTransactionStatus)
        .WithOpenApi()
        .WithSummary("Returns transaction detail.")
        .WithTags("Client")
        .Produces<GetTransactionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);
    }


    static IResult getTransactionDefinition(
        [FromQuery(Name = "url")] string requestOrOrderUrl,
        [FromServices] TransactionDBContext context,
        [FromServices] ILogger<IResult> logger
    )
    {
        logger.LogError($"getTransactionDefinition is queried with {requestOrOrderUrl}");
        
        var definition = context!.Definitions!
           .Include(d => d.Checkers)
           .Where(t => t.RequestUrlTemplate == requestOrOrderUrl || t.OrderUrlTemplate == requestOrOrderUrl)
           .FirstOrDefault();

        if (definition == null)
            return Results.NotFound();

        return Results.Ok(
            new GetDefinitionResponse(definition.Id, definition.RequestUrlTemplate, definition.OrderUrlTemplate, definition.Client, definition.Workflow, definition.TTL, definition.SignalRHub,
            definition.Checkers!.Select(c => new GetDefinitionCheckerResponse(c.RequestDataPath, c.OrderDataPath, c.Type)).ToArray())
        );
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