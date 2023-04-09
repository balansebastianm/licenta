using System.ComponentModel.DataAnnotations;

namespace LicWeb.ViewModels
{
    public class ForgottenPasswordViewModel
    {
        [Display(Name = "Adresa Email")]
        [Required(ErrorMessage = "Email necesar")]
        public string Email { get; set; }
    }
}
