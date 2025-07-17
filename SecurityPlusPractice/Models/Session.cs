namespace SecurityPlusPractice.Models
{
	public class Session
	{
		public int SessionID { get; set; }
		public required string UserName { get; set; }
		public DateTime StartedAt { get; set; }
		public DateTime? FinishedAt { get; set; }
		public int TotalQuestions { get; set; }
		public int? CorrectAnswers { get; set; }
		public decimal? ScorePercent { get; set; }
	}
}
