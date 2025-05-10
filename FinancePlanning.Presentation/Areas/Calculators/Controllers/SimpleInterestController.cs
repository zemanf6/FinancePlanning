using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Presentation.Areas.Calculators.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

            return View(updatedViewModel);
        }
    }
}
