using System.ComponentModel.DataAnnotations;

namespace WMS_DEPI_GRAD.ViewModels
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare( nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
