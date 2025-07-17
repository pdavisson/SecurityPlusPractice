using Microsoft.AspNetCore.Mvc;
namespace SecurityPlusPractice.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}