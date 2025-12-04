using System.ComponentModel.DataAnnotations;

namespace WMS_DEPI_GRAD.ViewModels
{
    public class ProfileViewModel
    {
        // User Basic Info
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        // Address Fields
        [StringLength(200)]
        [Display(Name = "Street Address")]
        public string? AddressStreet { get; set; }

        [StringLength(100)]
        [Display(Name = "City")]
        public string? AddressCity { get; set; }

        [StringLength(100)]
        [Display(Name = "State/Province")]
        public string? AddressState { get; set; }

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string? AddressPostalCode { get; set; }

        [StringLength(100)]
        [Display(Name = "Country")]
        public string? AddressCountry { get; set; }

        [Phone]
        [Display(Name = "Contact Phone")]
        public string? Phone { get; set; }

        [StringLength(100)]
        [Display(Name = "Company")]
        public string? Company { get; set; }
    }
}
