﻿/*using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Web.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Web.ModelView.Requests
{
    public class AcesseRequestCreateModelView : IObjectModelView<AcesseRequest>
    {
        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [RegularExpression(@"^\d+$", ErrorMessage = ResponseMessages.OnlyNumbersAllowed)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required(ErrorMessage = ResponseMessages.RequiredField)]
        [RegularExpression(@"^\d+$", ErrorMessage = ResponseMessages.OnlyNumbersAllowed)]
        public string GroupAdId { get; set; } = string.Empty;
        public bool HasPriorApproval { get; set; } = false;

        public AcesseRequest ToEntity()
        {
            return new AcesseRequest
            {
                EmployeeId = int.Parse(EmployeeId),
                GroupAdId = int.Parse(GroupAdId),
                HasPriorApproval = HasPriorApproval,
            };
        }
    }
}*/
