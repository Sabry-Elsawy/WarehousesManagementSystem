# Master Data Implementation Guide

## Overview
This guide covers the completed Master Data implementation for **Products**, **Suppliers** (Vendors), and **Customers** in the Warehouse Management System (WMS).

## What Has Been Implemented

### 1. Entities & Database
- **Product**: Updated with `Barcode` and `IsActive` fields
- **Vendor** (Supplier): Updated with `ContactPhone` and `IsActive` fields
- **Customer**: New entity created with `Name`, `Address`, `Phone`, `Email`, `IsActive`
- **Migration**: `AddMasterDataUpdates` migration created and applied successfully

### 2. Repository Layer
- **GenericRepository**: Enhanced with `GetPagedListAsync` method for pagination, filtering, and sorting
- Supports:
  - Filter expressions
  - Custom sort order
  - Include properties for eager loading
  - Efficient skip/take pagination

### 3. Service Layer (BLL)
Location: `WMS.BLL/Services/` and `WMS.BLL/Interfaces/`

**Implemented Services:**
- `ProductService` → `IProductService`
- `SupplierService` → `ISupplierService  `
- `CustomerService` → `ICustomerService`

**Service Features:**
- Full CRUD operations
- Pagination with filtering and sorting
- Server-side validation (unique code/name/email)
- Soft delete (toggle `IsActive` status)
- Exception handling with meaningful errors

### 4. ViewModels (DTOs)
Location: `WMS_DEPI_GRAD/ViewModels/`

**Files:**
- `ProductViewModels.cs` → `ProductViewModel`, `CreateProductViewModel`, `ProductListViewModel`
- `SupplierViewModels.cs` → `SupplierViewModel`, `CreateSupplierViewModel`, `SupplierListViewModel`
- `CustomerViewModels.cs` → `CustomerViewModel`, `CreateCustomerViewModel`, `CustomerListViewModel`

**Validation Attributes Applied:**
- `[Required]`, `[StringLength]`, `[EmailAddress]`, `[Phone]`, `[Range]`

### 5. Controllers (MVC)
Location: `WMS_DEPI_GRAD/Controllers/`

**Implemented:**
- `ProductController` - Full CRUD with pagination
- `SupplierController` - Full CRUD with pagination
- `CustomerController` - Full CRUD with pagination

**Actions:** Index, Details, Create (GET/POST), Edit (GET/POST), Delete (GET/POST)

### 6. Views (Razor)
Location: `WMS_DEPI_GRAD/Views/`

**Completed for Product:**
- `Product/Index.cshtml` - List with pagination, search, filter, sort
- `Product/Create.cshtml` - Create form with validation
- `Product/Edit.cshtml` - Edit form
- `Product/Details.cshtml` - Read-only view
- `Product/Delete.cshtml` - Confirmation page for toggling status

**TODO for Supplier & Customer:**
Views need to be created following the same pattern as Product views. See templates below.

## Setup Instructions

### 1. Database Migration (Already Done)
```bash
# Migration was created and applied
dotnet ef migrations add AddMasterDataUpdates --project WMS.DL/WMS.DAL.csproj --startup-project WMS_DEPI_GRAD/WMS_DEPI_GRAD.csproj
dotnet ef database update --project WMS.DL/WMS.DAL.csproj --startup-project WMS_DEPI_GRAD/WMS_DEPI_GRAD.csproj
```

### 2. Service Registration (Already Done)
Services are registered in `DependencyInjection.cs`:
```csharp
services.AddScoped<IProductService, ProductService>();
services.AddScoped<ISupplierService, SupplierService>();
services.AddScoped<ICustomerService, CustomerService>();
```

### 3. Running the Application
```bash
cd WMS_DEPI_GRAD
dotnet run
```

Navigate to:
- Products: `/Product`
- Suppliers: `/Supplier`
- Customers: `/Customer`

## Creating Remaining Views

### Supplier Views Template
Create the following files in `Views/Supplier/`:

1. **Index.cshtml** - Copy from `Views/Product/Index.cshtml` and replace:
   - `ProductListViewModel` → `SupplierListViewModel`
   - `Products` → `Suppliers`
   - Table columns: Code, Name, Unit, Weight, Volume, Barcode → Name, Email, Phone, Address

2. **Create.cshtml** - Copy from `Views/Product/Create.cshtml` and adapt fields:
   - Remove: Code, Description, UnitOfMeasure, Weight, Volume, Barcode
   - Keep: Name, IsActive
   - Add: Address, ContactEmail, ContactPhone

