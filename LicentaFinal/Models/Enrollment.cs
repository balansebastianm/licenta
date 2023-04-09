using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicWeb.Models
{
    public class Enrollment
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
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public bool? Passed { get; set; }
    }
}
