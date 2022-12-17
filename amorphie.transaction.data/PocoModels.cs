using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

public record GetTransactionResponse(string Name);
public record GetTransactionStatusResponse(Guid id, TransactionStatus status);

public record PostTransactionRequest(TransactionDefinition.MethodType method, string url, string upStreamUrl, string scope, string client, string reference, string user, Dictionary<string,string> headers, Dictionary<string,string> queryParams, JsonNode body);
public record PostTransactionRequestResponse(string hubToken, WebHeaderCollection headers, dynamic responseBody);

public record PostTransactionOrder(string url, string scope, string client, string reference, dynamic requestBody);
public record PostTransactionOrderResponse(WebHeaderCollection headers, dynamic responseBody);

public record PostCommand(string url, string scope, string client, string reference, string command, string reason, dynamic requestBody);

public record GetDefinitionResponse(Guid id, TransactionDefinition.MethodType requestUrlMethod, string requestUrlTemplate, TransactionDefinition.MethodType orderUrlMethod, string orderUrlTemplate, string client, string workflow, int ttl, GetDefinitionValidatorResponse[] validators);
public record GetDefinitionValidatorResponse(Guid id, string RequestDataPath, string OrderDataPath, DataValidator.ComparerType type);

public record PostDefinitionRequest(TransactionDefinition.MethodType requestUrlMethod, string requestUrlTemplate, TransactionDefinition.MethodType orderUrlMethod, string orderUrlTemplate, string client, string workflow, int ttl);
