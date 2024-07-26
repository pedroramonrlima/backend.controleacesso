using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.Extensions;
using ControleAcesso.Web.Interfaces;
using ControleAcesso.Web.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace ControleAcesso.Web.Controllers
{
    public abstract class AbstractController<T, TCreateModelView, TUpdateModelView> : ControllerBase
        where T : class
        where TCreateModelView : IObjectModelView<T>
        where TUpdateModelView : IObjectModelView<T>
    {
        protected readonly IGenericService<T> _service;

        public AbstractController(IGenericService<T> service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<T>>> GetAll()
        {
            try
            {
                return Ok(await _service.GetAllAsync());

            }catch(DomainException ex)
            {
                 return HandleError<IEnumerable<T>>(ex, 404, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<T>> GetById(int id)
        {
            try
            {
                return Ok(await _service.GetByIdAsync(id));
            }
            catch (DomainException ex)
            {
                return HandleError(ex, 404, ex.Message);
            }
        }

        [HttpPost]
        public virtual async Task<ActionResult<T>> Post(TCreateModelView model)
        {
            try
            {
                return Ok(await _service.AddAsync(model.ToEntity()));
            }
            catch (DomainException ex)
            {
                return HandleError(ex, 400, ex.Message);
            }

        }

        [HttpPut]
        public async Task<ActionResult<T>> Put(TUpdateModelView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToErrorResponse());
            }

            try
            {
                return Ok(await _service.UpdateAsync(model.ToEntity()));
            }
            catch (DomainException ex)
            {
                return HandleError(ex, 400, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<T>> Delete(int id)
        {
            try
            {
                return Ok(await _service.Delete(id));
            }
            catch (DomainException ex)
            {
                return HandleError(ex,400, ex.Message);
            }
        }

        protected object FormatValidationErrors(ModelStateDictionary modelState, int statusCode)
        {
            var errors = modelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return new
            {
                errors = errors,
                title = "One or more validation errors occurred.",
                status = statusCode
            };
        }

        protected virtual ActionResult<T> HandleError(DomainException ex, int code, string erroMenssage)
        {
            var errorResponse = new
            {
                Status = code,
                Title = ResponseMessages.NotProcessingRequest,
                Errors = new List<ErrorDetail>
            {
                new ErrorDetail
                {
                    Field = "DomainException",
                    ErrorMessage = erroMenssage
                }
            }
            };

            // Adicionar o detalhe da exceção ao log para depuração
            //_logger.LogError(ex, "Erro ao processar a solicitação");

            return StatusCode(code,errorResponse);
        }

        protected ActionResult<TResponse> HandleError<TResponse>(DomainException ex, int code, string errorMessage)
        {
            var errorResponse = new
            {
                Code = code,
                Message = ResponseMessages.NotProcessingRequest,
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        Field = "DomainException",
                        ErrorMessage = errorMessage
                    }
                }
            };

            // Adicionar o detalhe da exceção ao log para depuração
            //_logger.LogError(ex, "Erro ao processar a solicitação");

            return StatusCode(code,errorResponse);
        }
    }
}
