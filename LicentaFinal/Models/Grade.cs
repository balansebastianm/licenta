using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LicWeb.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Enrollment")]
        public int EnrollmentId { get; set; }
        public float? GradeCurs { get; set; }
        public float? GradeSeminar { get; set; }
    }
}
