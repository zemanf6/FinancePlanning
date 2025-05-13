using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Presentation.Areas.Calculators.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FinancePlanning.Presentation.Areas.Calculators.Controllers
{
    [Area("Calculators")]
    public class SimpleInterestController : Controller
    {
        private readonly ISimpleInterestCalculatorManager _calculator;
        private readonly IMapper _mapper;
        private readonly ISimpleInterestStorageManager _storageManager;

        public SimpleInterestController(ISimpleInterestCalculatorManager calculator, IMapper mapper, ISimpleInterestStorageManager storageManager)
        {
            _calculator = calculator;
            _mapper = mapper;
            _storageManager = storageManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
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

            var dto = _mapper.Map<SimpleInterestDto>(viewModel);
            var result = _calculator.Calculate(dto);
            var updatedViewModel = _mapper.Map<SimpleInterestViewModel>(result);

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
