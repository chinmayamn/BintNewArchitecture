using System.ComponentModel.DataAnnotations;

namespace Bint.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class MobileLoginViewModel
    {
        [Required]
        [Display(Name ="Mobile Number")]
        public string MobileNumber { get; set; }

        [Required]
        public string OTP { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


}
