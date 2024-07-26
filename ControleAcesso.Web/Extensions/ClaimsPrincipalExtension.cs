using ControleAcesso.Domain.Models.Token;
using System.Security.Claims;
using System.Text.Json;

namespace ControleAcesso.Web.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static Token GetTokenObject(this ClaimsPrincipal user)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            return JsonSerializer.Deserialize<Token>(user.FindFirst("Employee").Value, options);
        }
    }
}
