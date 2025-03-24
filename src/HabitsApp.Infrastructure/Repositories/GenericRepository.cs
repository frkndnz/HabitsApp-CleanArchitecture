using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;
using HabitsApp.Domain.Abstractions.Repositories;

namespace HabitsApp.Infrastructure.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : Entity
{
    public void Add(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public void Delete(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public IQueryable<T> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public void Update(T entity)
    {
        throw new NotImplementedException();
    }
}
