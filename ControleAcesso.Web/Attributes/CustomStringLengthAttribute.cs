using ControleAcesso.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Web.Attributes
{
    public class CustomStringLengthAttribute : StringLengthAttribute
    {
        public CustomStringLengthAttribute(int maximumLength) : base(maximumLength)
        {
            ErrorMessage = ResponseMessages.MaxCharacters(maximumLength);
        }
    }
}
