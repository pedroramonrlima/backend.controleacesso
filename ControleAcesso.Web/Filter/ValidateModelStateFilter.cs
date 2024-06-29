using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ControleAcesso.Web.Extensions;

namespace ControleAcesso.Web.Filter
{
    public class ValidateModelStateFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorResponse = context.ModelState.ToErrorResponse();
                context.Result = new BadRequestObjectResult(new
                {
                    code = 400,
                    message = "Há erros de validação.",
                    errors = errorResponse.Errors
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
