using System.ComponentModel.DataAnnotations;

namespace LicWeb.ViewModels
{
    public class AdeverintaViewModel
    {
        [Key]
        public int Id { get; set; }
        public IFormFile? Adeverinta { get; set; }
        public DateTime MotivareDin { get; set; }
        public DateTime MotivarePana { get; set; }
        public string EmailStudent { get; set; }
        public IFormFile? CheiePrivata { get; set; }

    }
}
