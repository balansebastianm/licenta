using System.ComponentModel.DataAnnotations;

namespace LicWeb.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Cod Inregistrare")]
        [Required(ErrorMessage = "Cod Necesar")]
        public string CodInregistrare { get; set; }
        [Display(Name = "Adresa Email")]
        [Required(ErrorMessage = "Email necesar")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campul trebuie completat")]
        [DataType(DataType.Password)]
        public string Parola { get; set; }
        [Required(ErrorMessage = "Parolele nu corespund")]
        [DataType(DataType.Password)]
        [Compare(nameof(Parola), ErrorMessage = "Parolele nu corespund")]
        public string RepetaParola { get; set; }

        [Display(Name = "Nume")]
        [Required(ErrorMessage = "Nume necesar")]
        public string Nume { get; set; }
        [Display(Name = "Prenume")]
        [Required(ErrorMessage = "Prenume necesat")]
        public string Prenume { get; set; }
        [Display(Name = "Parafa")]
        [Required(ErrorMessage = "Parafa Necesara")]
        public string Parafa { get; set; }
        [Display(Name = "Localitate")]
        [Required(ErrorMessage = "Localitate necesara")]
        public string Localitate { get; set; }
        [Display(Name = "Judet")]
        [Required(ErrorMessage = "Judet necesar")]
        public string Judet { get; set; }
        [Display(Name = "Nomenclatura")]
        [Required(ErrorMessage = "Nomenclatura necesara")]
        public string Nomenclatura { get; set; }
        public int IdSpecializare { get; set; }
        [Display(Name = "Nr. Matricol")]
        [Required(ErrorMessage = "Nr. Matricol necesar")]
        public int NrMatricol { get; set; }
        [Display(Name = "An Studii")]
        [Required(ErrorMessage = "An Studii necesar")]
        public int AnStudii { get; set; }
        [Display(Name = "Modul Studii")]
        [Required(ErrorMessage = "Modul Studii necesar")]
        public int ModulStudii { get; set; }
    }
}
