using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Web.Attributes;
using ControleAcesso.Web.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Web.ModelView.Departament
{
    public class DepartamentCreateModelView : IObjectModelView<Department>
    {
        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = ResponseMessages.OnlyTextAllowed)]
        [CustomStringLength(45)]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"^\d+$", ErrorMessage = ResponseMessages.OnlyNumbersAllowed)]
        public string? ManageId {  get; set; }
        
        public Department ToEntity()
        {
            var department = new Department
            {
                Name = Name
            };

            if (ManageId != null)
            {
                int parsedManagerId;
                if (int.TryParse(ManageId, out parsedManagerId))
                {
                    department.ManagerId = parsedManagerId;
                }
            }

            return department;
        }
    }
}
