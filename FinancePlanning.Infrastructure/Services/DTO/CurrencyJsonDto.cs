using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
