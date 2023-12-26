using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Đồ_án_của_Thái.Models
{
    public class RegisterAdminViewModels
    {
        [Required]
        [EmailAddress]
        [RegularExpression("^(.+)@(google\\.com|email\\.com|gmail\\.com|yahoo\\.com)$", ErrorMessage = "Email must be a valid MAIL address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        
    }
}