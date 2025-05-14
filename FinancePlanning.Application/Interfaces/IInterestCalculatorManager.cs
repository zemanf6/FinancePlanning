namespace FinancePlanning.Application.Interfaces
{
    public interface IInterestCalculatorManager<TDto>
    {
        TDto Calculate(TDto dto);
    }
}
