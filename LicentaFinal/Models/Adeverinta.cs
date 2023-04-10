using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicWeb.Models
{
    public class Adeverinta
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string EncryptedData { get; set; }
        [Required]
        public string IdStudent { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string PathToAdeverinta { get; set; }
        [Required]
        [ForeignKey(nameof(DoctorFamilie))]
        public int DoctorId { get; set; }
        public int Passed { get; set; }
        public int CurrentStatus { get; set; } //0-trimisa, 1-aprobata de administrator, 2-aprobata de student, -1-respinsa de prof, -2-respinsa de student
    }
}
