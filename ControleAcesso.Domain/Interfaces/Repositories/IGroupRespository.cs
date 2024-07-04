using ControleAcesso.Domain.Entities;
using System.Linq.Expressions;

namespace ControleAcesso.Domain.Interfaces.Repositories
{
    public interface IGroupRespository : IGenericRepository <GroupAd>       
    {
        Task<IEnumerable<GroupAd>> GetAllGroupAdAsync();

        Task<GroupAd?> GetEmployeeAsync(Expression<Func<GroupAd, bool>> predicate);
    }
}
