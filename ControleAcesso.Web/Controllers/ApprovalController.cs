using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Domain.Models.Token;
using ControleAcesso.Web.Extensions;
using ControleAcesso.Web.ModelView.Requests;
using ControleAcesso.Web.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalController : ControllerBase
    {
        private readonly IAcesseRequestDetailService _approvalRequestDetailService;
        public ApprovalController(IAcesseRequestDetailService approvalRequestDetailService)
        {
            _approvalRequestDetailService = approvalRequestDetailService;
        }

        [HttpGet("list-especialist")]
        public async Task<ActionResult<IEnumerable<AcesseRequestDetail>>> GetAllEspecialist()
        {
            var _token = User.GetTokenObject();
            return Ok(await _approvalRequestDetailService.GetPendingEspecialistAsync(_token.EmployeeId));
        }

        [HttpGet("list-manager")]
        public async Task<ActionResult<IEnumerable<AcesseRequestDetail>>> GetAllManager()
        {
            var _token = User.GetTokenObject();
            return Ok(await _approvalRequestDetailService.GetPendingManagerAsync(_token.EmployeeId));
        }

        [HttpPost("approve-manager")]
        public async Task<ActionResult<AcesseRequestDetail>> post(ApprovalRequestModelView acesseRequest)
        {
            var token = User.GetTokenObject();
            try
            {
                if (acesseRequest.IsApproved)
                {
                    return Ok(await _approvalRequestDetailService.ApproveManagerAsync(acesseRequest.ToEntity(), token.EmployeeId));
                }
                else
                {
                    return Ok(await _approvalRequestDetailService.ManagerRejectAsync(acesseRequest.ToEntity(), token.EmployeeId));
                }
            }
            catch (DomainException ex)
            {
                return HandleError(ex, 400, "");
            }
        }

        [HttpPost("approve-especialista")]
        public async Task<ActionResult<AcesseRequestDetail>> ApproveEspecialistas(ApprovalRequestModelView acesseRequest)
        {
            try
            {
                var token = User.GetTokenObject();
                if(acesseRequest.IsApproved)
                {
                    return Ok(await _approvalRequestDetailService.PriorApprovalAsync(acesseRequest.ToEntity(), token.EmployeeId));
                }else
                {
                    return Ok(await _approvalRequestDetailService.PriorRejectAsync(acesseRequest.ToEntity(), token.EmployeeId));
                }
            }
            catch (DomainException ex)
            {
                return HandleError(ex, 400, "");
            }
        }

        private ActionResult<AcesseRequestDetail> HandleError(DomainException ex, int code, string erroMenssage)
        {
            List<ErrorDetail> errorDetails = ex.Properties.SelectMany(prop =>
                prop.Value.Select(value => new ErrorDetail
                {
                    Field = prop.Key,
                    ErrorMessage = value
                })
            ).ToList();

            var errorResponse = new
            {
                Status = code,
                Title = ex.Message,
                Errors = errorDetails
            };

            // Adicionar o detalhe da exceção ao log para depuração
            //_logger.LogError(ex, "Erro ao processar a solicitação");

            return StatusCode(code, errorResponse);
        }
    }
}
