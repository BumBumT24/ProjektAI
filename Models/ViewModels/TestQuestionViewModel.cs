namespace ProjektCRUD20510.Models.ViewModels
{
    public class TestQuestionViewModel
    {
        public int TestId { get; set; }
        public Fiszki Fiszka { get; set; }
        public string Direction { get; set; }
        public string UserAnswer { get; set; }
        public int CurrentIndex { get; set; }
        public int TotalCount { get; set; }
    }
}