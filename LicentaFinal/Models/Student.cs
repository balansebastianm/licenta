using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LicWeb.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public int? NumarMatricol { get; set; }
        public int? AnDeStudii { get; set; }
        public string? Specializare { get; set; }
        [Required]
        [ForeignKey("User")]
        [StringLength(450)]
        public string StudentUserId { get; set; }
    }
}
