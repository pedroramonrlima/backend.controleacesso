using ControleAcesso.Domain.Entities;
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
                    // Verifique o estado atual da entidade para garantir que está sendo rastreada corretamente
                    var trackedEntity = await _context.Set<AcesseRequestDetail>()
                        .Include(ard => ard.Status)
                        .FirstOrDefaultAsync(ard => ard.Id == acesseRequestDetail.Id);

                    if (trackedEntity != null)
                    {
                        // Atualize o estado da entidade rastreada
                        trackedEntity.StatusRequestId = acesseRequestDetail.StatusRequestId;
                        trackedEntity.Status = acesseRequestDetail.Status;

                        _context.Entry(trackedEntity).State = EntityState.Modified;
                    }
                    else
                    {
                        // Caso a entidade não esteja sendo rastreada, adicione-a como nova
                        _context.Set<AcesseRequestDetail>().Update(acesseRequestDetail);
                    }

                    // Adiciona a PriorApproval
                    _context.Set<PriorApproval>().Add(priorApproval);
                    await _context.SaveChangesAsync();

                    // Comita a transação
                    await transaction.CommitAsync();

                    // Retorne a entidade com a navegação `Status` incluída
                    return await _context.Set<AcesseRequestDetail>()
                        .Include(ard => ard.Status)
                        .Include(ard => ard.ManagerApproval)
                            .ThenInclude(ard => ard.Employee)
                        .Include(ard => ard.RequesterEmployee)
                        .FirstOrDefaultAsync(ard => ard.Id == acesseRequestDetail.Id);
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

        public async Task<AcesseRequestDetail?> GetPendingEspecialistByIdAsync(int id, int employeeId)
        {
            return await _context.Set<AcesseRequestDetail>()
                .Include(ard => ard.Status)
                .Include(ard => ard.AcesseRequest)
                    .ThenInclude(ar => ar.GroupAd)
                        .ThenInclude(ga => ga.GroupApprovals)
                .Where(ard => 
                              ard.Id == id &&
                              ard.AcesseRequest.HasPriorApproval == true &&
                              ard.AcesseRequest.GroupAd.GroupApprovals.Any(ga => ga.EmployeeId == employeeId))
                .FirstOrDefaultAsync();
        }
    }
}
