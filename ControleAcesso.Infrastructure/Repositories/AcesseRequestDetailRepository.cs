using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Infrastructure.Data;
using ControleAcesso.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ControleAcesso.Infrastructure.Repositories
{
    public class AcesseRequestDetailRepository : GenericRepository<AcesseRequestDetail>, IAcesseRequestDetailRepository
    {
        public AcesseRequestDetailRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<AcesseRequestDetail> AddWithApprovalAsync(AcesseRequestDetail acesseRequestDetail, PriorApproval priorApproval)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Adiciona a AcesseRequestDetail
                    acesseRequestDetail = await base.CreateAsync(acesseRequestDetail);


                    // Adiciona a PriorApproval
                    _context.Set<PriorApproval>().Add(priorApproval);
                    await _context.SaveChangesAsync();

                    // Comita a transação
                    await transaction.CommitAsync();

                    return acesseRequestDetail;
                }
                catch (Exception)
                {
                    // Reverte a transação em caso de erro
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        public async Task<IEnumerable<AcesseRequestDetail>> GetPendingEspecialistAsync(int employee)
        {
            return await _context.Set<AcesseRequestDetail>()
                .Include(ard => ard.AcesseRequest)
                    .ThenInclude(ar => ar.GroupAd)
                        .ThenInclude(ga => ga.GroupApprovals)
                .Where(ard => ard.StatusRequestId == 2 &&
                              ard.AcesseRequest.HasPriorApproval == true &&
                              ard.AcesseRequest.GroupAd.GroupApprovals.Any(ga => ga.EmployeeId == employee))
                .ToListAsync();
        }
    }
}
