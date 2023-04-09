using System.ComponentModel.DataAnnotations;

namespace LicWeb.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Adresa Email")]
        [Required(ErrorMessage = "Email necesar")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Parola { get; set; }

    }
}
