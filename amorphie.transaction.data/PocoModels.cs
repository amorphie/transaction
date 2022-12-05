using System.Net;

public record GetTransactionResponse(string Name);
public record GetTransactionStatusResponse(Guid id, TransactionStatus status);

public record PostTransactionRequest(string url, string scope, string client, string reference, dynamic requestBody);
public record PostTransactionRequestResponse(string signalrHub, string hubToken, WebHeaderCollection headers, dynamic responseBody);

public record PostTransactionOrder(string url, string scope, string client, string reference, dynamic requestBody);
public record PostTransactionOrderResponse(WebHeaderCollection headers, dynamic responseBody);

public record PostCommand(string url, string scope, string client, string reference, string command, string reason, dynamic requestBody);

public record GetDefinitionResponse(Guid id,

    string requestUrlTemplate,
    string orderUrlTemplate,
    string client,
    string workflow,
    int ttl,
    string signalRHub,
    GetDefinitionCheckerResponse[] checkers
);

public record GetDefinitionCheckerResponse(string RequestDataPath, string OrderDataPath, DataChecker.ComparerType type);
