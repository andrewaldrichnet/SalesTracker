using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SalesTracker.Shared.Data;

namespace SalesTracker.Data.Sqlite;

public class SqliteDataStore<T> : IDataStore<T> where T : class
{
    private readonly SalesTrackerDbContext _context;

    public SqliteDataStore(SalesTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<int> AddAsync(T entity)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();

        var idProperty = typeof(T).GetProperty("ItemID") ?? typeof(T).GetProperty("OrderID") ?? typeof(T).GetProperty("ImageID");
        if (idProperty != null)
        {
            return (int)(idProperty.GetValue(entity) ?? 0);
        }

        return 0;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<T>> QueryAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }
}
