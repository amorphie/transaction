using amorphie.transaction.data;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
System.Threading.Thread.Sleep(5000);
await builder.Configuration.AddVaultSecrets(builder.Configuration["DAPR_SECRET_STORE_NAME"],new string[]{"DatabaseConnections","ServiceConnections"});


builder.Services.AddDbContext<TransactionDBContext>
   (options => options.UseNpgsql(builder.Configuration["TransactionDb"],t => t.MigrationsAssembly("amorphie.transaction.data")));



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

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<TransactionDBContext>();
db.Database.Migrate();

app.UseTransactionMiddleware();
app.UseCors();
app.UseCloudEvents();
app.UseRouting();
app.MapSubscribeHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.MapTransactionEndpoints();
app.MapTestModuleEndpoints();

try
{
    app.Logger.LogInformation("Starting application...");
    app.Run();

}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Aplication is terminated unexpectedly ");
}


        
