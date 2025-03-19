using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;



namespace HabitsApp.Domain.Abstractions.Repositories;
public interface IGenericRepository<T> where T:Entity
{
    Task<List<T>> GetAllAsync();
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    public IQueryable<T> GetAll();
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
}

