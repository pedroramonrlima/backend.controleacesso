using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ControleAcesso.Infrastructure.Repositories
{
    public class AcesseRequestRepository : GenericRepository<AcesseRequest>, IAcesseRequestRepository
    {
        public AcesseRequestRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<AcesseRequest> AddWithDetailsAsync(AcesseRequest acesseRequest, AcesseRequestDetail acesseRequestDetail)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Adiciona a AcesseRequest
                    acesseRequest = await base.CreateAsync(acesseRequest,NavigationLevel.SecondLevel);

                    acesseRequestDetail.AcesseRequestId = acesseRequest.Id;
                    acesseRequestDetail.ManagerApprovalId = (int)acesseRequest.Employee.Department.ManagerId;

                    // Adiciona a AcesseRequestDetail
                    _context.Set<AcesseRequestDetail>().Add(acesseRequestDetail);
                    await _context.SaveChangesAsync();

                    // Comita a transação
                    await transaction.CommitAsync();

                    return acesseRequest;
                }
                catch (Exception)
                {
                    // Reverte a transação em caso de erro
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<AcesseRequest?> GetAcesseRequestAsync(Expression<Func<AcesseRequest, bool>> predicate)
        {
            return await _context.AcesseRequests
                         .Include(t => t.Employee)
                         .ThenInclude(e => e.Department)
                         .ThenInclude(d => d.Manager)
                         .ThenInclude(e => e.Employee)
                         .FirstOrDefaultAsync(predicate);
        }
        public async Task<IEnumerable<AcesseRequest>> GetAllAcesseRequestAsync()
        {
            return await _context.AcesseRequests
                         .Include(t => t.Employee)
                         .ThenInclude(e => e.Department)
                         .ThenInclude(d => d.Manager)
                         .ThenInclude(e => e.Employee)
                         .ToListAsync();
        }
    }
}
