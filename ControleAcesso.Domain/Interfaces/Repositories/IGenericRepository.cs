using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ControleAcesso.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        T Create(T entity);
        Task<T> CreateAsync(T entity);
        T Update(T entity);
        Task<T> UpdateAsync(T entity);
        T Delete(T entity);
    }
}
