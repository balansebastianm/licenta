using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LicWeb.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        [StringLength(450)]
        public string StudentUserId { get; set; }
        [Required]
        public int NumarMatricol { get; set; }
        [ForeignKey("Specializare")]
        [StringLength(450)]
        public int IdSpecializare { get; set; }
        
        public int AnDeStudii { get; set; }
        public int ModulStudii { get; set; }

    }
}
