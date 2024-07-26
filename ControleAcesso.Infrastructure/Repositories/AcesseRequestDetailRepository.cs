using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Interfaces.Entities;
using ControleAcesso.Infrastructure.Data;
using ControleAcesso.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

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
                        .Include(ard => ard.AcesseRequest)
                            .ThenInclude(ard => ard.Employee)
                         .Include(ard=> ard.AcesseRequest.GroupAd)
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

        public async Task<AcesseRequestDetail> UpdateAsync(AcesseRequestDetail acesseRequestDetail)
        {
            try
            {
                var existingEntity = await _context.Set<AcesseRequestDetail>()
                    .FirstOrDefaultAsync(ard => ard.Id == acesseRequestDetail.Id);

                if (existingEntity != null)
                {
                    var local = _context.Set<AcesseRequestDetail>().Local.FirstOrDefault(entry => entry.Id.Equals(acesseRequestDetail.Id));
                    if (local != null)
                    {
                        // Desanexa a entidade local
                        _context.Entry(local).State = EntityState.Detached;
                    }

                    // Atualize o estado da entidade
                    existingEntity.StatusRequestId = acesseRequestDetail.StatusRequestId;
                    existingEntity.Status = acesseRequestDetail.Status;

                    _context.Entry(existingEntity).State = EntityState.Modified;
                }
                else
                {
                    _context.Set<AcesseRequestDetail>().Update(acesseRequestDetail);
                }

                await _context.SaveChangesAsync();

                return await _context.Set<AcesseRequestDetail>()
                        .Include(ard => ard.Status)
                        .Include(ard => ard.ManagerApproval)
                            .ThenInclude(ard => ard.Employee)
                        .Include(ard => ard.RequesterEmployee)
                        .Include(ard => ard.AcesseRequest)
                            .ThenInclude(ard => ard.Employee)
                        .Include(ard => ard.AcesseRequest.GroupAd)
                        .FirstOrDefaultAsync(ard => ard.Id == acesseRequestDetail.Id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao atualizar a entidade.", ex);
            }
        }

        public async Task<IEnumerable<AcesseRequestDetail>> GetPendingEspecialistAsync(int employee)
        {
            return await _context.Set<AcesseRequestDetail>()
                .Include(ard => ard.Status)
                .Include(ard => ard.AcesseRequest)
                    .ThenInclude(ar => ar.GroupAd)
                        .ThenInclude(ga => ga.GroupApprovals)
                .Include(ar => ar.AcesseRequest.Employee)
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

        public async Task<AcesseRequestDetail?> GetPendingEspecialistByIdAsync(int id)
        {
            return await _context.Set<AcesseRequestDetail>()
                .AsNoTracking()
                .Include(ard => ard.Status)
                .Include(ard => ard.AcesseRequest)
                    .ThenInclude(ar => ar.GroupAd)
                        .ThenInclude(ga => ga.GroupApprovals)
                .Include(ars => ars.AcesseRequest.Employee)
                .Where(ard =>
                              ard.Id == id &&
                              ard.AcesseRequest.HasPriorApproval == true)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AcesseRequestDetail>> GetRequestByEmployeeIdAsync(int employeeId)
        {
           return await _context.Set<AcesseRequestDetail>()
                .Include(ard => ard.RequesterEmployee)
                .Include(ard => ard.Status)
                .Include(ard => ard.AcesseRequest)
                    .ThenInclude(ard => ard.GroupAd)
                .Include(ard => ard.AcesseRequest.Employee)
                .Where(ar => ar.RequesterEmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AcesseRequestDetail>> GetPendingManagerAsync(int employeeId)
        {
            return await _context.Set<AcesseRequestDetail>()
                .AsNoTracking()
                .Include(ard => ard.Status)
                .Include(ard => ard.AcesseRequest)
                    .ThenInclude(ar => ar.GroupAd)
                        .ThenInclude(ga => ga.GroupApprovals)
                .Include(ars => ars.AcesseRequest.Employee)
                .Where(ard => 
                    ard.ManagerApproval.EmployeeId == employeeId &&
                    ard.StatusRequestId == (int)EStatusRequest.AguardandoAprovacaManager
                    )
                .ToListAsync();
        }
    }
}
