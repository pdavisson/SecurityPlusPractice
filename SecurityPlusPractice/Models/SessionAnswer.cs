namespace SecurityPlusPractice.Models
{
	public class SessionAnswer
	{
		public int SessionAnswerID { get; set; }
		public int SessionID { get; set; }
		public required string UserName { get; set; }
		public int QuestionID { get; set; }
		public string? SelectedAnswer { get; set; } // e.g., "A,B"
		public bool IsCorrect { get; set; }
		public DateTime AnsweredAt { get; set; }
	}
}
