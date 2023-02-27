using System.ComponentModel.DataAnnotations;

namespace amorphie.transaction.data.Api.MessagingGateway.Model
{
    public class PhoneString
    {
        [Required(ErrorMessage = "This Field is Mandatory")]
        public string CountryCode { get; set; }
        [Required(ErrorMessage = "This Field is Mandatory")]
        public string Prefix { get; set; }
        [Required(ErrorMessage = "This Field is Mandatory")]
        public string Number { get; set; }


    }
}
