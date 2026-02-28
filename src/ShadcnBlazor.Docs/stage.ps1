# Local staging script - mirrors the GitHub Actions deployment workflow
# Usage: .\stage.ps1
# Then visit: http://localhost:3000/ShadcnBlazor/

$ErrorActionPreference = "Stop"
$DocsDir = $PSScriptRoot

# Assert local-server.js exists
$ServerScript = Join-Path $DocsDir "local-server.js"
if (-not (Test-Path $ServerScript)) {
    Write-Error "local-server.js not found at $ServerScript"
    exit 1
}
Write-Host "✓ local-server.js found" -ForegroundColor Green

# Assert node is available
try {
    $nodeVersion = node --version
    Write-Host "✓ node $nodeVersion found" -ForegroundColor Green
} catch {
    Write-Error "node is not installed or not in PATH"
    exit 1
}

# Clean previous publish
$PublishDir = Join-Path $DocsDir "publish"
if (Test-Path $PublishDir) {
    Write-Host "Cleaning previous publish..." -ForegroundColor Yellow
    Remove-Item $PublishDir -Recurse -Force
}

# Restore
Write-Host "`nRestoring packages..." -ForegroundColor Cyan
dotnet restore (Join-Path $DocsDir "ShadcnBlazor.Docs.csproj")
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Publish
Write-Host "`nPublishing..." -ForegroundColor Cyan
dotnet publish (Join-Path $DocsDir "ShadcnBlazor.Docs.csproj") -c Release -o (Join-Path $DocsDir "publish")
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Fix base href (mirrors GH Actions step, no-op if already correct)
$IndexHtml = Join-Path $DocsDir "publish\wwwroot\index.html"
if (Test-Path $IndexHtml) {
    $content = Get-Content $IndexHtml -Raw
    $fixed = $content -replace '<base href="/" />', '<base href="/ShadcnBlazor/" />'
    Set-Content $IndexHtml $fixed -NoNewline
    Write-Host "✓ base href checked/fixed" -ForegroundColor Green
} else {
    Write-Error "index.html not found in publish output"
    exit 1
}

# Copy index.html → 404.html (mirrors GH Actions step, enables SPA routing)
$NotFoundHtml = Join-Path $DocsDir "publish\wwwroot\404.html"
Copy-Item $IndexHtml $NotFoundHtml -Force
Write-Host "✓ 404.html created" -ForegroundColor Green

# Start local server
Write-Host "`n🚀 Starting local staging server..." -ForegroundColor Cyan
Set-Location $DocsDir
node $ServerScript
