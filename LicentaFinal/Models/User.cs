using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LicWeb.Models
{
    [Table("AspNetUsers")]
    public class User : IdentityUser
    {
        [Required]
        public string? Nume { get; set; }
        [Required]
        public string? Prenume { get; set; }
        [Required]
        public int StatusCont { get; set; } //0 - pending inscriere -1 - inscris,neacceptat, 2-acceptat
        [Required]
        public string TokenInregistrare { get; set; }
        [Required]
        public string Functie { get; set; }
        [Required]
        public string Judet { get; set; }
        [Required]
        public string Localitate { get; set; }

        public static implicit operator string(User v)
        {
            throw new NotImplementedException();
        }
    }
}
