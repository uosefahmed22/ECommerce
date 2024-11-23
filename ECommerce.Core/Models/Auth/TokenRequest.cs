using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Auth
{
    public class TokenRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Token must be between 10 and 100 characters.")]
        public string Token { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "RefreshToken must be between 10 and 100 characters.")]
        public string RefreshToken { get; set; }
    }
}
