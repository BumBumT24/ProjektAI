using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ProjektCRUD20510.Models
{
    public class Kategorie
    {
        public int Id { get; set; }
        [Required]
        public string Nazwa { get; set; }
    }

}

