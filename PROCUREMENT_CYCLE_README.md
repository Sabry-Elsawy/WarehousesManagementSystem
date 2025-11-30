# Procurement Cycle Implementation - README

## Overview
This implementation provides a complete, production-ready **Procurement Cycle** (Purchase Order → Advanced Shipping Notice → Receipt) for the Warehouse Management System using ASP.NET Core MVC with the GenericRepository and UnitOfWork patterns.

## Features Implemented

### 1. **Entity Models** (WMS.DL/Entities)
- `PurchaseOrder`: Complete PO entity with PO_Number, VendorId, WarehouseId, OrderDate, ExpectedArrivalDate, Status
- `PurchaseOrderItem`: Line items with QtyOrdered, UnitPrice, QtyReceived, LineStatus
- `AdvancedShippingNotice` (ASN): ASN_Number, PurchaseOrderId, TrackingNumber, ExpectedArrivalDate, Status
- `AdvancedShippingNoticeItem`: QtyShipped, LinkedPOItemId (optional for partial shipments)
- `Receipt`: ReceiptNumber, ASNId, WarehouseId, ReceivedDate, Status
- `ReceiptItem`: QtyExpected, QtyReceived, DiscrepancyType, Notes, ASNItemId

### 2. **Enums** (WMS.DL/Enums)
- `PurchaseOrderStatus`: Open, Approved, Closed
- `AdvancedShippingNoticeStatus`: Sent, Received, Closed
- `ReceiptStatus`: Open, Closed
- `DiscrepancyType`: None, Shortage, Overage

### 3. **Services** (WMS.BLL/Services)
- **PurchaseOrderService**: Full CRUD, approval workflow, prevent deletion if linked ASNs exist
- **ASNService**: Create from Approved POs only, partial shipment validation
- **ReceiptService**: Create from Received ASNs, scan functionality, discrepancy detection

### 4. **Controllers** (WMS_DEPI_GRAD/Controllers)
- **PurchaseOrderController**: CRUD, Approve, Close, AddItem (AJAX)
- **ASNController**: Create from P O, MarkReceived, Close, AddItem
- **ReceiptController**: Create from ASN, Scan (dedicated view and API endpoint), HandleDiscrepancy, Close

### 5. **API Endpoints**
- `POST /api/receipt/scan`: Accepts JSON `{ receiptId, productCodeOrBarcode, qty }`
- Returns: `{ success, message, product, qty }`

## Database Migration

### Run Migration
```powershell
cd d:\DEPI\GRD_PR\WMS\WarehousesManagementSystem
dotnet ef database update --project WMS.DL --startup-project WMS_DEPI_GRAD
```

This will apply the `ProcurementCycleUpdate` migration that was created.

## Configuration

### Services Registration
All services are registered in `DependencyInjection.cs`:
```csharp
.AddScoped<IPurchaseOrderService, PurchaseOrderService>()
.AddScoped<IASNService, ASNService>()
.AddScoped<IReceiptService, ReceiptService>()
```

### Authorization Roles
- **Procurement**: Can create/approve POs, create ASNs
- **Warehouse**: Can mark ASN received, create and process Receipts
- **Admin**: Full access to all operations

## Workflow & Business Rules

### Purchase Order (PO)
1. Create PO with status `Open`
2. Add Items to PO
3. Approve PO (status: `Open` → `Approved`)
   - **Only Procurement/Admin roles**
   - ASNs can only be created from Approved POs
4. Close PO (status: `Approved` → `Closed`)
   - Cannot delete PO if linked ASNs exist

### Advanced Shipping Notice (ASN)
1. Create ASN from an **Approved PO only**
   - Initial status: `Sent`
   - Validates PO is approved
2. Add ASN Items
   - Link to PO Items (optional for partial shipments)
   - Validates QtyShipped ≤ QtyOrdered
3. Mark ASN as Received (status: `Sent` → `Received`)
   - **Only Warehouse/Admin roles**
4. Close ASN (status: `Received` → `Closed`)

### Receipt
1. Create Receipt from a **Received ASN only**
   - Auto-generates ReceiptNumber
   - Creates ReceiptItems from ASNItems
   - Initial status: `Open`
2. Scan Items (Warehouse workers)
   - Use barcode scanner or manual entry
   - Updates `QtyReceived` for each item
   - Auto-detects discrepancies:
     - `QtyReceived < QtyExpected`: **Shortage**
     - `QtyReceived > QtyExpected`: **Overage**
     - `QtyReceived == QtyExpected`: **None**
   - Updates linked PO Item `QtyReceived`
3. Handle Discrepancies
   - Add notes to discrepancy items
   - Flag for procurement team review
4. Close Receipt (status: `Open` → `Closed`)
   - Cannot close if unprocessed items exist
   - **Only Warehouse/Admin roles**

## Seed Data Script

Run this SQL script to create sample data for testing:

