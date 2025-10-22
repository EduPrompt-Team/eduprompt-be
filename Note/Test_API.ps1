Write-Host "Testing API endpoints..."

Write-Host "Testing PaymentMethod API..."
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/PaymentMethod/user/1" -Method GET
    Write-Host "PaymentMethod API works: $($response.Count) items returned"
} catch {
    Write-Host "PaymentMethod API failed: $($_.Exception.Message)"
}

Write-Host "Testing Transaction API..."
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/Transaction/user/1" -Method GET
    Write-Host "Transaction API works: $($response.Count) items returned"
} catch {
    Write-Host "Transaction API failed: $($_.Exception.Message)"
}

Write-Host "API testing completed."
