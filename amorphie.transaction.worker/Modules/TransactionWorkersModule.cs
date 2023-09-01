using System.Text.Json;
using System.Text.Json.Nodes;
using amorphie.transaction.data.Api.MessagingGateway;
using amorphie.transaction.data.Api.MessagingGateway.Model;

public static class TransactionWorkersModule
{

    static WebApplication _app = default!;

    public static void MapTransactionWorkersEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapPost("/IvrCall", ivrCall)
            .WithOpenApi()
            .WithSummary("Send a Ivr Call Request.")
            .WithTags("Ivr Call");

        _app.MapPost("/SendApprovalOtp", sendApprovalOtp)
            .WithOpenApi()
            .WithSummary("Send otp to transaction.")
            .WithTags("SendOtp Worker");

        _app.MapPost("/Fraud", fraudCheck)
            .WithOpenApi()
            .WithSummary("Fraud Check.")
            .WithTags("Fraud Check");

        _app.MapPost("/MakeTransfer", makeTransfer)
            .WithOpenApi()
            .WithSummary("Order Transaction.")
            .WithTags("Order");

        _app.MapPost("/IsIvrRequired", isIvrRequired)
            .WithOpenApi()
            .WithSummary("Check Ivr is Mandatory Or Not.")
            .WithTags("Ivr");
    }

    static async Task<IResult> makeTransfer(
       [FromBody] dynamic body,
       HttpRequest request,
       HttpContext httpContext,
       [FromServices] DaprClient client,
       [FromServices] TransactionDBContext context
    )
    {
        Guid id = Guid.Parse(body.GetProperty("transactionId").ToString()!);
        var transaction = await context!.Transactions!.FirstOrDefaultAsync(t => t.Id == id);

        var httpClient = new HttpClient();

        var httpResponse = await httpClient.PostAsync(transaction.OrderUpStreamUrl, JsonContent.Create(transaction.OrderUpstreamBody));

        httpResponse.EnsureSuccessStatusCode();

        var response = await httpResponse.Content.ReadAsStringAsync();

        transaction.OrderUpstreamResponse = response;
        transaction.Status = "Completed";
        transaction.StatusReason = "Transaction is completed successfully";
        await context.SaveChangesAsync();

        var hubResponse = await client.InvokeMethodAsync<PostPublishStatusRequest, string>(
            HttpMethod.Post,
            "amorphie-transaction-hub.test-amorphie-transaction-hub",
            "transaction/publish-status",
            new PostPublishStatusRequest(
                id,
                "TransactionCompleted",
                "SendOtp",
                "SendOtp"
            ));

        return Results.Ok();
    }

    static async Task<IResult> ivrCall(
       [FromBody] dynamic body,
       HttpRequest request,
       HttpContext httpContext,
       [FromServices] DaprClient client,
       [FromServices] TransactionDBContext context
    )
    {
        await Task.CompletedTask;

        Guid id = Guid.Parse(body.GetProperty("transactionId").ToString()!);

        var hubResponse = await client.InvokeMethodAsync<PostPublishStatusRequest, string>(
            HttpMethod.Post,
            "amorphie-transaction-hub.test-amorphie-transaction-hub",
            "transaction/publish-status",
            new PostPublishStatusRequest(
                id,
                "IvrCall",
                "SendOtp",
                "SendOtp"
            ));

        return Results.Ok();
    }

    static async Task<IResult> sendApprovalOtp(
       [FromBody] dynamic body,
       HttpRequest request,
       HttpContext httpContext,
       [FromServices] DaprClient client,
       [FromServices] TransactionDBContext context,
       IConfiguration configuration,
       IMessagingGatewayApi messagingGatewayApi
    )
    {

        var stateStoreName = configuration["DAPR_STATE_STORE_NAME"];

        Guid id = Guid.Parse(body.GetProperty("transactionId").ToString()!);

        var rand = new Random();

        var otpValue = rand.Next(100000, 999999).ToString();

        var cacheKey = id + "|OtpValue";
        await client.SaveStateAsync<string>(stateStoreName, cacheKey, otpValue);

        SmsRequestString otpRequest = new();
        otpRequest.Content = "Para transferi işlemini tamamlamak için kodu kullanın. " + otpValue;
        otpRequest.Phone = new()
        {
            CountryCode = configuration["TestData:CountryCode"],
            Prefix = configuration["TestData:Prefix"],
            Number = configuration["TestData:Number"]
        };
        otpRequest.Process = new()
        {
            Action = "Transaction Otp Sending",
            ItemId = id.ToString(),
            Identity = "TransactionMicroservice",
            Name = "TransactionMicroservice"
        };
        otpRequest.Sender = SenderType.AutoDetect;
        otpRequest.SmsType = SmsTypes.Otp;

        var loggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("OtpEvents");

        try
        {
            logger.LogInformation("Otp request : " + JsonSerializer.Serialize(otpRequest));
            var otpResponse = await messagingGatewayApi.SendOtp(otpRequest);
            logger.LogInformation("Otp Servis cevabı : " + JsonSerializer.Serialize(otpResponse));
        }
        catch (System.Exception ex)
        {
            logger.LogInformation("Otp Servisi çağrılamadı : " + ex.Message);
        }

        logger.LogInformation("Otp Value : " + otpValue);

        var response = await client.InvokeMethodAsync<PostPublishStatusRequest, string>(
            HttpMethod.Post,
            "amorphie-transaction-hub.test-amorphie-transaction-hub",
            "transaction/publish-status",
            new PostPublishStatusRequest(
                id,
                "SendOtp",
                "SendOtp",
                "SendOtp"
            ));

        return Results.Ok();
    }

    static async Task<IResult> isIvrRequired(
       [FromBody] dynamic body,
       HttpRequest request,
       HttpContext httpContext,
       [FromServices] DaprClient client,
       [FromServices] TransactionDBContext context
    )
    {
        await Task.CompletedTask;

        Guid id = Guid.Parse(body.GetProperty("transactionId").ToString()!);
        long processInstanceKey = Convert.ToInt64(request.Headers["X-Zeebe-Process-Instance-Key"]);

        var details = new Dictionary<string, dynamic>();
        details["elementInstanceKey"] = processInstanceKey;
        details["variables"] = new Dictionary<string, dynamic>();

        details["variables"]["isIvrRequired"] = true;

        await client.InvokeMethodAsync<PostCommand>("amorphie-transaction.test-amorphie-transaction", "transaction/instance/" + id + "/command", new PostCommand(
            CommandType.ZeebeSetVariables,
            details));

        return Results.Ok();
    }

    static async Task<IResult> fraudCheck(
       [FromBody] dynamic body,
       HttpRequest request,
       HttpContext httpContext,
       [FromServices] DaprClient client,
       [FromServices] TransactionDBContext context
    )
    {
        Guid id = Guid.Parse(body.GetProperty("transactionId").ToString()!);
        long processInstanceKey = Convert.ToInt64(request.Headers["X-Zeebe-Process-Instance-Key"]);

        using var httpClient = new HttpClient();
        JsonContent bodyContent = JsonContent.Create(new { test = "123" });
        var httpResponse = await httpClient.PostAsJsonAsync("http://localhost:3000/fraud", new { test = "123" });
        var response = await httpResponse.Content.ReadAsStringAsync();
        var responseBody = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(response);

        var details = new Dictionary<string, dynamic>();
        details["elementInstanceKey"] = processInstanceKey;
        details["variables"] = new Dictionary<string, dynamic>();
        foreach (var element in responseBody)
        {
            details["variables"][element.Key] = element.Value;
        }
        await client.InvokeMethodAsync<PostCommand>("amorphie-transaction.test-amorphie-transaction", "transaction/instance/" + id + "/command", new PostCommand(
            CommandType.ZeebeSetVariables,
            details));

        return Results.Ok();
    }
}