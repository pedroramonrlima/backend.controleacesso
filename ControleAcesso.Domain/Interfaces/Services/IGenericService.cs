using ControleAcesso.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleAcesso.Domain.Interfaces.Services
{
    public interface IGenericService<T>
    {
        Task<IEnumerable<T>> GetAllAsync(NavigationLevel navigationLevel = NavigationLevel.None);

        Task<T?> GetByIdAsync(int id, NavigationLevel navigationLevel = NavigationLevel.None);
        T Update(T entity);
        Task<T> UpdateAsync(T entity);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        Task<T> Delete(int id);
    }
}
