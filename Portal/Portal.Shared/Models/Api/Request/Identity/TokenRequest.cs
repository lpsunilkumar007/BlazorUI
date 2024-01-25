using System.ComponentModel.DataAnnotations;

namespace Portal.Shared.Models.Api.Request.Identity
{
    public class TokenRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