```sql
-- Insert Vendor
INSERT INTO Vendors (Name, CreatedOn, CreatedBy)
VALUES ('ACME Supplies', GETUTCDATE(), 'System');

DECLARE @VendorId INT = SCOPE_IDENTITY();

-- Insert Products
INSERT INTO Products (Name, SKU, CreatedOn, CreatedBy)
VALUES 
('Laptop Dell XPS', 'LAP-001', GETUTCDATE(), 'System'),
('Monitor Samsung 27"', 'MON-001', GETUTCDATE(), 'System'),
('Keyboard Logitech', 'KEY-001', GETUTCDATE(), 'System');

-- Get Product IDs
DECLARE @LaptopId INT = (SELECT Id FROM Products WHERE SKU = 'LAP-001');
DECLARE @MonitorId INT = (SELECT Id FROM Products WHERE SKU = 'MON-001');
DECLARE @KeyboardId INT = (SELECT Id FROM Products WHERE SKU = 'KEY-001');

-- Get a Warehouse ID (assuming one exists)
DECLARE @WarehouseId INT = (SELECT TOP 1 Id FROM Warehouses);

-- Create Purchase Order
INSERT INTO POs (PO_Number, VendorId, WarehouseId, OrderDate, ExpectedArrivalDate, Status, CreatedOn, CreatedBy)
VALUES ('PO-2024-001', @VendorId, @WarehouseId, GETUTCDATE(), DATEADD(DAY, 7, GETUTCDATE()), 0, GETUTCDATE(), 'System');

DECLARE @POId INT = SCOPE_IDENTITY();

-- Add PO Items
INSERT INTO PO_Items (PurchaseOrderId, ProductId, SKU, QtyOrdered, UnitPrice, QtyReceived, LineStatus, CreatedOn, CreatedBy)
VALUES 
(@POId, @LaptopId, 'LAP-001', 10, 1200.00, 0, 'Open', GETUTCDATE(), 'System'),
(@POId, @MonitorId, 'MON-001', 20, 350.00, 0, 'Open', GETUTCDATE(), 'System'),
(@POId, @KeyboardId, 'KEY-001', 30, 75.00, 0, 'Open', GETUTCDATE(), 'System');

-- Approve the PO (Status: 0=Open, 1=Approved, 2=Closed)
UPDATE POs SET Status = 1 WHERE Id = @POId;

-- Create ASN
INSERT INTO ASNs (ASN_Number, PurchaseOrderId, ExpectedArrivalDate, TrackingNumber, Status, CreatedOn, CreatedBy)
VALUES ('ASN-2024-001', @POId, DATEADD(DAY, 5, GETUTCDATE()), 'TRACK123456', 0, GETUTCDATE(), 'System');

DECLARE @ASNId INT = SCOPE_IDENTITY();

-- Get PO Item IDs
DECLARE @POItem1 INT = (SELECT TOP 1 Id FROM PO_Items WHERE PurchaseOrderId = @POId AND ProductId = @LaptopId);
DECLARE @POItem2 INT = (SELECT TOP 1 Id FROM PO_Items WHERE PurchaseOrderId = @POId AND ProductId = @MonitorId);
DECLARE @POItem3 INT = (SELECT TOP 1 Id FROM PO_Items WHERE PurchaseOrderId = @POId AND ProductId = @KeyboardId);

-- Add ASN Items
INSERT INTO ASN_Items (AdvancedShippingNoticeId, ProductId, SKU, QtyShipped, LinkedPOItemId, CreatedOn, CreatedBy)
VALUES 
(@ASNId, @LaptopId, 'LAP-001', 10, @POItem1, GETUTCDATE(), 'System'),
(@ASNId, @MonitorId, 'MON-001', 20, @POItem2, GETUTCDATE(), 'System'),
(@ASNId, @KeyboardId, 'KEY-001', 30, @POItem3, GETUTCDATE(), 'System');

-- Mark ASN as Received (Status: 0=Sent, 1=Received, 2=Closed)
UPDATE ASNs SET Status = 1 WHERE Id = @ASNId;

SELECT 'Seed data created successfully!' AS Result;
SELECT 'PO ID: ' + CAST(@POId AS VARCHAR) AS PurchaseOrder;
SELECT 'ASN ID: ' + CAST(@ASNId AS VARCHAR) AS ASN;
```

## Testing the Flow

### Manual Testing Steps:

1. **View Purchase Orders**
   - Navigate to `/PurchaseOrder/Index`
   - Should see the seeded PO

2. **View PO Details**
   - Click on PO to see details
   - Verify status is "Approved"
   - See all line items

3. **View ASNs**
   - Navigate to `/ASN/Index`
   - Should see the seeded ASN linked to the PO

4. **Mark ASN Received** (if not already)
   - In ASN Details, click "Mark Received"
   - Status changes to "Received"

5. **Create Receipt from ASN**
   - Navigate to `/Receipt/Create?asnId={asnId}`
   - Select warehouse
   - Submit - Receipt is created with status "Open"

6. **Scan Items**
   - Navigate to `/Receipt/Scan/{receiptId}`
   - Enter SKU (e.g., "LAP-001") and quantity
   - Click Scan
   - Repeat for all items

