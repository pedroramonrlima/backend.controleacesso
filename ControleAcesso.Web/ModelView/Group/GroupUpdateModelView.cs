﻿using ControleAcesso.Domain.Entities;
using ControleAcesso.Web.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Web.ModelView.Group
{
    public class GroupUpdateModelView : IObjectModelView<GroupAd>
    {
        [Required(ErrorMessage = "O Id é obrigatório.")]
        public string? Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "O Nome deve conter apenas letras.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "O DN é obrigatório.")]
        [RegularExpression(@"^[a-zA-Z\s/=\,]*$", ErrorMessage = "O DN deve conter apenas letras, /, = ou ,.")]
        [StringLength(255, ErrorMessage = "O DN deve ter no máximo 255 caracteres.")]
        public string? Dn { get; set; }

        [Required(ErrorMessage = "O campo é obrigatório.")]
        public string? ExpiresHour { get; set; }

        public GroupAd ToEntity()
        {
            return new GroupAd
            {
                Id = int.Parse(Id!),
                Name = Name,
                Dn = Dn,
                ExpiresHour = int.Parse(ExpiresHour!)
            };
        }
    }
}
