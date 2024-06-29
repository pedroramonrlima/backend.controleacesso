using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Web.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Web.ModelView.Managers
{
    public class ManagerUpdateModelView : IObjectModelView<Manager>
    {
        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        public int Id { get; set; }
        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        public int EmployeeId { get; set; }

        public Manager ToEntity()
        {
            return new Manager 
            { 
                Id = Id,
                EmployeeId = EmployeeId 
            };

        }
    }
}
