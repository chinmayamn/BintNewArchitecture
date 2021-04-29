using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bint.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        [StringLength(10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid mobile number")]
        public string Mobile { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
      
        public string Address { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string IfscCode { get; set; }
        public string AccountHolderName { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedId { get; set; }
        public string Pan { get; set; }
        public string UpiId { get; set; }
        public string Status { get; set; }
        public string ProfilePicture { get; set; }

    }
}
