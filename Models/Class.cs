using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace ProjektCRUD20510.Models
{
    public class Class
    {
        public int Id { get; set; }
        [Required]
        public string Nazwa { get; set; }
        [Required]
        public string Haslo { get; set; }
        [Required]
        public bool Role { get; set; }
    }
}
