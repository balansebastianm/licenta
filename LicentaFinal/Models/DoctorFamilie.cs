using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LicWeb.Models
{
    public class DoctorFamilie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? CheiePublica { get; set; }
        [Required]
        [ForeignKey("User")]
        [StringLength(450)]
        public string DoctorUserId { get; set; }
        
    }
}
