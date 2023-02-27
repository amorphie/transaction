using System.ComponentModel.DataAnnotations;

namespace amorphie.transaction.data.Api.MessagingGateway.Model
{
    public class Process
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is mandatory.")]
        public string Name { get; set; }
        public string ItemId { get; set; }
        public string Action { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is mandatory.")]
        public string Identity { get; set; }
    }
}
