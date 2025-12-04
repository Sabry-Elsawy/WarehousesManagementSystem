# Database Migration Helper Script
# Run this script to apply the AddAddressFields migration

Write-Host "WMS Identity Enhancements - Database Migration" -ForegroundColor Cyan
Write-Host "============================================`n" -ForegroundColor Cyan

# Navigate to WMS_DEPI_GRAD project directory
$projectDir = "d:\DEPI\GRD_PR\WMS\WarehousesManagementSystem\WMS_DEPI_GRAD"

if (Test-Path $projectDir) {
    Set-Location $projectDir
    Write-Host "✓ Navigated to project directory" -ForegroundColor Green
} else {
    Write-Host "✗ Project directory not found: $projectDir" -ForegroundColor Red
    exit 1
}

Write-Host "`nStep 1: Creating migration 'AddAddressFields'..." -ForegroundColor Yellow
Write-Host "This will add State, PostalCode, Phone, and Company columns to the Addresses table.`n"

try {
    $output = dotnet ef migrations add AddAddressFields --project ..\WMS.DL\WMS.DAL.csproj --startup-project .\WMS_DEPI_GRAD.csproj 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Migration created successfully!" -ForegroundColor Green
        Write-Host $output
    } else {
        Write-Host "✗ Migration creation failed" -ForegroundColor Red
        Write-Host $output
        Write-Host "`nNote: If migration already exists, proceed to Step 2." -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ Error creating migration: $_" -ForegroundColor Red
}

Write-Host "`n`nStep 2: Applying migration to database..." -ForegroundColor Yellow
Write-Host "This will update your database schema.`n"

$confirm = Read-Host "Do you want to apply the migration now? (Y/N)"

if ($confirm -eq 'Y' -or $confirm -eq 'y') {
    try {
        $output = dotnet ef database update --project ..\WMS.DL\WMS.DAL.csproj --startup-project .\WMS_DEPI_GRAD.csproj 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Database updated successfully!" -ForegroundColor Green
            Write-Host $output
            Write-Host "`n✓ Migration complete! The Addresses table now has State, PostalCode, Phone, and Company columns." -ForegroundColor Green
        } else {
            Write-Host "✗ Database update failed" -ForegroundColor Red
            Write-Host $output
        }
    } catch {
        Write-Host "✗ Error updating database: $_" -ForegroundColor Red
    }
} else {
    Write-Host "`nMigration not applied. Run the following command when ready:" -ForegroundColor Yellow
    Write-Host "dotnet ef database update --project ..\WMS.DL\WMS.DAL.csproj --startup-project .\WMS_DEPI_GRAD.csproj" -ForegroundColor Cyan
}

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "Migration script completed." -ForegroundColor Cyan
Write-Host "See walkthrough.md for testing instructions." -ForegroundColor Cyan
