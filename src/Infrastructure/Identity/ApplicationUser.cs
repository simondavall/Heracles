using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Heracles.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [ProtectedPersonalData]
        [MaxLength(200)]
        public string DisplayName { get; set; }
    }
}
