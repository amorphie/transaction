public class TransactionMiddleware
{
    private readonly RequestDelegate _next;
    public TransactionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TransactionDBContext dbContext, ILogger<TransactionMiddleware> logger)
    {
        try
        {
            context.Request.EnableBuffering();
            var bodyAsText = await new System.IO.StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            await _next(context);
        }
        catch (Exception ex)
        {
            logger.LogError("Transaction failed! Exception : " + ex);
        }
        finally
        {
            await dbContext.SaveChangesAsync();
        }
    }

}

public static class TransactionMiddlewareExtensions
{
    public static void UseTransactionMiddleware(
        this IApplicationBuilder builder)
    {
        builder.UseWhen(context =>
        {
            return context.Request.Path.StartsWithSegments("/transaction/instance")
            && context.Request.Method == "POST" && (context.Request.Path.ToString().Split('/').Last() == "request"
            || context.Request.Path.ToString().Split('/').Last() == "order"
            || context.Request.Path.ToString().Split('/').Last() == "command");
        }, appBuilder =>
        {
            appBuilder.UseMiddleware<TransactionMiddleware>();
        });
    }
}