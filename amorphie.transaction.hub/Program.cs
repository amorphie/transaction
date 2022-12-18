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
            builder.WithOrigins("http://localhost:5500")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
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
        OnMessageReceived = context =>
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
        OnTokenValidated = context =>
        {
            //context.Fail(new Exception("Token Revoked"));
            return Task.CompletedTask;
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

var app = builder.Build();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors();

app.MapPost("/security/create-token",
[AllowAnonymous]
(PostCreateTransactionHubTokenRequest data) =>
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
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return Results.Ok(tokenHandler.WriteToken(token));
});


app.MapHub<TransactionHub>("/transaction");

app.Run();

public record TransactionInfo(string user, string transactionId, int ttl);

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TransactionHub : Hub
{
    ILogger<TransactionHub> _logger;


    public TransactionHub(ILogger<TransactionHub> logger)
    {
        _logger = logger;

    }

    public async Task NewMessage(string token, string message) =>
        await Clients.All.SendAsync("messageReceived", token, message);

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation($"Client Connected: {Context.ConnectionId}");

        return base.OnConnectedAsync();
    }

}

