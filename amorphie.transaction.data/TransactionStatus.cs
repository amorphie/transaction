
public record TransactionStatus(string status, string openBankingStatus, string iso20022Status, string[] availableCommands);

public static class StatusList
{
    static readonly List<TransactionStatus> statusList;

    static StatusList()
    {
        statusList = new List<TransactionStatus>();

        statusList.Add(new TransactionStatus("received", "RCVD", "B", new string[] { }));
        statusList.Add(new TransactionStatus("processing", "ACTC", "B", new string[] { }));

        statusList.Add(new TransactionStatus("waiting-sca", "PDNG", "B", new string[] { }));

        statusList.Add(new TransactionStatus("waiting-otp-validation", "PDNG", "B", new string[] { "validate", "retry" }));
        statusList.Add(new TransactionStatus("waiting-device-validation", "PDNG", "B", new string[] { "validate" }));
        statusList.Add(new TransactionStatus("waiting-push-validation", "PDNG", "B", new string[] { "validate" }));

        statusList.Add(new TransactionStatus("ready-to-order", "PDNG", "B", new string[] { }));
        statusList.Add(new TransactionStatus("post-processing", "PDNG", "B", new string[] { }));

        statusList.Add(new TransactionStatus("completed", "ACCC", "E", new string[] { }));
        statusList.Add(new TransactionStatus("expired", "CANC", "I", new string[] { }));
        statusList.Add(new TransactionStatus("rejected", "RJCT", "I", new string[] { }));
        statusList.Add(new TransactionStatus("canceled", "CANC", "I", new string[] { }));
    }

    public static string[] GetCommands(string status)
    {
        var selectedStatus = statusList.FirstOrDefault(s => s.status == status);

        if (selectedStatus is not null)
            return selectedStatus.availableCommands;
        else
            throw new Exception("status is not found.");
    }
}