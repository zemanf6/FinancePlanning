namespace FinancePlanning.Infrastructure.Interfaces
{
    public interface IUserScopedRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetByUserIdAsync(string userId);
        Task DeleteAllByUserIdAsync(string userId);
    }
}
