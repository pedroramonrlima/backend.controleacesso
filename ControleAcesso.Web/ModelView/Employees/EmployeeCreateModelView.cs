
using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Web.Attributes;
using ControleAcesso.Web.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Web.ModelView.Employees
{
    public class EmployeeCreateModelView : IObjectModelView<Employee>
    {
        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [RegularExpression(@"\d{3}\.\d{3}\.\d{3}-\d{2}", ErrorMessage = "Formato de CPF inválido.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [CustomStringLength(20)]
        public string Registration { get; set; }

        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [CustomStringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(EmployeeCreateModelView), nameof(ValidateBirthDate))]
        public DateTime BomDate { get; set; }

        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(EmployeeCreateModelView), nameof(ValidateContractDate))]
        public DateTime ContractDate { get; set; }

        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        public int OfficeId { get; set; }

        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        public int EmployeeStatusId { get; set; }

        public Employee ToEntity()
        {
            return new Employee
            {
                Cpf = this.Cpf,
                Registration = this.Registration,
                Name = this.Name,
                BomDate = this.BomDate,
                ContractDate = this.ContractDate,
                OfficeId = this.OfficeId,
                DepartmentId = this.DepartmentId,
                EmployeeStatusId = this.EmployeeStatusId
            };
        }

        public static ValidationResult ValidateBirthDate(DateTime date, ValidationContext context)
        {
            if (date > DateTime.Today)
            {
                return new ValidationResult(ResponseMessages.NoFutureDateAllowed);
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateContractDate(DateTime date, ValidationContext context)
        {
            if (date < DateTime.Today)
            {
                return new ValidationResult(ResponseMessages.NoPastDateAllowed);
            }
            return ValidationResult.Success;
        }
    }
}
