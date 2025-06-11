using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjektCRUD20510.Models
{
    public class Fiszki
    {
        public int Id { get; set; }
        [Required]
        public int Id_Uzytkownika { get; set; } // Matches database column
        [Required]
        public string Nazwa_PL { get; set; }
        [Required]
        public string Nazwa_ENGLISH { get; set; }
        [ForeignKey("Id_Uzytkownika")]
        public Class? Uzytkownik { get; set; } // Navigation property
    }
}
