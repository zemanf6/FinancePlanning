using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Infrastructure.Interfaces
{
    public interface ICurrencyFormatter
    {
        string Format(decimal amount, string currencyCode);
    }
}
