# Chronicles of the Omuns - Database Setup Script (PowerShell)
# This script sets up the LocalDB instance, creates migrations, and runs the app

Write-Host "=== Chronicles of the Omuns - Database Setup ===" -ForegroundColor Cyan

# Step 1: Create LocalDB instance
Write-Host "`nStep 1: Creating LocalDB instance..." -ForegroundColor Yellow
sqllocaldb create "ChroniclesDB" -s
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ LocalDB instance created successfully" -ForegroundColor Green
}
else {
    Write-Host "⚠ LocalDB instance may already exist (this is fine)" -ForegroundColor Yellow
}

# Step 2: Navigate to PlayerApp directory
Write-Host "`nStep 2: Navigating to PlayerApp directory..." -ForegroundColor Yellow
$playerAppPath = "c:\Users\thefl\source\repos\TheChroniclesOfTheOmuns\PlayerApp"
Set-Location $playerAppPath
Write-Host "✓ Current directory: $(Get-Location)" -ForegroundColor Green

# Step 3: Create migration
Write-Host "`nStep 3: Creating database migration..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Migration created successfully" -ForegroundColor Green
}
else {
    Write-Host "⚠ Migration may already exist or there was an issue" -ForegroundColor Yellow
}

# Step 4: Apply migration
Write-Host "`nStep 4: Applying migration to database..." -ForegroundColor Yellow
dotnet ef database update
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Database updated successfully" -ForegroundColor Green
}
else {
    Write-Host "✗ Error updating database" -ForegroundColor Red
    exit 1
}

# Step 5: Run the app
Write-Host "`nStep 5: Starting the application..." -ForegroundColor Yellow
dotnet run

Write-Host "`n=== Setup Complete ===" -ForegroundColor Cyan
