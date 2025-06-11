using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektCRUD20510.Models
{
    public class Testy_Fiszki
    {
        [Required]
        [ForeignKey("Testy")] // Specify the navigation property
        public int Id_Testu { get; set; }
        [Required]
        [ForeignKey("Fiszki")] // Specify the navigation property
        public int Id_Fiszki { get; set; }
        public Testy? Test { get; set; } // Navigation property
        public Fiszki? Fiszka { get; set; } // Navigation property
    }
}