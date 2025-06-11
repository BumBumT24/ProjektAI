namespace ProjektCRUD20510.Models.ViewModels
{
    public class TestResultViewModel
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int CorrectCount { get; set; }
        public int TotalCount { get; set; }
        public string Direction { get; set; } // Add Direction property
        public List<(Fiszki Fiszka, string UserAnswer, bool IsCorrect)> Answers { get; set; }
    }
}