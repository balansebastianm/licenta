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
        public int NrCredite { get; set; }
        [Required]
        public int IdSpecializare { get; set; }
        [Required]
        public int AnStudii { get; set; }
        [Required]
        public int ModulStudii { get; set; }
        [Required]
        public int ProfesorCursId { get; set; }
        [Required]
        public int ProfesorSeminarId { get; set; }
        [Required]
        public float ProcentajPrezCurs { get; set; }
        [Required]
        public float ProcentajPrezSeminar { get; set; }
        [Required]
        public int ZiuaSaptamaniiCurs { get; set; }

        public TimeOnly StartTimeCurs { get; set; }
        public TimeOnly EndTimeCurs { get; set; }
        [Required]
        public int ZiuaSaptamaniiSeminar { get; set; }

        public TimeOnly StartTimeSeminar { get; set; }
        public TimeOnly EndTimeSeminar { get; set; }

    }
}
