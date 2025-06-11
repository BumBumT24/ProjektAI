using System.ComponentModel.DataAnnotations;

namespace ProjektCRUD20510.Models.ViewModels
{
    public class TestyEditViewModel
    {
        public int Id { get; set; }
        [Required]
        public int Id_Kategorii { get; set; }
        [Required]
        public string NazwaTestu { get; set; }
        public List<int> SelectedFiszkiIds { get; set; } = new List<int>(); // List of selected Fiszki IDs
    }
}