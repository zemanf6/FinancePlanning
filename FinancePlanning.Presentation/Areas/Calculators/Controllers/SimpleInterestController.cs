using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Presentation.Areas.Calculators.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FinancePlanning.Presentation.Areas.Calculators.Controllers
{
    [Area("Calculators")]
    [AllowAnonymous]
    public class SimpleInterestController : Controller
    {
        private readonly ISimpleInterestCalculatorManager calculator;
        private readonly IMapper mapper;

        public SimpleInterestController(ISimpleInterestCalculatorManager calculator, IMapper mapper)
        {
            this.calculator = calculator;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Pokud v TempData existují výsledky, zobrazíme je
            if (TempData.ContainsKey("Result"))
            {
                var json = TempData["Result"] as string;
                if (json != null)
                {
                    var resultViewModel = JsonSerializer.Deserialize<SimpleInterestViewModel>(json);
                    return View(resultViewModel);
                }
            }

            return View(new SimpleInterestViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SimpleInterestViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var dto = mapper.Map<SimpleInterestDto>(viewModel);
            var result = calculator.Calculate(dto);
            var updatedViewModel = mapper.Map<SimpleInterestViewModel>(result);

            TempData["Result"] = JsonSerializer.Serialize(updatedViewModel);

            return RedirectToAction("Index");
        }
    }
}
