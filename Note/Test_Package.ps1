Write-Host "Testing Package API (no auth required)..."
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/Package" -Method GET
    Write-Host "SUCCESS: Package API works - $($response.Count) packages returned"
    if ($response.Count -gt 0) {
        Write-Host "First package: $($response[0] | ConvertTo-Json -Depth 2)"
    }
} catch {
    Write-Host "ERROR: Package API failed: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response body: $responseBody"
    }
}
