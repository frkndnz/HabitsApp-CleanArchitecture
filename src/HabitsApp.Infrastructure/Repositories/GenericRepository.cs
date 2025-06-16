using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Infrastructure.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : Entity
{
    private readonly DbContext _dbContext;

    public GenericRepository(DbContext context)
    {
        _dbContext = context;
    }

    public void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().AnyAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return predicate is null ?
            await _dbContext.Set<T>().CountAsync()
            : await _dbContext.Set<T>().CountAsync(predicate);
    }

    public void Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public IQueryable<T> GetAll()
    {
        return _dbContext.Set<T>().AsQueryable();
    }

    public async Task<List<T>> GetAllAsync()
    {
       return await _dbContext.Set<T>().ToListAsync();  
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }
}
