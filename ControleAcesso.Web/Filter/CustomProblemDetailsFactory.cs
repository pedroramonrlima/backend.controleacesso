using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using ControleAcesso.Web.Extensions;
using ControleAcesso.Web.Response;
using ControleAcesso.Domain.Constants;

namespace ControleAcesso.Web.Filter
{
    public class CustomProblemDetailsFactory : ProblemDetailsFactory
    {
        private readonly ApiBehaviorOptions options;
        private readonly JsonOptions jsonOptions;

        public CustomProblemDetailsFactory(IOptions<ApiBehaviorOptions> options, IOptions<JsonOptions> jsonOptions)
        {
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.jsonOptions = jsonOptions?.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
        }

        public override ProblemDetails CreateProblemDetails(
            HttpContext httpContext,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null)
        {
            statusCode ??= 500;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(
            HttpContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null)
        {
            if (modelStateDictionary == null)
            {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            statusCode ??= 400;

            var errorResponse = modelStateDictionary.ToErrorResponse();

            return new CustomValidationProblemDetails
            {
                Status = statusCode,
                Title = ResponseMessages.ErrorValidate,
                Errors = errorResponse.Errors,
                Detail = errorResponse.Message,
                Instance = instance,
                Type = type
            };
        }

        private void ApplyProblemDetailsDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
        {
            problemDetails.Status ??= statusCode;

            if (options.ClientErrorMapping.TryGetValue(statusCode, out var clientErrorData))
            {
                problemDetails.Title ??= clientErrorData.Title;
                problemDetails.Type ??= clientErrorData.Link;
            }

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null)
            {
                problemDetails.Extensions["traceId"] = traceId;
            }
        }
    }

    public class CustomValidationProblemDetails : ValidationProblemDetails
    {
        public List<ErrorDetail> Errors { get; set; }

        public CustomValidationProblemDetails() : base()
        {
            Errors = new List<ErrorDetail>();
        }
    }
}
