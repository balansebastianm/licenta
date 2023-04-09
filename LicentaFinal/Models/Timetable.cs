using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicWeb.Models
{
    public class Timetable
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        [Required]
        [ForeignKey("Seminar")]
        public int SeminarId { get; set; }
        [Required]
        public int DayOfWeek { get; set; }
        [Required]
        public TimeOnly start_time { get; set; }
        [Required]
        public TimeOnly end_time { get; set; }
    }
}
