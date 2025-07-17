using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityPlusPractice.Data;

namespace SecurityPlusPractice.Controllers
{
	public class HistoryController : Controller
	{
		private readonly AppDbContext _context;

		public HistoryController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var user = Environment.UserName;

			var sessions = _context.Sessions
				.Where(s => s.UserName == user)
				.OrderByDescending(s => s.StartedAt)
				.ToList();

			return View(sessions);
		}
	}
}
