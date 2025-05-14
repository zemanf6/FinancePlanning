namespace FinancePlanning.Infrastructure.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        void Remove(T entity);
        Task SaveChangesAsync();
    }
}
