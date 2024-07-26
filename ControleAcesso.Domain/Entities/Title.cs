﻿using ControleAcesso.Domain.Interfaces.Entities;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Domain.Entities
{
    public class Title : IEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(45)]
        public string Name { get; set; }
    }
}