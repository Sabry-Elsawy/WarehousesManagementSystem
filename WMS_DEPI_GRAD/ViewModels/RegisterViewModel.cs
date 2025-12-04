using System.ComponentModel.DataAnnotations;

namespace WMS_DEPI_GRAD.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = null!;

        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
