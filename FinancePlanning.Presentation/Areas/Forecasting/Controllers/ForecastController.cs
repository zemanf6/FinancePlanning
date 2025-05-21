using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Presentation.Areas.Forecasting.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FinancePlanning.Presentation.Areas.Forecasting.Controllers
{
    [Area("Forecasting")]
    public class ForecastController : Controller
    {
        private readonly IMonteCarloSimulator _simulator;
        private readonly IMapper _mapper;
        private readonly IPdfExportManager _pdfExportManager;
        private readonly IXmlExportManager _xmlExportManager;

        public ForecastController(IMonteCarloSimulator simulator, IMapper mapper, IPdfExportManager pdfExportManager, IXmlExportManager xmlExportManager)
        {
            _simulator = simulator;
            _mapper = mapper;
            _pdfExportManager = pdfExportManager;
            _xmlExportManager = xmlExportManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new InvestmentPredictionViewModel
            {
                PortfolioItems = new List<PortfolioItemViewModel>
                {
                    new PortfolioItemViewModel()
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(InvestmentPredictionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<InvestmentPredictionDto>(model);
            dto.ExpectedReturn = model.CalculatedExpectedReturn;
            var result = _simulator.Simulate(dto);

            model.Result = result;

            var resultJson = JsonSerializer.Serialize(result);
            HttpContext.Session.SetString("LastSimulationResult", resultJson);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExportPdf(InvestmentPredictionViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            var resultJson = HttpContext.Session.GetString("LastSimulationResult");
            if (string.IsNullOrWhiteSpace(resultJson))
                return RedirectToAction(nameof(Index));

            var result = JsonSerializer.Deserialize<SimulationResultDto>(resultJson);

            var exportDto = _mapper.Map<InvestmentExportDto>(model);
            exportDto.ExpectedReturn = model.CalculatedExpectedReturn;
            exportDto.StandardDeviation = model.CalculatedStandardDeviation;
            exportDto.Result = result;

            var pdf = _pdfExportManager.GeneratePdf(exportDto);
            return File(pdf, "application/pdf", "InvestmentReport.pdf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExportXml(InvestmentPredictionViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            var resultJson = HttpContext.Session.GetString("LastSimulationResult");
            if (string.IsNullOrWhiteSpace(resultJson))
                return RedirectToAction(nameof(Index));

            var result = JsonSerializer.Deserialize<SimulationResultDto>(resultJson);

            var exportDto = _mapper.Map<InvestmentExportDto>(model);
            exportDto.ExpectedReturn = model.CalculatedExpectedReturn;
            exportDto.StandardDeviation = model.CalculatedStandardDeviation;
            exportDto.Result = result;

            var xml = _xmlExportManager.GenerateXml(exportDto);
            return File(xml, "application/xml", "InvestmentReport.xml");
        }
    }
}