3. **Edit.cshtml** - Same as Create but bind to `SupplierViewModel`

4. **Details.cshtml** - Copy from Product and adjust displayed fields

5. **Delete.cshtml** - Copy from Product and replace model

### Customer Views Template
Same approach as Supplier:
- Fields: Name, Address, Phone, Email, IsActive
- Follow same pattern as Product views

## API Usage Examples

### ProductService Example
```csharp
// Get paginated products
var (products, totalCount) = await _productService.GetPagedListAsync(
    pageNumber: 1,
    pageSize: 10,
    searchTerm: "widget",
    filterActive: true,
    sortBy: "Name",
    sortOrder: "asc"
);

// Create product
var product = new Product
{
    Code = "PRD001",
    Name = "Widget A",
    UnitOfMeasure = "PCS",
    IsActive = true
};
await _productService.CreateAsync(product); // Will validate unique code

// Update product
product.Name = "Widget A (Updated)";
await _productService.UpdateAsync(product);

// Toggle active status
await _productService.ToggleActiveAsync(productId);
```

## Testing

### Manual Testing Checklist
1. **Product CRUD**
   - [ ] Create a new product
   - [ ] List products with pagination
   - [ ] Search and filter products
   - [ ] Edit a product
   - [ ] View product details
   - [ ] Toggle product active status
   - [ ] Verify unique code validation

2. **Supplier CRUD** (after creating views)
   - [ ] Create a new supplier
   - [ ] List suppliers with pagination
   - [ ] Verify unique name validation
   - [ ] Edit and toggle status

3. **Customer CRUD** (after creating views)
   - [ ] Create a new customer
   - [ ] List customers with pagination
   - [ ] Verify unique email validation
   - [ ] Edit and toggle status

### Unit Test Example
See `ProductServiceTests.cs` for an example unit test.

## Architecture

```
Browser
  ↓
Controller (ProductController)
  ↓
ViewModel (CreateProductViewModel)
  ↓ (maps to Domain Model)
Service (ProductService)
  ↓
UnitOfWork
  ↓
GenericRepository<Product, int>
  ↓
Entity Framework Core
  ↓
SQL Server Database
```

## Key Design Decisions

1. **ViewModels over Domain Models**: Controllers always return ViewModels to Views, never domain entities
2. **Generic Repository**: Single `GenericRepository<TEntity, TKey>` handles all CRUD via UnitOfWork
3. **Soft Delete**: `IsActive` flag instead of physical deletion
4. **Pagination in Repository**: Filtering and sorting logic pushed to repository layer
5. **Service Validation**: Uniqueness checks and business rules enforced in services
6. **Consistent Patterns**: All three entities follow identical CRUD patterns

## Troubleshooting

### Build Errors
```bash
dotnet clean
dotnet build
```

### Migration Issues
```bash
# Remove last migration
dotnet ef migrations remove --project WMS.DL/WMS.DAL.csproj --startup-project WMS_DEPI_GRAD/WMS_DEPI_GRAD.csproj

# Recreate
dotnet ef migrations add AddMasterDataUpdates --project WMS.DL/WMS.DAL.csproj --startup-project WMS_DEPI_GRAD/WMS_DEPI_GRAD.csproj
dotnet ef database update --project WMS.DL/WMS.DAL.csproj --startup-project WMS_DEPI_GRAD/WMS_DEPI_GRAD.csproj
```

## Next Steps

1. **Create Supplier & Customer Views** (10 views total)
   - Use Product views as templates
   - Adjust fields and model names

2. **Add Navigation Menu Items**
   - Update `_Layout.cshtml` to add links to Product/Supplier/Customer

3. **Optional Enhancements**
   - Add export to Excel functionality
   - Add bulk upload/import
   - Add product images
   - Add supplier rating system

4. **Unit Tests**
   - Expand test coverage using `ProductServiceTests.cs` as template
   - Test edge cases and validation

## Summary

✅ **Completed:**
- Entities (Product, Vendor/Supplier, Customer)
- Repository enhancements (Pagination)
- Services (3 complete with validation)
- ViewModels (3 sets with validation)
- Controllers (3 complete)
- Product Views (5 complete)
- Service registration
- Database migration

⏳ **Remaining:**
- Supplier Views (5 views)
- Customer Views (5 views)
- Unit tests expansion
- Navigation menu updates

**Estimated Time to Complete:** ~2 hours for remaining views (copy/paste/adapt from Product)
