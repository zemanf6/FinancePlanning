using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Presentation.Areas.Forecasting.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlanning.Presentation.Areas.Forecasting.Controllers
{
    [Area("Forecasting")]
    public class ForecastController : Controller
    {
        private readonly IMonteCarloSimulator _simulator;
        private readonly IMapper _mapper;

        public ForecastController(IMonteCarloSimulator simulator, IMapper mapper)
        {
            _simulator = simulator;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new InvestmentPredictionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(InvestmentPredictionViewModel model)
        {
            Console.WriteLine();
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<InvestmentPredictionDto>(model);
            dto.ExpectedReturn = model.CalculatedExpectedReturn;
            var result = _simulator.Simulate(dto);

            model.Result = result;
            return View(model);
        }
    }
}