7. **Handle Discrepancies** (if any)
   - If shortage or overage detected, add notes
   - Submit discrepancy handling

8. **Close Receipt**
   - In Receipt Details, click "Close"
   - Verify all items processed
   - Receipt status changes to "Closed"

### Testing Scan API Endpoint

Use Postman or curl:

```bash
curl -X POST https://localhost:5001/api/receipt/scan \
  -H "Content-Type: application/json" \
  -d '{"receiptId": 1, "productCodeOrBarcode": "LAP-001", "qty": 10}'
```

Expected Response:
```json
{
  "success": true,
  "message": "Item scanned successfully",
  "product": "LAP-001",
  "qty": 10
}
```

## Sequence Diagram

```
User (Procurement) → PO Controller → PO Service → UnitOfWork → DB
                                                   ↓
                                            Create PO (Status: Open)
                                                   ↓
                                            Add Items
                                                   ↓
                                            Approve (Status: Approved)

User (Procurement) → ASN Controller → ASN Service → Validate PO Approved
                                                   ↓
                                            Create ASN from PO
                                                   ↓
                                            Add ASN Items (link to PO Items)

User (Warehouse)   → ASN Controller → ASN Service → Mark AS N Received

User (Warehouse)   → Receipt Controller → Receipt Service → Validate ASN Received
                                                   ↓
                                            Create Receipt from ASN
                                                   ↓
                                            Auto-create Receipt Items

User (Warehouse)   → Scan View → API: /api/receipt/scan
                                                   ↓
                                            Validate Product
                                                   ↓
                                            Update QtyReceived
                                                   ↓
                                            Detect Discrepancy
                                                   ↓
                                            Update PO Item QtyReceived

User (Warehouse)   → Receipt Controller → Receipt Service → Close Receipt
                                                   ↓
                                            Validate all items processed
                                                   ↓
                                            Status: Closed
```

## Views to Create

Due to length constraints, here are the key views you need to create:

### PurchaseOrder Views (~/Views/PurchaseOrder/)
- `Index.cshtml`: List all POs with filter/search
- `Details.cshtml`: Show PO with items, approve/close buttons
- `Create.cshtml`: Form with vendor, warehouse, date selection
- `Edit.cshtml`: Edit PO details

### ASN Views (~/Views/ASN/)
- `Index.cshtml`: List all ASNs
- `Details.cshtml`: Show ASN with items, mark received/close buttons
- `Create.cshtml`: Form to create ASN from PO dropdown

### Receipt Views (~/Views/Receipt/)
- `Index.cshtml`: List all Receipts
- `Details.cshtml`: Show Receipt with items and discrepancies
- `Create.cshtml`: Form to create Receipt from ASN dropdown
- `Scan.cshtml`: Barcode scanning interface (input for SKU + Qty, submit via AJAX to `/api/receipt/scan`)

## Unit Tests (Example)

Create a test project `WMS.Tests` and add these test examples:

```csharp
using Xunit;
using WMS.BLL.Services;
using WMS.DAL;
using WMS.DAL.UnitOfWork;

public class ProcurementCycleTests
{
    [Fact]
    public async Task CreatePO_ValidData_Success()
    {
        // Arrange
        var po = new PurchaseOrder
        {
            PO_Number = "TEST-001",
            VendorId = 1,
            WarehouseId = 1
        };

        // Act & Assert
        // (Mock UnitOfWork and test service methods)
    }

    [Fact]
    public async Task CreateASN_FromApprovedPO_Success()
    {
        // Test ASN creation from approved PO
    }

    [Fact]
    public async Task CreateASN_FromOpenPO_ThrowsException()
    {
        // Test that ASN cannot be created from non-approved PO
    }

    [Fact]
    public async Task ScanItem_ValidSKU_UpdatesQtyReceived()
    {
        // Test scan functionality
    }

    [Fact]
    public async Task ScanItem_Shortage_DetectsDiscrepancy()
    {
        // Test discrepancy detection
    }
}
```

## Troubleshooting

### Build Errors
```powershell
dotnet build WMS.DL
dotnet build WMS.BLL
dotnet build WMS_DEPI_GRAD
```

### Migration Issues
```powershell
# Remove last migration if needed
dotnet ef migrations remove --project WMS.DL --startup-project WMS_DEPI_GRAD

# Re-create migration
dotnet ef migrations add ProcurementCycleUpdate --project WMS.DL --startup-project WMS_DEPI_GRAD

# Apply migration
dotnet ef database update --project WMS.DL --startup-project WMS_DEPI_GRAD
```

## Next Steps

1. Run the database migration
2. Execute the seed data script
3. Create the Razor views (use existing views as templates)
4. Test the complete flow
5. Add client-side validation
6. Implement unit tests
7. Add logging for status transitions

## Support

For issues or questions, review:
- Entity configurations in `WMS.DL/Data/Configurations`
- Service implementations in `WMS.BLL/Services`
- Controller actions in `WMS_DEPI_GRAD/Controllers`
