var builder = WebApplication.CreateBuilder(args);


var client = new DaprClientBuilder().Build();
var configurations = await client.GetConfiguration("configstore", new List<string>() { "config-amorphie-transaction-db" });


builder.Services.AddDbContext<TransactionDBContext>
    (options => options.UseNpgsql(configurations.Items["config-amorphie-transaction-db"].Value));

builder.Services.AddDaprClient();
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
	options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseCloudEvents();
app.UseRouting();
app.MapSubscribeHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.MapTransactionEndpoints();

try
{
    app.Logger.LogInformation("Starting application...");
    app.Run();

}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Aplication is terminated unexpectedly ");
}
