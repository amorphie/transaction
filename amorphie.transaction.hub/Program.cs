using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Dapr.Client;
using System.Security.Principal;

var builder = WebApplication.CreateBuilder(args);
var client = new DaprClientBuilder().Build();

byte[] JwtKey = Encoding.ASCII.GetBytes("thisissupersecretthisissupersecretthisissupersecrethisissupersecrett");
string JwtIssuer = "https://transaction.amorphie.burgan.com.tr/";
string JwtAudience = "https://transaction.amorphie.burgan.com.tr/";

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = (context) =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/transaction")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = async context =>
        {
            var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("JwtBearerEvents");
            logger.LogInformation($"Token validated{context}");
            logger.LogInformation($"Token validated{context.Principal?.Identity?.Name}");

            string? transactionId = context.Principal?.Identity?.Name;

            if (string.IsNullOrEmpty(transactionId))
            {
                context.Fail(new Exception("Token does not contain transaction id."));
                return;
            }

            var tokenStatus = await client.GetStateAsync<TransactionTokenStatus>("transaction-cache", transactionId);
            tokenStatus.LastValidatedAt = DateTime.Now;
            await client.SaveStateAsync<TransactionTokenStatus>("transaction-cache", transactionId, tokenStatus);

            return;
        }
    };

    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = JwtIssuer,
        ValidAudience = JwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(JwtKey),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization();
builder.Services.AddSignalR();



builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapPost("/security/create-token",
[AllowAnonymous] async (PostCreateTransactionHubTokenRequest data) =>
{

    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        // User and key of principal is Transaction ID forever.
        Subject = new GenericIdentity(data.transactionId.ToString()),
        Issuer = JwtIssuer,
        Audience = JwtAudience,
        Claims = new Dictionary<string, object> {
            { "transactionId", data.transactionId },
            { "definitionId", data.definitionId },
            { "user", data.user },
            { "scope", data.scope },
            { "client", data.client },
            { "reference", data.reference } },
        Expires = DateTime.UtcNow.AddSeconds(data.ttl),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(JwtKey), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.WriteToken(
        tokenHandler.CreateToken(tokenDescriptor)
        );


    await client.SaveStateAsync<TransactionTokenStatus>(
        "transaction-cache",
        data.transactionId.ToString(),
        new TransactionTokenStatus { Token = token, TransactionId = data.transactionId, DefinitionId = data.definitionId, Scope = data.scope, Client = data.client, User = data.user, Reference = data.reference, TTL = data.ttl, ExpiryAt = tokenDescriptor.Expires },
        metadata: new Dictionary<string, string> { { "ttlInSeconds", $"{data.ttl}" } }
        );


    return Results.Ok(token);
});

app.MapPost("/transaction/publish-status",
[AllowAnonymous] async Task<IResult> (PostPublishStatusRequest data, IHubContext<TransactionHub> hubContext) =>
{
    await hubContext.Clients.User(data.id.ToString()).SendAsync("on-status-changed", data.status, data.reason, data.details);

    return Results.Ok("");
});

app.UseWhen(context => context.Request.Path.StartsWithSegments("/transaction/publish-status"),app => app.UseMiddleware<TransactionMiddleware>());
app.MapHub<TransactionHub>("/transaction/hub");

app.Run();

public record TransactionInfo(string user, string transactionId, int ttl);


//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize]
public class TransactionHub : Hub
{
    ILogger<TransactionHub> _logger;


    public TransactionHub(ILogger<TransactionHub> logger)
    {
        _logger = logger;
    }

    //public async Task PublishStatus(Guid transacionId, string status, string reason, string details) =>
    //await Clients.All.SendAsync("publishStatus", status, reason, details);

    public override Task OnConnectedAsync()
    {

        //Context?.UserIdentifier = Context?.User?.Identity?.Name;

        _logger.LogInformation($"Client Connected: {Context.ConnectionId}, user id : {Context?.User?.Identity?.Name}, user ident: {this.Context?.UserIdentifier}");
        return base.OnConnectedAsync();
    }

}

public class NameUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection) => 
    (connection?.User?.Identity?.Name ?? "");
}

public class TransactionMiddleware
{
    private readonly RequestDelegate _next;
    public TransactionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
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
        }
        finally
        {
            
        }
    }

}