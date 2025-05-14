namespace FinancePlanning.Infrastructure.Interfaces
{
    public interface ICurrencyFormatter
    {
        string Format(decimal amount, string currencyCode);
    }
}
