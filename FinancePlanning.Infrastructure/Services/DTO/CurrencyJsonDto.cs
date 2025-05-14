using System.Text.Json.Serialization;

namespace FinancePlanning.Infrastructure.Services.DTO
{
    public class CurrencyJsonDto
    {
        [JsonPropertyName("symbol")]
        public required string Symbol { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("decimal_digits")]
        public int DecimalDigits { get; set; }
    }
}
