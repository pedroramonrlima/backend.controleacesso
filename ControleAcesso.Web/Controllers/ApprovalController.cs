﻿using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Web.ModelView.Requests;
using ControleAcesso.Web.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalController : ControllerBase
    {
        private readonly IAcesseRequestDetailService _approvalRequestDetailService;

        public ApprovalController(IAcesseRequestDetailService approvalRequestDetailService)
        {
            _approvalRequestDetailService = approvalRequestDetailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcesseRequestDetail>>> GetAll()
        {
            return Ok(await _approvalRequestDetailService.GetPedentManager(2));
        }

        [HttpPost]

        public async Task<ActionResult<AcesseRequestDetail>> post(ApprovalRequestModelView acesseRequest)
        {
            try
            {
                return Ok(await _approvalRequestDetailService.ApproveManager(acesseRequest.ToEntity(), 1));
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
                Title = ResponseMessages.NotProcessingRequest,
                Errors = errorDetails
            };

            // Adicionar o detalhe da exceção ao log para depuração
            //_logger.LogError(ex, "Erro ao processar a solicitação");

            return StatusCode(code, errorResponse);
        }
    }
}
