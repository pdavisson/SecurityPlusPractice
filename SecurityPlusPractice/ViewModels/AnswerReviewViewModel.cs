namespace SecurityPlusPractice.ViewModels
{
    public class AnswerReviewViewModel
    {
        public string? QuestionText { get; set; }
        public Dictionary<string, string>? Choices { get; set; }
        public string? SelectedAnswer { get; set; }
        public string? CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }
        public int NextIndex { get; set; }
    }
}
