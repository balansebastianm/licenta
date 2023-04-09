using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicWeb.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CourseName { get; set; }
        [Required]
        public int DayOfWeek { get; set; }
        [Required]
        public DateTime TimeOfDay { get; set; }
        [Required]
        [ForeignKey("Profesor")]
        public int ProfesorCursId { get; set; }

    }
}
