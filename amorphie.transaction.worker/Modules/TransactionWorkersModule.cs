public static class TransactionWorkersModule
{
    const string STATE_STORE = "transaction-cache";
    static WebApplication _app = default!;

    public static void MapTransactionWorkersEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapPost("/RequestReceivedWorker", requestReceived)
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
        _app.Logger.LogInformation($"RequestReceivedWorker is called with {body}, { request.Headers}");
        return Results.Ok();
    }
}