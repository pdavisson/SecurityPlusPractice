using SecurityPlusPractice.Models;

namespace SecurityPlusPractice.ViewModels
{
	public class QuestionViewModel
	{
		public int SessionID { get; set; }
		public int QuestionID { get; set; }
		public string? QuestionText { get; set; }

		// Optional: these may still be used depending on how your view is structured
		public string? ChoiceA { get; set; }
		public string? ChoiceB { get; set; }
		public string? ChoiceC { get; set; }
		public string? ChoiceD { get; set; }

		// ? Add this:
		public Dictionary<string, string> Choices { get; set; } = new();

		public string? UserAnswer { get; set; }
		public bool IsCorrect { get; set; }
		public string? ExplanationText { get; set; }

		public int CurrentIndex { get; set; }
		public int TotalQuestions { get; set; }
	}
}

