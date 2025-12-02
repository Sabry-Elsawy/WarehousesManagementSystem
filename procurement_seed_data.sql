-- Procurement Cycle Seed Data Script
-- This script creates sample data for testing the complete procurement workflow

-- Insert Vendor
IF NOT EXISTS (SELECT 1 FROM Vendors WHERE Name = 'ACME Supplies')
BEGIN
    INSERT INTO Vendors (Name, CreatedOn, CreatedBy)
    VALUES ('ACME Supplies', GETUTCDATE(), 'System');
END

DECLARE @VendorId INT = (SELECT TOP 1 Id FROM Vendors WHERE Name = 'ACME Supplies');

-- Insert Products (if they don't exist)
IF NOT EXISTS (SELECT 1 FROM Products WHERE Code = 'LAP-001')
BEGIN
    INSERT INTO Products (Name, Code, CreatedOn, CreatedBy)
    VALUES ('Laptop Dell XPS', 'LAP-001', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM Products WHERE Code = 'MON-001')
BEGIN
    INSERT INTO Products (Name, Code, CreatedOn, CreatedBy)
    VALUES ('Monitor Samsung 27"', 'MON-001', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM Products WHERE Code = 'KEY-001')
BEGIN
    INSERT INTO Products (Name, Code, CreatedOn, CreatedBy)
    VALUES ('Keyboard Logitech', 'KEY-001', GETUTCDATE(), 'System');
END

-- Get Product IDs
DECLARE @LaptopId INT = (SELECT Id FROM Products WHERE Code = 'LAP-001');
DECLARE @MonitorId INT = (SELECT Id FROM Products WHERE Code = 'MON-001');
DECLARE @KeyboardId INT = (SELECT Id FROM Products WHERE Code = 'KEY-001');

-- Get a Warehouse ID (assuming one exists)
DECLARE @WarehouseId INT = (SELECT TOP 1 Id FROM Warehouses);

IF @WarehouseId IS NULL
BEGIN
    PRINT 'ERROR: No warehouse found. Please create a warehouse first.';
    RETURN;
END

-- Create Purchase Order
IF NOT EXISTS (SELECT 1 FROM POs WHERE PO_Number = 'PO-2024-001')
BEGIN
    INSERT INTO POs (PO_Number, VendorId, WarehouseId, OrderDate, ExpectedArrivalDate, Status, CreatedOn, CreatedBy)
    VALUES ('PO-2024-001', @VendorId, @WarehouseId, GETUTCDATE(), DATEADD(DAY, 7, GETUTCDATE()), 0, GETUTCDATE(), 'System');
END

DECLARE @POId INT = (SELECT Id FROM POs WHERE PO_Number = 'PO-2024-001');

-- Add PO Items (if they don't exist)
IF NOT EXISTS (SELECT 1 FROM PO_Items WHERE PurchaseOrderId = @POId AND ProductId = @LaptopId)
BEGIN
    INSERT INTO PO_Items (PurchaseOrderId, ProductId, SKU, QtyOrdered, UnitPrice, QtyReceived, LineStatus, CreatedOn, CreatedBy)
    VALUES (@POId, @LaptopId, 'LAP-001', 10, 1200.00, 0, 'Open', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM PO_Items WHERE PurchaseOrderId = @POId AND ProductId = @MonitorId)
BEGIN
    INSERT INTO PO_Items (PurchaseOrderId, ProductId, SKU, QtyOrdered, UnitPrice, QtyReceived, LineStatus, CreatedOn, CreatedBy)
    VALUES (@POId, @MonitorId, 'MON-001', 20, 350.00, 0, 'Open', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM PO_Items WHERE PurchaseOrderId = @POId AND ProductId = @KeyboardId)
BEGIN
    INSERT INTO PO_Items (PurchaseOrderId, ProductId, SKU, QtyOrdered, UnitPrice, QtyReceived, LineStatus, CreatedOn, CreatedBy)
    VALUES (@POId, @KeyboardId, 'KEY-001', 30, 75.00, 0, 'Open', GETUTCDATE(), 'System');
END

-- Approve the PO (Status: 0=Open, 1=Approved, 2=Closed)
UPDATE POs SET Status = 1 WHERE Id = @POId;

-- Create ASN
IF NOT EXISTS (SELECT 1 FROM ASNs WHERE ASN_Number = 'ASN-2024-001')
BEGIN
    INSERT INTO ASNs (ASN_Number, PurchaseOrderId, ExpectedArrivalDate, TrackingNumber, Status, CreatedOn, CreatedBy)
    VALUES ('ASN-2024-001', @POId, DATEADD(DAY, 5, GETUTCDATE()), 'TRACK123456', 0, GETUTCDATE(), 'System');
END

DECLARE @ASNId INT = (SELECT Id FROM ASNs WHERE ASN_Number = 'ASN-2024-001');

-- Get PO Item IDs
DECLARE @POItem1 INT = (SELECT TOP 1 Id FROM PO_Items WHERE PurchaseOrderId = @POId AND ProductId = @LaptopId);
DECLARE @POItem2 INT = (SELECT TOP 1 Id FROM PO_Items WHERE PurchaseOrderId = @POId AND ProductId = @MonitorId);
DECLARE @POItem3 INT = (SELECT TOP 1 Id FROM PO_Items WHERE PurchaseOrderId = @POId AND ProductId = @KeyboardId);

-- Add ASN Items (if they don't exist)
IF NOT EXISTS (SELECT 1 FROM ASN_Items WHERE AdvancedShippingNoticeId = @ASNId AND ProductId = @LaptopId)
BEGIN
    INSERT INTO ASN_Items (AdvancedShippingNoticeId, ProductId, SKU, QtyShipped, LinkedPOItemId, CreatedOn, CreatedBy)
    VALUES (@ASNId, @LaptopId, 'LAP-001', 10, @POItem1, GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM ASN_Items WHERE AdvancedShippingNoticeId = @ASNId AND ProductId = @MonitorId)
BEGIN
    INSERT INTO ASN_Items (AdvancedShippingNoticeId, ProductId, SKU, QtyShipped, LinkedPOItemId, CreatedOn, CreatedBy)
    VALUES (@ASNId, @MonitorId, 'MON-001', 20, @POItem2, GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM ASN_Items WHERE AdvancedShippingNoticeId = @ASNId AND ProductId = @KeyboardId)
BEGIN
    INSERT INTO ASN_Items (AdvancedShippingNoticeId, ProductId, SKU, QtyShipped, LinkedPOItemId, CreatedOn, CreatedBy)
    VALUES (@ASNId, @KeyboardId, 'KEY-001', 30, @POItem3, GETUTCDATE(), 'System');
END

-- Mark ASN as Received (Status: 0=Sent, 1=Received, 2=Closed)
UPDATE ASNs SET Status = 1 WHERE Id = @ASNId;

-- Output results
SELECT 'Seed data created successfully!' AS Result;
SELECT 'PO ID: ' + CAST(@POId AS VARCHAR) + ' - ' + 'PO-2024-001' AS PurchaseOrder;
SELECT 'ASN ID: ' + CAST(@ASNId AS VARCHAR) + ' - ' + 'ASN-2024-001' AS ASN;
SELECT 'Vendor: ACME Supplies' AS Vendor;
SELECT 'Products: 3 (Laptop, Monitor, Keyboard)' AS Products;
SELECT 'Warehouse ID: ' + CAST(@WarehouseId AS VARCHAR) AS Warehouse;
PRINT 'Ready for receipt creation and testing!';
