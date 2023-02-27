using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace amorphie.transaction.data.Api.MessagingGateway.Model
{
    public class SendSmsOtpResponse
    {
        public Guid TxnId { get; set; }
        public SendSmsResponseStatus Status { get; set; }

    }
}

