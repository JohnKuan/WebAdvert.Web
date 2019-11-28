﻿using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class ConfirmModel
    {
        [Required(ErrorMessage ="Email is required.")]
        [Display(Name ="Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage ="Code is required.")]
        [Display(Name = "Code from email")]
        public string Code { get; set; }
    }
}