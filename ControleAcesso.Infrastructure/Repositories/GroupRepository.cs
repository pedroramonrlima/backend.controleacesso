using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ControleAcesso.Infrastructure.Repositories
{
    public class GroupRepository : GenericRepository<GroupAd>, IGroupRespository
    {
        public GroupRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GroupAd>> GetAllGroupAdAsync()
        {
            //Pesquisa personalizada para trazer algumas coisas a mais na consulta do GroupAd
            var groupAds = await _context.GroupAds.Include(t => t.GroupApprovals).ToListAsync();
            
            return groupAds;

        }

        public Task<GroupAd?> GetEmployeeAsync(Expression<Func<GroupAd, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
