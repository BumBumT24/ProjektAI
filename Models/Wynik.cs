using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektCRUD20510.Models
{
    public class Wynik
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Id_Testu { get; set; }

        [Required]
        public int Id_Uzytkownika { get; set; }

        [Required]
        public int Id_Kategorii { get; set; }

        [Required]
        public string wynik { get; set; }

        [ForeignKey("Id_Testu")]
        public virtual Testy? Test { get; set; }

        [ForeignKey("Id_Uzytkownika")]
        public virtual Class? Uzytkownik { get; set; }

        [ForeignKey("Id_Kategorii")]
        public virtual Kategorie? Kategoria { get; set; }
    }
}