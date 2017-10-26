using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iDealsSolutions.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class UpdateViewModel
    {
        public UpdateViewModel()
        {

        }
        public UpdateViewModel(ApplicationUser user)
        {
            this.Id = user.Id;
            this.Email = user.Email;
            this.Name = user.Name;
            this.Surname = user.Surname;
            this.PhoneNumber = user.PhoneNumber;
        }
        [Required]       
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Phone]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} characters long and no longer than 15.", MinimumLength = 9)]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }
       
        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Name")]
        public string Name { get; set; }
       
        [Required]
        [Phone]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} characters long and no longer than 15.", MinimumLength = 9)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }   
}
