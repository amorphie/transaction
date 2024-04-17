
using Newtonsoft.Json;

public static class ValidatorHelper
{
    public static ValidateResponse ValidateRequests(string RequestBody, string OrderBody, List<DataValidator> validators)
    {
        var requestBody = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(RequestBody);
        var orderBody = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(OrderBody);
        foreach (var validator in validators)
        {
            var parameters = GetJsonKey(validator);
            if (validator.Type == DataValidator.ComparerType.Number)
            {
                dynamic requestVal = default!;
                dynamic orderVal = default!;

                foreach (var param in parameters.Item1)
                {
                    requestVal = requestBody[param];
                }
                foreach (var param in parameters.Item2)
                {
                    orderVal = orderBody[param];
                }

                if (Convert.ToDouble(requestVal) != Convert.ToDouble(orderVal))
                {
                    return new ValidateResponse()
                    {
                        IsSuccess = 0,
                        Message = $"Request Path:{validator.RequestDataPath} AND Order Path:{validator.OrderDataPath} Are Not Matched"
                    };
                }

            }
            if (validator.Type == DataValidator.ComparerType.String)
            {
                dynamic requestVal = default!;
                dynamic orderVal = default!;

                foreach (var param in parameters.Item1)
                {
                    requestVal = requestBody[param];
                }
                foreach (var param in parameters.Item2)
                {
                    orderVal = orderBody[param];
                }

                if (Convert.ToString(requestVal) != Convert.ToString(orderVal))
                {
                    return new ValidateResponse()
                    {
                        IsSuccess = 0,
                        Message = $"Request Path:{validator.RequestDataPath} AND Order Path:{validator.OrderDataPath} Are Not Matched"
                    };
                }
            }
            if (validator.Type == DataValidator.ComparerType.Boolean)
            {
                dynamic requestVal = default!;
                dynamic orderVal = default!;

                foreach (var param in parameters.Item1)
                {
                    requestVal = requestBody[param];
                }
                foreach (var param in parameters.Item2)
                {
                    orderVal = orderBody[param];
                }

                if (Convert.ToBoolean(requestVal) != Convert.ToBoolean(orderVal))
                {
                    return new ValidateResponse()
                    {
                        IsSuccess = 0,
                        Message = $"Request Path:{validator.RequestDataPath} AND Order Path:{validator.OrderDataPath} Are Not Matched"
                    };
                }
            }
        }

        return new ValidateResponse() { IsSuccess = 1 };
    }

    public static (string[], string[]) GetJsonKey(DataValidator validator)
    {
        var requestSplitted = validator.RequestDataPath.Split('.').Skip(1).ToArray();
        var orderSplitted = validator.OrderDataPath.Split('.').Skip(1).ToArray();
        return (requestSplitted, orderSplitted);
    }
}


public class ValidateResponse
{
    public int IsSuccess { get; set; }
    public string? Message { get; set; }
}