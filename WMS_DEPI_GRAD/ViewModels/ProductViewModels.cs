using System.ComponentModel.DataAnnotations;

namespace WMS_DEPI_GRAD.ViewModels;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Product Code")]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Product Name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Unit of Measure")]
    [StringLength(20)]
    public string UnitOfMeasure { get; set; } = string.Empty;

    [Display(Name = "Weight (kg)")]
    [Range(0, double.MaxValue)]
    public double Weight { get; set; }

    [Display(Name = "Volume (m³)")]
    [Range(0, double.MaxValue)]
    public double Volume { get; set; }

    [Display(Name = "Barcode")]
    [StringLength(100)]
    public string Barcode { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateProductViewModel
{
    [Required(ErrorMessage = "Product Code is required")]
    [Display(Name = "Product Code")]
    [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product Name is required")]
    [Display(Name = "Product Name")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Unit of Measure is required")]
    [Display(Name = "Unit of Measure")]
    [StringLength(20)]
    public string UnitOfMeasure { get; set; } = "PCS";

    [Display(Name = "Weight (kg)")]
    [Range(0, double.MaxValue, ErrorMessage = "Weight must be positive")]
    public double Weight { get; set; }

    [Display(Name = "Volume (m³)")]
    [Range(0, double.MaxValue, ErrorMessage = "Volume must be positive")]
    public double Volume { get; set; }

    [Display(Name = "Barcode")]
    [StringLength(100)]
    public string Barcode { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class ProductListViewModel
{
    public IReadOnlyList<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
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
