using static Transaction;

public static class TestModule
{
    static WebApplication _app = default!;

    public static void MapTestModuleEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapGet("/Simulate", simulateUrl)
        .WithOpenApi()
        .WithSummary("Gets registered transation definitions.")
        .WithDescription("Returns existing transaction definitions with metadata.Query parameter url is can contain request or order url of transaction.")
        .WithTags("Definition")
        .Produces<GetDefinitionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        _app.MapPost("/Order", orderUrl)
        .WithOpenApi()
        .WithSummary("Gets registered transation definitions.")
        .WithDescription("Returns existing transaction definitions with metadata.Query parameter url is can contain request or order url of transaction.")
        .WithTags("Definition")
        .Produces<GetDefinitionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }

    static async Task<IResult> simulateUrl(
        HttpRequest request,
        HttpContext httpContext,
        [FromServices] DaprClient client,
        [FromServices] TransactionDBContext context,
        IConfiguration configuration
    )
    {
        await Task.CompletedTask;

        return Results.Ok();
    }

    static async Task<IResult> orderUrl(
        HttpRequest request,
        HttpContext httpContext,
        [FromServices] DaprClient client,
        [FromServices] TransactionDBContext context,
        IConfiguration configuration
    )
    {
        await Task.CompletedTask;
        return Results.Ok();
    }
}
