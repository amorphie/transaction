
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface ITransactionHubJwtUtils
{
    public string GenerateToken(string user, string transactionId, int ttl);
    public string? ValidateToken(string token);
}

public class TransactionHubJwtUtils : ITransactionHubJwtUtils
{
    byte[] key = Encoding.ASCII.GetBytes("thisissupersecretthisissupersecretthisissupersecrethisissupersecrett");
    

    public string GenerateToken(string user, string transactionId, int ttl)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("transactionId", transactionId), new Claim("user", user) }),
            Claims =  new Dictionary<string, object> { {"user", user },{"transactionId", transactionId} },
            Expires = DateTime.UtcNow.AddSeconds(ttl),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string? ValidateToken(string token)
    {
        if (token == null) 
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var transactionId = jwtToken.Claims.First(x => x.Type == "transactionId").Value;

            // return user id from JWT token if validation successful
            return transactionId;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }
}