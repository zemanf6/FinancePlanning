using FinancePlanning.Infrastructure.Interfaces;
using FinancePlanning.Infrastructure.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinancePlanning.Infrastructure.Services
{
    public class CurrencyFormatter : ICurrencyFormatter
    {
        private readonly Dictionary<string, CurrencyJsonDto> _currencies;

        public CurrencyFormatter(Dictionary<string, CurrencyJsonDto> currencies)
        {
            _currencies = currencies ?? throw new ArgumentNullException(nameof(currencies));
        }

        public string Format(decimal amount, string currencyCode)
        {
            if (!_currencies.TryGetValue(currencyCode, out var currency))
                currency = _currencies.GetValueOrDefault("USD");

            if (currency is null)
                return "";

            var rounded = Math.Round(amount, currency.DecimalDigits);
            return $"{currency.Symbol} {rounded.ToString($"N{currency.DecimalDigits}")}";
        }
    }

}
