# Publish the Blazor WASM app to bin/publish
$publishPath = "bin\publish"

Write-Host "Cleaning old publish directory..." -ForegroundColor Yellow
if (Test-Path $publishPath) {
    Remove-Item -Recurse -Force $publishPath
    Write-Host "Removed $publishPath" -ForegroundColor Green
}

Write-Host "Publishing to $publishPath..." -ForegroundColor Cyan
dotnet clean
dotnet publish -c Release -o $publishPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Publish complete!" -ForegroundColor Green

# Serve the published wwwroot folder
$wwwrootPath = "$publishPath\wwwroot"

if (-not (Test-Path $wwwrootPath)) {
    Write-Host "Published wwwroot not found at $wwwrootPath" -ForegroundColor Red
    exit 1
}

Write-Host "Serving from: $wwwrootPath" -ForegroundColor Cyan
Write-Host "Starting HTTP server on http://localhost:8000" -ForegroundColor Cyan

# Try Python first
try {
    python -m http.server 8000 --directory $wwwrootPath
}
catch {
    # Fallback to npx http-server
    Write-Host "Python not available, trying npx http-server..." -ForegroundColor Yellow
    npx http-server $wwwrootPath -p 8000 -g
}
