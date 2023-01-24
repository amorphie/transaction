using System.Text.Json;
using SecretExtensions;

var builder = WebApplication.CreateBuilder(args);
await builder.Configuration.AddVaultSecrets("transaction-secretstore","bbt.gateway.messaging");
var client = new DaprClientBuilder().Build();

#pragma warning disable 618
var configurations = await client.GetConfiguration("configstore", new List<string>() { "config-amorphie-transaction-db" });
#pragma warning restore 618 

builder.Services.AddDbContext<TransactionDBContext>
   (options => options.UseNpgsql(configurations.Items["config-amorphie-transaction-db"].Value));

builder.Services.AddDaprClient();
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();
var kk = app.Configuration["id"];
var conf = app.Configuration;

app.Logger.LogInformation("Db String : "+configurations.Items["config-amorphie-transaction-db"].Value);


app.UseTransactionMiddleware();
app.UseCors();
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


        