using System.Text.Json.Nodes;

public static class TransactionWorkersModule
{

    static WebApplication _app = default!;

    public static void MapTransactionWorkersEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapPost("/RequestReceivedWorker", requestReceived)
            .WithOpenApi()
            .WithSummary("Send command messaage to transaction flow.")
            .WithTags("Transaction Worker");

        _app.MapPost("/IterateWorker", iterateRequest)
            .WithOpenApi()
            .WithSummary("Send command messaage to transaction flow.")
            .WithTags("Transaction Worker");
    }

    static IResult requestReceived(
        [FromBody] dynamic body,
        HttpRequest request,
        HttpContext httpContext,
        [FromServices] DaprClient client,
        [FromServices] TransactionDBContext context
    )
    {
        Guid id = Guid.Parse(body.GetProperty("transactionId").ToString()!);

        client.InvokeMethodAsync<PostPublishStatusRequest, string>(
            HttpMethod.Post,
            "amorphie-transaction-hub",
            "transaction/publish-status",
            new PostPublishStatusRequest(
                id,
                request.Headers["Cm-Status"].ToString() ?? "",
                request.Headers["Cm-Reason"].ToString() ?? "",
                request.Headers["Cm-Details"].ToString() ?? ""
            ));

        string headers = String.Empty;
        foreach (var key in request.Headers.Keys)
            headers += key + "=" + request.Headers[key] + Environment.NewLine;


        _app.Logger.LogInformation($"RequestReceivedWorker is called with {body}, {headers}");
        return Results.Ok();
    }

    static IResult iterateRequest(
       [FromBody] dynamic body,
       HttpRequest request,
       HttpContext httpContext,
       [FromServices] DaprClient client,
       [FromServices] TransactionDBContext context
   )
    {
        _app.Logger.LogInformation($"IterateWorker is called with {body}");

        Guid id = Guid.Parse(body.GetProperty("transactionId").ToString()!);

        client.InvokeMethodAsync<PostPublishStatusRequest, string>(
            HttpMethod.Post,
            "amorphie-transaction-hub",
            "transaction/publish-status",
            new PostPublishStatusRequest(
                id,
                request.Headers["Cm-Status"].ToString() ?? "",
                request.Headers["Cm-Reason"].ToString() ?? "",
                request.Headers["Cm-Details"].ToString() ?? ""
            ));

        return Results.Ok();
    }
}