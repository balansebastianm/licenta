﻿using System.ComponentModel.DataAnnotations;
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
        public string EmailStudent { get; set; }
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
    }
}
