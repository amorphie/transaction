using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

byte[] JwtKey = Encoding.ASCII.GetBytes("thisissupersecretthisissupersecretthisissupersecrethisissupersecrett");
string JwtIssuer = "https://transaction.amorphie.burgan.com.tr/";
string JwtAudience = "https://transaction.amorphie.burgan.com.tr/";

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ITransactionHubJwtUtils, TransactionHubJwtUtils>();

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

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/transaction")))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors();

app.MapControllers();

app.MapPost("/security/createToken",
    [AllowAnonymous]
(TransactionInfo info) =>

{
    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Issuer = JwtIssuer,
        Audience = JwtAudience,
        Claims = new Dictionary<string, object> { { "user", info.user }, { "transactionId", info.transactionId } },
        Expires = DateTime.UtcNow.AddSeconds(info.ttl),
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
    ITransactionHubJwtUtils _jwtutil;


    public TransactionHub(ILogger<TransactionHub> logger, ITransactionHubJwtUtils jwtutil)
    {
        _logger = logger;
        _jwtutil = jwtutil;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task NewMessage(string token, string message) =>
        await Clients.All.SendAsync("messageReceived", token, message);

    public override Task OnConnectedAsync()
    {

        var accessToken = Context.GetHttpContext()!.Request.Query["access_token"];

        /*

        if (accessToken[0] == null)
        {
            Context.Abort();
        }
        
        var id = _jwtutil.ValidateToken(accessToken!);

         if (id == null)
        {
            Context.Abort();
        }
        */

        //var token = _jwtutil.GenerateToken("38552069000", "ed39bb4d-fbb7-4c88-8974-e1e3b0603b35", 36000);
        //_logger.LogInformation($"Token: {token} ");            


        //_logger.LogInformation($"Clients and groups: {Context.User.Identity.IsAuthenticated} - {Context.Features} ");

        //_logger.LogInformation($"Client Connected: {accessToken} - {Context.ConnectionId} ");
        //_logger.LogInformation($"User Connected: {Context.User.Claims.First(x => x.Type == "user").Value}");
        _logger.LogInformation($"Client Connected: {Context.ConnectionId}");

        return base.OnConnectedAsync();
    }

}

