﻿namespace UniformancePhdConnectSystem.Models.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class CreateRoleModel
    {
        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}