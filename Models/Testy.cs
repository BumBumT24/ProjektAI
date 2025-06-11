using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektCRUD20510.Models
{
    public class Testy
    {
        public int Id { get; set; }
        [Required]
        public int Id_Kategorii { get; set; }
        [Required]
        public string NazwaTestu { get; set; }
        [ForeignKey("Id_Kategorii")]
        public Kategorie? Kategoria { get; set; }
        public List<Testy_Fiszki> Testy_Fiszki { get; set; } = new List<Testy_Fiszki>(); // Navigation property
    }
}