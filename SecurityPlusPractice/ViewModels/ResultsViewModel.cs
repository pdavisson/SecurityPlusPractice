namespace SecurityPlusPractice.ViewModels
{
	public class ResultViewModel
	{
		public int SessionID { get; set; }
		public required string UserName { get; set; }
		public DateTime SessionDate { get; set; }
		public int Total { get; set; }
		public int Score { get; set; }
		public double Percentage => Total == 0 ? 0 : (double)Score / Total * 100;
	}
}
