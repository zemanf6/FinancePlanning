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
    public class CompoundInterestController : Controller
    {
        private readonly IInterestCalculatorManager<CompoundInterestDto> _calculator;
        private readonly ICompoundInterestStorageManager _storageManager;
        private readonly IMapper _mapper;

        public CompoundInterestController(
            IInterestCalculatorManager<CompoundInterestDto> calculator,
            ICompoundInterestStorageManager storageManager,
            IMapper mapper)
        {
            _calculator = calculator;
            _storageManager = storageManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = LoadFromTempData() ?? new CompoundInterestViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CompoundInterestViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var dto = _mapper.Map<CompoundInterestDto>(viewModel);
            var result = _calculator.Calculate(dto);
            var updatedViewModel = _mapper.Map<CompoundInterestViewModel>(result);

            SaveToTempData(updatedViewModel);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCalculation(CompoundInterestViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var dto = _mapper.Map<CompoundInterestDto>(viewModel);
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

            var viewModel = _mapper.Map<CompoundInterestViewModel>(dto);
            SaveToTempData(viewModel);

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
        private void SaveToTempData(CompoundInterestViewModel model)
        {
            TempData["Result"] = JsonSerializer.Serialize(model);
        }
        private CompoundInterestViewModel? LoadFromTempData()
        {
            if (TempData["Result"] is string json)
                return JsonSerializer.Deserialize<CompoundInterestViewModel>(json);
            return null;
        }
    }
}
