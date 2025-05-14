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
    public class SimpleInterestController : Controller
    {
        private readonly IInterestCalculatorManager<SimpleInterestDto> _calculator;
        private readonly IMapper _mapper;
        private readonly ISimpleInterestStorageManager _storageManager;

        public SimpleInterestController(
            IInterestCalculatorManager<SimpleInterestDto> calculator,
            IMapper mapper,
            ISimpleInterestStorageManager storageManager)
        {
            _calculator = calculator;
            _mapper = mapper;
            _storageManager = storageManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (TempData["Result"] is string json)
            {
                var viewModel = JsonSerializer.Deserialize<SimpleInterestViewModel>(json);
                if (viewModel?.ChartData != null)
                {
                    viewModel.ChartData = viewModel.ChartData
                        .OfType<JsonElement>()
                        .Select(e => JsonSerializer.Deserialize<InterestChartStep>(e.GetRawText()))
                        .Cast<object>()
                        .ToList();
                }
                return View(viewModel);
            }

            return View(new SimpleInterestViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SimpleInterestViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var dto = _mapper.Map<SimpleInterestDto>(viewModel);
            var result = _calculator.Calculate(dto);
            var updatedViewModel = _mapper.Map<SimpleInterestViewModel>(result);

            updatedViewModel.ChartData = result.ChartData
                .Cast<object>()
                .ToList();

            TempData["Result"] = JsonSerializer.Serialize(updatedViewModel);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCalculation(SimpleInterestViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var dto = _mapper.Map<SimpleInterestDto>(viewModel);
            await _storageManager.SaveCalculationAsync(dto, User);

            TempData["Success"] = "Calculation was saved successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Saved()
        {
            var saved = await _storageManager.GetSavedCalculationsAsync(User);
            return View(saved);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoadCalculation(int id)
        {
            var dto = await _storageManager.LoadDtoByIdAsync(id, User);
            if (dto == null)
                return RedirectToAction("Saved");

            var viewModel = _mapper.Map<SimpleInterestViewModel>(dto);

            viewModel.ChartData = dto.ChartData
                .Cast<object>()
                .ToList();

            TempData["Result"] = JsonSerializer.Serialize(viewModel);
            TempData["TriggerAutoCalculate"] = "true";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCalculation(int id)
        {
            await _storageManager.DeleteByIdAsync(id, User);
            TempData["Success"] = "Calculation was deleted.";
            return RedirectToAction("Saved");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllCalculations()
        {
            await _storageManager.DeleteAllAsync(User);
            TempData["Success"] = "All calculations were deleted.";
            return RedirectToAction("Saved");
        }
    }
}
