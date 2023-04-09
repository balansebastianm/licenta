using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LicWeb.ViewModels
{
    public class MaterieViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string NumeMaterie { get; set; }
        [Required]
        public int ProfesorCursId { get; set; }
        [Required]
        public int ProfesorSeminarId { get; set; }
        [Required]
        public int ZiuaSaptamaniiCurs { get; set; }

        public TimeOnly StartTimeCurs { get; set; }
        [Required]
        public int ZiuaSaptamaniiLabSeminar { get; set; }

        public TimeOnly StartTimeLabSeminar { get; set; }

    }
}
