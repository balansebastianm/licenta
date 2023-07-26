using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LicWeb.Models
{
    public class Profesor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("User")]
        [StringLength(450)]
        public string ProfesorUserId { get; set; }
        [Required]
        public string Nomenclatura { get; set; }
    }
}
