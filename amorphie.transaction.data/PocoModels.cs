using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

public record GetTransactionResponse(string Name);
public record GetTransactionStatusResponse(Guid id, TransactionStatus status);

public record PostTransactionRequest(
    TransactionDefinition.MethodType method,
    string url,
    string upStreamUrl,
    string scope,
    string client,
    string reference,
    string user,
    Dictionary<string, string> headers,
    Dictionary<string, string> queryParams,
    JsonNode body);

public record PostTransactionRequestResponse(string hubToken, WebHeaderCollection headers, dynamic responseBody);

public record PostTransactionOrder(string url, string scope, string client, string reference, dynamic requestBody);
public record PostTransactionOrderResponse(WebHeaderCollection headers, dynamic responseBody);

public record PostCommand(string url, string scope, string client, string reference, string command, string reason, dynamic requestBody);

public record GetDefinitionResponse(Guid id, TransactionDefinition.MethodType requestUrlMethod, string requestUrlTemplate, TransactionDefinition.MethodType orderUrlMethod, string orderUrlTemplate, string client, string workflow, int ttl, GetDefinitionValidatorResponse[] validators);
public record GetDefinitionValidatorResponse(Guid id, string RequestDataPath, string OrderDataPath, DataValidator.ComparerType type);

public record PostDefinitionRequest(TransactionDefinition.MethodType requestUrlMethod, string requestUrlTemplate, TransactionDefinition.MethodType orderUrlMethod, string orderUrlTemplate, string client, string workflow, int ttl);

public record PostPublishStatusRequest(Guid id, string status, string reason, string details);

public record PostCreateTransactionHubTokenRequest(Guid transactionId, Guid definitionId, string scope, string client, string user, string reference, int ttl);


public record TransactionTokenStatus
{
    public String? Token { get; set; }
    public Guid TransactionId { get; set; }
    public Guid DefinitionId { get; set; }
    public string? Scope { get; set; }
    public string? Client { get; set; }
    public string? User { get; set; }
    public string? Reference { get; set; }
    public int TTL { get; set; } = 0;
     public DateTime IssuedAt { get; set; } = DateTime.Now;
     public DateTime? ExpiryAt { get; set; }
     public DateTime? ExpiredAt { get; set; } 
     public DateTime? LastValidatedAt { get; set; } 

};
