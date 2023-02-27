using amorphie.transaction.data.Attributes;
using System.ComponentModel.DataAnnotations;
namespace amorphie.transaction.data.Api.MessagingGateway.Model
{
    public class SmsRequestString
    {
        /// <summary>
        /// You have to submit sender source. There is two senders are available, **Burgan** end On. If you do not know sender of receiver, please set as AutoDetect.
        /// </summary>
        /// <remarks>
        /// System uses header configuration to set sender automatically.
        /// </remarks>
        [Required]
        public SenderType Sender { get; set; }
        [Required]
        public SmsTypes SmsType { get; set; }
        [Required]
        public PhoneString Phone { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Content { get; set; }
        public long? CustomerNo { get; set; }
        [CitizenshipNo(10,11)]
        public string? CitizenshipNo { get; set; }
        public string[] Tags { get; set; }
        public Process Process { get; set; }
    }
}
