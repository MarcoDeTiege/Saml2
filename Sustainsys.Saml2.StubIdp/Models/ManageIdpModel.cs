using System.ComponentModel.DataAnnotations;

namespace Sustainsys.Saml2.StubIdp.Models
{
    public class ManageIdpModel
    {
        [Required]
        [Display(Name = "Configuration data")]
        [DataType(DataType.MultilineText)]
        public string JsonData { get; set; }
    }
}