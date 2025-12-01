using System.ComponentModel.DataAnnotations;

namespace WMS_DEPI_GRAD.ViewModels;

public class CustomerViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Customer Name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Address")]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Phone")]
    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateCustomerViewModel
{
    [Required(ErrorMessage = "Customer Name is required")]
    [Display(Name = "Customer Name")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Address")]
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Phone")]
    [Phone(ErrorMessage = "Invalid phone format")]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class CustomerListViewModel
{
    public IReadOnlyList<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public string? SearchTerm { get; set; }
    public bool? FilterActive { get; set; }
    public string SortBy { get; set; } = "Name";
    public string SortOrder { get; set; } = "asc";
    
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
