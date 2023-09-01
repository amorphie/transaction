using System.Text.Json;
using amorphie.transaction.data.Api.MessagingGateway;
using Newtonsoft.Json;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

await builder.Configuration.AddVaultSecrets(builder.Configuration["DAPR_SECRET_STORE_NAME"], new string[] { "DatabaseConnections", "ServiceConnections", "MockData" });

builder.Services.AddDbContext<TransactionDBContext>
    (options => options.UseNpgsql(builder.Configuration["TransactionDb"]));



builder.Services.AddDaprClient();
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRefitClient<IMessagingGatewayApi>(new RefitSettings
{
    ContentSerializer = new NewtonsoftJsonContentSerializer(
                        new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                        }
                )
})
               .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["MessagingGatewayUri"]));


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

app.MapTransactionWorkersEndpoints();

try
{
    app.Logger.LogInformation("Starting application...");
    app.Run();

}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Aplication is terminated unexpectedly ");
}

