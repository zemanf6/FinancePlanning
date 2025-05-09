using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlanning.Presentation.Areas.Calculators.Controllers
{
    [Area("Calculators")]
    [AllowAnonymous]
    public class SimpleInterestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
