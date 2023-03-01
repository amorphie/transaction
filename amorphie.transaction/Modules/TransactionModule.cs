using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static Transaction;

public static class TransactionModule
{
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

        _app.MapPost("/transaction/instance/{transaction-id}/request", requestTransaction)
        .WithOpenApi()
        .WithSummary("Starts a new transaction.")
        .WithDescription("**For Gateway Integration** Used to start new transaction (consent request or simulate).")
        .WithTags("Gateway Integration")
        .Produces<PostTransactionRequestResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        _app.MapPost("/transaction/instance/{transaction-id}/order", orderTransaction)
        .WithOpenApi()
        .WithSummary("Advance pending transaction.")
        .WithDescription("**For Gateway Integration** Used to complete requested transaction.")
        .WithTags("Gateway Integration")
        .Produces<PostTransactionOrderResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        _app.MapPost("/transaction/instance/{transaction-id}/command", commandTransaction)
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
        [FromQuery(Name="Url")]string requestOrOrderUrl,
        HttpRequest request,
        HttpContext httpContext,
        [FromServices] DaprClient client,
        [FromServices] TransactionDBContext context,
        IConfiguration configuration
    )
    {
        var stateStoreName = configuration["DAPR_STATE_STORE_NAME"];

        _app.Logger.LogInformation($"getTransactionDefinition is queried with {requestOrOrderUrl}");

        var cachedResponse = await client.GetStateAsync<GetDefinitionResponse>(stateStoreName, requestOrOrderUrl);
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

        var response = new GetDefinitionResponse(definition.Id, definition.RequestUrlMethod, definition.RequestUrlTemplate, definition.OrderUrlMethod, definition.OrderUrlTemplate, definition.Client, definition.Workflow, definition.TTL,
            definition.Validators!.Select(c => new GetDefinitionValidatorResponse(c.Id, c.RequestDataPath, c.OrderDataPath, c.Type)).ToArray());


        var metadata = new Dictionary<string, string> { { "ttlInSeconds", "300" } };
        await client.SaveStateAsync(stateStoreName, response.orderUrlTemplate, response, metadata: metadata);
        await client.SaveStateAsync(stateStoreName, response.requestUrlTemplate, response, metadata: metadata);

        return Results.Ok(response);
    }


    static async Task<IResult> postTransactionDefinition(
        [FromBody] PostDefinitionRequest data,
        [FromServices] DaprClient client,
        [FromServices] TransactionDBContext context,
        IConfiguration configuration
        )
    {
        var stateStoreName = configuration["DAPR_STATE_STORE_NAME"];

        // Check any url and client configuration is exists ?
        var definitions = context!.Definitions!
          .Where(t => (t.RequestUrlTemplate == data.requestUrlTemplate || t.OrderUrlTemplate == data.orderUrlTemplate) && t.Client == data.client)
          .ToArray();

        _app.Logger.LogInformation($"Count of definition {definitions.Count()}");


        if (definitions.Count() == 0)
        {
            var newRecord = new TransactionDefinition { Id = Guid.NewGuid(), RequestUrlTemplate = data.requestUrlTemplate, OrderUrlTemplate = data.orderUrlTemplate, Client = data.client, TTL = data.ttl, Workflow = data.workflow };
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


            if (hasChanges)
            {
                await client.DeleteStateAsync(stateStoreName, data.orderUrlTemplate);
                await client.DeleteStateAsync(stateStoreName, data.requestUrlTemplate);

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

    static async Task<IResult> requestTransaction(
        [FromRoute(Name = "transaction-id")] Guid transactionId,
        [FromBody] PostTransactionRequest data,
        HttpRequest request,
        HttpContext httpContext,
        [FromServices] DaprClient client,
        [FromServices] TransactionDBContext dbContext,
        IConfiguration configuration
    )
    {
        var definition = dbContext!.Definitions!
          .Where(t => (t.RequestUrlMethod == data.method && t.RequestUrlTemplate == data.url && t.Client == data.client))
          .FirstOrDefault();

        if (definition == null)
        {
            return Results.NotFound("Transaction definition is not found. Please check url is exists in definitions.");
        }

        var transaction = new Transaction(){
            CreatedAt = DateTime.UtcNow,
            Id = transactionId,
            Status = "Initialize",
            StatusReason = "Transaction is About To Request To Upstream Url",
            TransactionDefinition = definition
        };

        await dbContext.Transactions.AddAsync(transaction);

        HttpResponseMessage upHttpResponse;
        HttpClient httpClient = new();

        httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue(data.headers.FirstOrDefault(h => h.Key == "Content-Type").Value));

        foreach (var h in data.headers)
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, h.Value);
        }

        var generatedUpstreamUrl = data.upStreamUrl;

        if(data.urlParams.Count > 0)
        {
            foreach(var urlParam in data.urlParams)
            {
                generatedUpstreamUrl += $"/{urlParam}";
            }
        }

        if (data.method == TransactionDefinition.MethodType.GET)
        {
            if(data.queryParams.Count > 0)
            {
                var firstIterate = true;
                generatedUpstreamUrl += "?";
                foreach(var queryParam in data.queryParams)
                {
                    if(firstIterate)
                    {
                        generatedUpstreamUrl += $"{queryParam.Key}={queryParam.Value}";
                        firstIterate = false;
                    }
                    else
                    {
                        generatedUpstreamUrl += $"&{queryParam.Key}={queryParam.Value}";
                    }
                }
            }
            upHttpResponse = await httpClient.GetAsync(generatedUpstreamUrl);
            
        }
        else
        {
            JsonContent bodyContent = JsonContent.Create(data.body);
            upHttpResponse = await httpClient.PostAsJsonAsync(generatedUpstreamUrl, data.body);
        }

        try
        {
            upHttpResponse.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            return Results.BadRequest(JsonSerializer.Serialize(new{
                StatusCode = (int)ex.StatusCode,
                Message = await upHttpResponse.Content.ReadAsStringAsync()
            }));
        }

        var upResponseData = await upHttpResponse.Content.ReadAsStringAsync();
        transaction.RequestUpstreamResponse = upResponseData;

        _app.Logger.LogInformation($"requestTransaction is called with {transactionId}");

        string requestBody = string.Empty;
        if(data.method == TransactionDefinition.MethodType.GET)
        {
            Dictionary<string,string> jsonValues = new();
            
            foreach(var queryParam in data.queryParams)
            {
                jsonValues.Add(queryParam.Key,queryParam.Value);
            }
            requestBody = JsonSerializer.Serialize(jsonValues);
        }
        else
        {
            requestBody = data.body.ToJsonString();
        }

        transaction.RequestUpstreamBody = requestBody;

        var variables = new TransactionInstanceRequestData(transactionId, data.scope, data.client, data.reference, data.user, requestBody);

        dynamic instanceData = new ExpandoObject();
        instanceData.bpmnProcessId = definition.Workflow;
        instanceData.variables = variables;

        var workflowInstanceResult = await client.InvokeBindingAsync<dynamic, dynamic>(configuration["DAPR_ZEEBE_COMMAND_NAME"], "create-instance", instanceData);

        var tokenRequestData = new PostCreateTransactionHubTokenRequest(transactionId, definition.Id, data.scope, data.client, data.user, data.reference, definition.TTL);
        _app.Logger.LogInformation(JsonSerializer.Serialize(tokenRequestData));
        var token = await client.InvokeMethodAsync<PostCreateTransactionHubTokenRequest, string>(HttpMethod.Post, "amorphie-transaction-hub.test-amorphie-transaction-hub", "security/create-token", tokenRequestData);

        transaction.SignalRHubToken = token;

        var returnValue = new PostTransactionRequestResponse(
        upResponseData,
        new PostTransactionRequestTransactionResponse(transactionId, workflowInstanceResult,configuration["TransactionHubUri"], token ));

        return Results.Ok(returnValue);
       
    }

    static async Task<IResult> orderTransaction(
        [FromRoute(Name = "transaction-id")] Guid transactionId,
        [FromBody] PostTransactionOrder data,
        HttpRequest request,
        HttpContext httpContext,
        [FromServices] DaprClient client,
        [FromServices] TransactionDBContext dbContext
    )
    {
        var definition = await dbContext!.Definitions!
          .Where(t => (t.OrderUrlMethod == data.method && t.OrderUrlTemplate == data.url && t.Client == data.client))
          .Include(t => t.Validators).FirstOrDefaultAsync();

        if (definition == null)
        {
            return Results.NotFound("Transaction definition is not found. Please check url is exists in definitions.");
        }

        var transaction = await dbContext!.Transactions!.Where(t => t.Id == transactionId).FirstOrDefaultAsync();

        if(transaction == null)
        {
            return Results.NotFound("Transaction is not found. Please check transaction is exists in active transactions.");
        }

        var validateResponse = ValidatorHelper.ValidateRequests(transaction.RequestUpstreamBody,data.body.ToString(),definition.Validators!);

        if(validateResponse.IsSuccess != 1)
        {
            return Results.Conflict("Request and Order Parameters are Not Matched. Detail : "+validateResponse.Message);
        }

        HttpResponseMessage upHttpResponse;
        HttpClient httpClient = new();

        httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue(data.headers.FirstOrDefault(h => h.Key == "Content-Type").Value));

        foreach (var h in data.headers)
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, h.Value);
        }

        var generatedUpstreamUrl = data.upStreamUrl;

        if(data.urlParams.Count > 0)
        {
            foreach(var urlParam in data.urlParams)
            {
                generatedUpstreamUrl += $"/{urlParam}";
            }
        }

        if (data.method == TransactionDefinition.MethodType.GET)
        {
            if(data.queryParams.Count > 0)
            {
                var firstIterate = true;
                generatedUpstreamUrl += "?";
                foreach(var queryParam in data.queryParams)
                {
                    if(firstIterate)
                    {
                        generatedUpstreamUrl += $"{queryParam.Key}={queryParam.Value}";
                        firstIterate = false;
                    }
                    else
                    {
                        generatedUpstreamUrl += $"&{queryParam.Key}={queryParam.Value}";
                    }
                }
            }            
        }
        else
        {
            JsonContent bodyContent = JsonContent.Create(data.body);
        }

        transaction.OrderUpstreamType = (MethodType)data.method;        
        transaction.OrderUpStreamUrl = generatedUpstreamUrl;
        transaction.OrderUpstreamBody = data.body.ToString();

        transaction.Status = "OrderUpstreamRequested";
        transaction.StatusReason = "Order Upstream Requested Successfully";

        dynamic variables = new ExpandoObject();
        variables.flowType = "WebFlow";

        dynamic messageData = new ExpandoObject();
        messageData.messageName = "Order";
        messageData.correlationKey = transactionId;
        messageData.variables = variables;
        var messageResult = await client.InvokeBindingAsync<dynamic, dynamic>("zeebe-command", "publish-message", messageData);

        return Results.Ok();
    }

    public static async Task<IResult> commandTransaction(IConfiguration configuration,[FromServices] DaprClient client,
        [FromServices] TransactionDBContext dbContext,[FromRoute(Name = "transaction-id")] string transactionId, PostCommand body)
    {
        var stateStoreName = configuration["DAPR_STATE_STORE_NAME"];

        if(body.commandType == CommandType.IvrResponse)
        {
            if(body.details["IvrResult"].ToString() == "Success")
            {
                dynamic messageData = new ExpandoObject();
                dynamic variables = new ExpandoObject();
                messageData.messageName = "IvrResult";
                messageData.correlationKey = transactionId;
                variables.IvrResult = body.details["IvrResult"];
                messageData.variables = variables;
                var messageResult = await client.InvokeBindingAsync<dynamic,dynamic>("zeebe-command", "publish-message", messageData);
            }
        }

        if(body.commandType == CommandType.ApproveOtp)
        {
            var cacheKey = transactionId+"|OtpValue";
            var otpValue = await client.GetStateAsync<string>(stateStoreName,cacheKey);
            if(otpValue == body.details["OtpValue"].ToString())
            {
                dynamic messageData = new ExpandoObject();
                messageData.messageName = "ValidateOtp";
                messageData.correlationKey = transactionId;
                var messageResult = await client.InvokeBindingAsync<dynamic,dynamic>("zeebe-command", "publish-message", messageData);
            }
            else
            {
                return Results.Conflict("Does Not Match");
            }
        }

        if(body.commandType == CommandType.ReSendOtp)
        {
            dynamic messageData = new ExpandoObject();
            messageData.messageName = "ReSentOtp";
            messageData.correlationKey = transactionId;

            var messageResult = await client.InvokeBindingAsync<dynamic,dynamic>("zeebe-command", "publish-message", messageData);
        }

        if(body.commandType == CommandType.ZeebeSetVariables)
        {
            var messageResult = await client.InvokeBindingAsync<dynamic,dynamic>("zeebe-command", "set-variables", body.details);
        }

        return Results.Ok();
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
