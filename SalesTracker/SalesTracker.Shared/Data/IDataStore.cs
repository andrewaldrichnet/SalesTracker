using System.Linq.Expressions;

namespace SalesTracker.Shared.Data;

public interface IDataStore<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<int> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<List<T>> QueryAsync(Expression<Func<T, bool>> predicate);
}
