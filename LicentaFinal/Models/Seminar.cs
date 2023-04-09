using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicWeb.Models
{
    public class Seminar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Curs")]
        public int IdCursAsociat { get; set; }
        [Required]
        public int DayOfWeek { get; set; }
        [Required]
        public DateTime TimeOfDay { get; set; }
        [Required]
        [ForeignKey("Profesor")]
        public int ProfesorSeminarId { get; set; }
    }
}
