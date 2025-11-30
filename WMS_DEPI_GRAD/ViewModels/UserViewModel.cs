using System.ComponentModel.DataAnnotations;

namespace WMS_DEPI_GRAD.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; } // Single role for simplicity as per requirements
        
        public bool IsActive { get; set; } // Assuming we might add this to ApplicationUser or manage via Lockout
    }
}
