using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LicWeb.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string? CNP { get; set; }
        [Required]
        public string? SerieBuletin { get; set; }
        [Required]
        public string? NumeComplet { get; set; }
        [Required]
        public int StatusCont { get; set; } //0 - pending inscriere -1 - inscris,neacceptat, 2-acceptat
        [Required]
        public string TokenInregistrare { get; set; }
        [Required]
        public string Localitate { get; set; }
        [Required]
        public string Judet { get; set; }
    }
}
