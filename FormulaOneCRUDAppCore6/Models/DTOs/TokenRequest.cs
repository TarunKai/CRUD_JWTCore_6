using System.ComponentModel.DataAnnotations;

namespace FormulaOneCRUDAppCore6.Models.DTOs
{
    public class TokenRequest
    {
        [Required]
        public string trToken { get; set; }

        [Required]
        public string trRefreshToken { get; set; }
    }
}
