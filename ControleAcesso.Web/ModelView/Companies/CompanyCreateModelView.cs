using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Web.Attributes;
using ControleAcesso.Web.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Web.ModelView.Companies
{
    public class CompanyCreateModelView : IObjectModelView<Company>
    {
        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = ResponseMessages.OnlyTextAllowed)]
        [CustomStringLength(30)]

        public string Name { get; set; } = string.Empty;


        public Company ToEntity()
        {
            var department = new Company
            {
                Name = Name
            };

            return department;
        }

    }
}
