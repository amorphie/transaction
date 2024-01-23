using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

public record KeyVal(string Key, String Value);
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
    List<KeyVal> headers,
    List<KeyVal> queryParams,
    List<string> urlParams,
    JsonNode body);

public record TransactionInstanceRequestData(Guid transactionId, string scope, string client, string reference, string user, dynamic requestBody);
public record PostTransactionRequestResponse(dynamic response, PostTransactionRequestTransactionResponse transaction);
public record PostTransactionRequestTransactionResponse(Guid id, dynamic workflow, string hub, string token);


public record PostTransactionRequestHubResponse(string hubToken, WebHeaderCollection headers, dynamic responseBody);


public record PostTransactionOrder(
    TransactionDefinition.MethodType method,
    string url,
    string upStreamUrl,
    string scope,
    string client,
    string reference,
    string user,
    List<KeyVal> headers,
    List<KeyVal> queryParams,
    List<string> urlParams,
    JsonNode body);


public record PostTransactionOrderResponse(WebHeaderCollection headers, dynamic responseBody);

public record PostCommand(CommandType commandType, Dictionary<string, dynamic> details);

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

public enum CommandType
{
    ApproveOtp = 0,
    ReSendOtp = 1,
    ZeebeSetVariables = 2,
    IvrResponse = 3
}
