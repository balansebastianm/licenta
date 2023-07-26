using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicWeb.Models
{
    public class Adeverinta
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey(nameof(Doctor))] 
        public int IdDoctor { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        [StringLength(450)]
        public string IdStudent { get; set; }
        [Required]
        public string SemnaturaDoctor { get; set; }
        [Required]
        public string SemnaturaUniversitate { get; set; }
        [Required]
        public int CurrentStatus { get; set; } //0-semnatura nu coincide (doctor), 1-semnatura doctor OK, 2-semnatura admin, 3- abprob. stud.
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string PathToAdeverinta { get; set; }
        [Required]
        public DateTime DataConsultatie { get; set; }
    }
}
