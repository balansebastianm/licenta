using System.ComponentModel.DataAnnotations;

namespace LicWeb.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Cod Inregistrare")]
        [Required(ErrorMessage = "Cod Necesar")]
        public string CodInregistrare { get; set; }
        [Display(Name = "Adresa Email")]
        [Required(ErrorMessage = "Email necesar")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campul trebuie completat")]
        [DataType(DataType.Password)]
        public string Parola { get; set; }
        [Required(ErrorMessage = "Parolele nu corespund")]
        [DataType(DataType.Password)]
        [Compare(nameof(Parola), ErrorMessage = "Parolele nu corespund")]
        public string RepetaParola { get; set; }

        [Display(Name = "Nume Complet")]
        [Required(ErrorMessage = "Nume necesar")]
        public string NumeComplet { get; set; }
        [Display(Name = "CNP")]
        [Required(ErrorMessage = "CNP necesar")]
        public string CNP { get; set; }
        [Display(Name = "Serie Buletin")]
        [Required(ErrorMessage = "Serie buletin necesara")]
        public string SerieBuletin { get; set; }
        [Display(Name = "Localitate")]
        [Required(ErrorMessage = "Localitate necesara")]
        public string Localitate { get; set; }
        [Display(Name = "Judet")]
        [Required(ErrorMessage = "Judet necesar")]
        public string Judet { get; set; }

    }
}
