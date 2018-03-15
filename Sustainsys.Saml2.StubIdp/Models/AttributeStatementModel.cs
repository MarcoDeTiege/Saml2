using System.ComponentModel.DataAnnotations;

namespace Sustainsys.Saml2.StubIdp.Models
{
    public class AttributeStatementModel
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
