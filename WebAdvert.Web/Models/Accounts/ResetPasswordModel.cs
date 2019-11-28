using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
     public class ResetPasswordModel
    {
        
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter the assigned token")]
        [Display(Name = "Token")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Enter a new password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }

        public ResetPasswordModel()
        {
        }
    }
}