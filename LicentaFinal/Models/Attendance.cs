using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LicWeb.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Enrollment")]
        public int EnrollmentId { get; set; }
        [Required]
        public DateTime? AttendanceDateTime { get; set; }
        [Required]
        public bool? Present { get; set; }

    }
}
