using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
