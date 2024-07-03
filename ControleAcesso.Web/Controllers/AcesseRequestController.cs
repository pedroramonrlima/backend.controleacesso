using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.ModelView.Requests;
using ControleAcesso.Web.Response;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcesseRequestController : AbstractController<AcesseRequest, AcesseRequestCreateModelView, AcesseRequestUpdateModelView>
    {
        private readonly IAcesseRequestService _acesseRequestService;
        public AcesseRequestController(IGenericService<AcesseRequest> service, IAcesseRequestService acesseRequestService) : base(service)
        {
            _acesseRequestService = acesseRequestService;
        }

        public override async Task<ActionResult<AcesseRequest>> Post(AcesseRequestCreateModelView model)
        {
            try
            {
                return Ok(await _acesseRequestService.AddAsync(model.ToEntity()));
            }
            catch (DomainException ex)
            {
                return this.HandleError(ex, 400, ex.Message);
            }

        }

        protected override ActionResult<AcesseRequest> HandleError(DomainException ex, int code, string erroMenssage)
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
