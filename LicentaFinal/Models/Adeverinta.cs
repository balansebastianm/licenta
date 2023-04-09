using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicWeb.Models
{
    public class Adeverinta
    {
        [Key]
        public int Id { get; set; }
        public string EncryptedData { get; set; }
        public string EmailStudent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PathToAdeverinta { get; set; }
    }
}
