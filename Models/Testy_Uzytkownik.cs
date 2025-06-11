using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ProjektCRUD20510.Models
{

    public class Testy_Uzytkownik
    {
        [Key]
        [Column(Order = 0)]
        public int Id_Testu { get; set; }

        [Key]
        [Column(Order = 1)]
        public int Id_Uzytkownika { get; set; }

        [ForeignKey("Id_Testu")]
        public virtual Testy? Test { get; set; }

        [ForeignKey("Id_Uzytkownika")]
        public virtual Class? Uzytkownik { get; set; }
    }
}
