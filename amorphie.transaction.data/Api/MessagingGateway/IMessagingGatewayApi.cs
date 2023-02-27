using amorphie.transaction.data.Api.MessagingGateway.Model;
using Refit;

namespace amorphie.transaction.data.Api.MessagingGateway
{
    public interface IMessagingGatewayApi
    {
        [Post("/api/v2/Messaging/sms/message/string")]
        Task<SendSmsOtpResponse> SendOtp(SmsRequestString request);
    }
}
