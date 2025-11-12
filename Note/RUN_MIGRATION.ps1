# PowerShell script to run Wishlist migration
# This script will execute MIGRATE_Add_StorageId_To_Wishlists.sql

param(
    [string]$ServerName = "(local)",
    [string]$DatabaseName = "EdupromptV2",
    [string]$UserId = "sa",
    [string]$Password = "123456"
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Running Wishlist Migration Script" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Get the script directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$sqlScriptPath = Join-Path $scriptPath "MIGRATE_Add_StorageId_To_Wishlists.sql"

if (-not (Test-Path $sqlScriptPath)) {
    Write-Host "ERROR: SQL script not found at: $sqlScriptPath" -ForegroundColor Red
    exit 1
}

Write-Host "SQL Script: $sqlScriptPath" -ForegroundColor Yellow
Write-Host "Server: $ServerName" -ForegroundColor Yellow
Write-Host "Database: $DatabaseName" -ForegroundColor Yellow
Write-Host ""

# Check if sqlcmd is available
$sqlcmdPath = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (-not $sqlcmdPath) {
    Write-Host "ERROR: sqlcmd not found. Please install SQL Server Command Line Utilities." -ForegroundColor Red
    Write-Host "Or run the script manually in SSMS." -ForegroundColor Yellow
    exit 1
}

# Build connection string
$connectionString = "-S $ServerName -d $DatabaseName -U $UserId -P $Password"

Write-Host "Executing migration script..." -ForegroundColor Green
Write-Host ""

try {
    # Run the SQL script
    $result = & sqlcmd $connectionString -i $sqlScriptPath -b
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "================================================" -ForegroundColor Green
        Write-Host "Migration completed successfully!" -ForegroundColor Green
        Write-Host "================================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Next steps:" -ForegroundColor Yellow
        Write-Host "1. Restart your API (dotnet run)" -ForegroundColor Yellow
        Write-Host "2. Test the wishlist endpoint" -ForegroundColor Yellow
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "ERROR: Migration failed with exit code $LASTEXITCODE" -ForegroundColor Red
        Write-Host "Please check the error messages above." -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host ""
    Write-Host "ERROR: Failed to execute migration script" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative: Run the script manually in SSMS:" -ForegroundColor Yellow
    Write-Host "  File: $sqlScriptPath" -ForegroundColor Yellow
    exit 1
}

