using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Web.Attributes;
using ControleAcesso.Web.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Web.ModelView.Employees
{
    public class EmployeeStatusCreateModelView : IObjectModelView<EmployeeStatus>
    {
        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = ResponseMessages.OnlyTextAllowed)]
        [CustomStringLength(45)]
        public string Name { get; set; } = string.Empty;
        public EmployeeStatus ToEntity()
        {
            return new EmployeeStatus { Name = Name };
        }
    }
}
