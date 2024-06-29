using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleAcesso.Domain.Interfaces.Services
{
    public interface IGenericService<T>
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);
        T Update(T entity);
        Task<T> UpdateAsync(T entity);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        Task<T> Delete(int id);
    }
}
