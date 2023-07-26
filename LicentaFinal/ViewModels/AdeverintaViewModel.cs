using System.ComponentModel.DataAnnotations;

namespace LicWeb.ViewModels
{
    public class AdeverintaViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public IFormFile? Adeverinta { get; set; }
        [Required]
        public DateTime MotivareDin { get; set; }
        [Required]
        public DateTime MotivarePana { get; set; }
        [Required]
        public string EmailStudent { get; set; }
        [Required]
        public IFormFile? CheiePrivata { get; set; }
        [Required]
        public DateTime DataConsultatie { get; set; }
        [Required]
        public string Diagnostic { get; set; }

    }
}
