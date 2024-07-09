using ControleAcesso.Domain.Enumerations;
using System.Linq.Expressions;
namespace ControleAcesso.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T>
    {
        //Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(NavigationLevel navigationLevel = NavigationLevel.None);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, NavigationLevel navigationLevel = NavigationLevel.None);
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> include);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate, NavigationLevel navigationLevel = NavigationLevel.None);
        T Create(T entity);
        Task<T> CreateAsync(T entity, NavigationLevel navigationLevel = NavigationLevel.None);
        T Update(T entity);
        Task<T> UpdateAsync(T entity, NavigationLevel navigationLevel = NavigationLevel.None);
        T Delete(T entity);
    }
}
