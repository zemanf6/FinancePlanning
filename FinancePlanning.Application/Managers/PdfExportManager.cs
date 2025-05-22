using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace FinancePlanning.Application.Managers
{
    public class PdfExportManager : IPdfExportManager
    {
        public byte[] GeneratePdf(InvestmentExportDto dto)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Row(row =>
                    {
                        row.RelativeColumn().Text("Investment Simulation Report").FontSize(20).Bold();
                        row.ConstantColumn(100).AlignRight().Text(date).FontSize(10);
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(25);

                        col.Item().Text("Input Parameters").FontSize(14).Bold();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            table.Cell().Text("Initial Investment").SemiBold();
                            table.Cell().Text($"{dto.Principal} {dto.SelectedCurrency}");

                            table.Cell().Text("Monthly Contribution").SemiBold();
                            table.Cell().Text($"{dto.MonthlyContribution} {dto.SelectedCurrency}");

                            table.Cell().Text("Investment Horizon (Years)").SemiBold();
                            table.Cell().Text($"{dto.Years}");

                            table.Cell().Text("Expected Return").SemiBold();
                            table.Cell().Text($"{dto.ExpectedReturn:F2}%");

                            table.Cell().Text("Volatility (Std. Dev.)").SemiBold();
                            table.Cell().Text($"{dto.StandardDeviation:F2}%");

                            table.Cell().Text("Inflation Rate").SemiBold();
                            table.Cell().Text($"{dto.Result?.InflationRate:F2}%");
                        });

                        col.Item().Text("Portfolio Allocation").FontSize(14).Bold();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150);
                                columns.ConstantColumn(70);
                                columns.ConstantColumn(70);
                                columns.ConstantColumn(90);
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Asset").SemiBold();
                                header.Cell().Text("Return (%)").SemiBold();
                                header.Cell().Text("Weight (%)").SemiBold();
                                header.Cell().Text("Volatility (%)").SemiBold();
                                header.Cell().Text("Level").SemiBold();
                            });

                            foreach (var item in dto.PortfolioItems)
                            {
                                table.Cell().Text(item.AssetName ?? "-");
                                table.Cell().Text($"{item.ExpectedReturn:F2}");
                                table.Cell().Text($"{item.Weight:F2}");
                                table.Cell().Text($"{item.StandardDeviation:F2}");
                                table.Cell().Text(item.SelectedVolatilityLevel);
                            }
                        });

                        if (dto.Result != null)
                        {
                            col.Item().Text("Simulation Results").FontSize(14).Bold();
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();
                                    c.RelativeColumn();
                                });

                                void Add(string label, decimal? val)
                                {
                                    table.Cell().Text(label).SemiBold();
                                    table.Cell().Text(val.HasValue ? $"{val:F2} {dto.SelectedCurrency}" : "-");
                                }

                                Add("5th Percentile", dto.Result.Percentile5);
                                Add("10th Percentile", dto.Result.Percentile10);
                                Add("25th Percentile", dto.Result.Percentile25);
                                Add("Median (50th)", dto.Result.Percentile50);
                                Add("75th Percentile", dto.Result.Percentile75);
                                Add("90th Percentile", dto.Result.Percentile90);
                                Add("95th Percentile", dto.Result.Percentile95);
                                Add("Average", dto.Result.AverageFinalValue);

                                if (dto.Result.TargetReachedProbability.HasValue)
                                {
                                    table.Cell().Text("Target Reached Probability").SemiBold();
                                    table.Cell().Text($"{dto.Result.TargetReachedProbability.Value:F1}%");
                                }
                            });

                            col.Item().Text("Results Adjusted for Inflation").FontSize(14).Bold();
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();
                                    c.RelativeColumn();
                                });

                                void AddReal(string label, decimal? val)
                                {
                                    table.Cell().Text(label).SemiBold();
                                    table.Cell().Text(val.HasValue ? $"{val:F2} {dto.SelectedCurrency}" : "-");
                                }

                                AddReal("10th Percentile (Real)", dto.Result.RealPercentile10);
                                AddReal("Median (50th) (Real)", dto.Result.RealPercentile50);
                                AddReal("90th Percentile (Real)", dto.Result.RealPercentile90);
                                AddReal("Average (Real)", dto.Result.RealAverageFinalValue);
                            });

                            if (dto.Result.ReachedMaxValue)
                            {
                                col.Item().PaddingTop(10).Text("⚠ Some simulations reached the system limit. Results may be capped.").FontColor(Colors.Orange.Darken2);
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text($"Generated by FinancePlanner – {date}").FontSize(10);
                });
            }).GeneratePdf();
        }
    }
}
