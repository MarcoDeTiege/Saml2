using System.ComponentModel.DataAnnotations;

namespace Sustainsys.Saml2.StubIdp.Models
{
    public class HomePageModel
    {
        [Required]
        public AssertionModel AssertionModel { get; set; }

        public string CustomDescription { get; set; }

        public bool HideDetails { get; set; }
    }
}