using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.Extensions;
using ControleAcesso.Web.ModelView.Requests;
using ControleAcesso.Web.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AcesseRequestController : ControllerBase
    {
        private readonly IAcesseRequestService _acesseRequestService;
        public AcesseRequestController(IGenericService<AcesseRequest> service, IAcesseRequestService acesseRequestService)
        {
            _acesseRequestService = acesseRequestService;
        }

        [HttpPost]
        public async Task<ActionResult<AcesseRequest>> Post(AcesseRequestCreateModelView model)
        {
            try
            {
                var result = await _acesseRequestService.AddAsync(model.GroupAds, User.GetTokenObject().EmployeeId);

                if (result.Success) 
                { 
                    return Ok(result.AcesseRequests);
                }else if (result.PartialSucess)
                {
                    return StatusCode(StatusCodes.Status207MultiStatus, new
                    {
                        SucessRequests = result.AcesseRequests,
                        Errors = result.Errors,
                        ValidationError = result.ValidationErrors
                    });
                }else
                {
                    return BadRequest(new {Errors = result.Errors, ValidationErrors = result.ValidationErrors, });
                }
            }
            catch (DomainException ex)
            {
                return this.HandleError(ex, 400, ex.Message);
            }

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcesseRequestDetail>>> GetAll()
        {
            try
            {
                var token = User.GetTokenObject();
                return Ok(await _acesseRequestService.GetAllAcesseRequestDetailAsync(token.EmployeeId));
            }
            catch (DomainException ex)
            {
                return BadRequest(ex);
            }
        }

        private ActionResult<AcesseRequest> HandleError(DomainException ex, int code, string erroMenssage)
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
                Title = ResponseMessages.NotProcessingRequest,
                Errors = errorDetails
            };

            // Adicionar o detalhe da exceção ao log para depuração
            //_logger.LogError(ex, "Erro ao processar a solicitação");

            return StatusCode(code, errorResponse);
        }
    }
}
