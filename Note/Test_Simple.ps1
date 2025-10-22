Write-Host "Testing Categories API (no auth required)..."
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/Categories" -Method GET
    Write-Host "SUCCESS: Categories API works - $($response.Count) categories returned"
    Write-Host "First category: $($response[0] | ConvertTo-Json -Depth 2)"
} catch {
    Write-Host "ERROR: Categories API failed: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response body: $responseBody"
    }
}
