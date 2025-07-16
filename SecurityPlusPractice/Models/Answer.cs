namespace SecurityPlusPractice.Models
{
	public class Answer
	{
		public int ID { get; set; }
		public string? CorrectAnswer { get; set; } // Handles single or multi-answer, e.g., "A,C"
	}
}
