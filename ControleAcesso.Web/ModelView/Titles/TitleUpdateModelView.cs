using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Web.Attributes;
using ControleAcesso.Web.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Web.ModelView.Titles
{
    public class TitleUpdateModelView : IObjectModelView<Title>
    {
        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [RegularExpression(@"^\d+$", ErrorMessage = ResponseMessages.OnlyNumbersAllowed)]
        public string Id { get; set; } = string.Empty;
        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = ResponseMessages.OnlyTextAllowed)]
        [CustomStringLength(30)]
        public string Name { get; set; } = string.Empty;
        public Title ToEntity()
        {
            var department = new Title
            {
                Id = int.Parse(Id),
                Name = Name
            };

            return department;
        }

    }
}

