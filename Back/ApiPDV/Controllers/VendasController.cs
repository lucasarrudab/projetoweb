using Microsoft.AspNetCore.Mvc;

namespace ApiPDV.Controllers
{
    public class VendasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
