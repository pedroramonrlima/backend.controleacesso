using ControleAcesso.Web.Response;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ControleAcesso.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static ErrorResponse ToErrorResponse(this ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(ms => ms.Value.Errors.Any())
                .SelectMany(ms => ms.Value.Errors.Select(e => new ErrorDetail
                {
                    Field = ms.Key,
                    ErrorMessage = e.ErrorMessage
                }))
                .ToList();

            return new ErrorResponse
            {
                Code = 400,
                Message = "Há erros de validação.",
                Errors = errors
            };
        }
    }
}
