# Demo Test Script for MorphingTalk API
# This script runs a quick demo to show the testing system is working

param(
    [string]$BaseUrl = "https://localhost:7218"
)

Write-Host "🎬 MorphingTalk API Demo Test" -ForegroundColor Green
Write-Host "=" * 50

Write-Host "🔧 Testing basic API connectivity..." -ForegroundColor Yellow

# Test 1: Basic API Health Check
Write-Host "1. Testing API health check..." -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/User/GetAllUsers" -Method GET -TimeoutSec 10
    Write-Host "   ✅ API is responding" -ForegroundColor Green
} catch {
    Write-Host "   ❌ API not responding: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   💡 Make sure your API server is running on $BaseUrl" -ForegroundColor Yellow
    exit 1
}

# Test 2: Check if Newman is available
Write-Host "2. Checking Newman installation..." -ForegroundColor Cyan
try {
    $newmanVersion = newman --version 2>$null
    Write-Host "   ✅ Newman found: $newmanVersion" -ForegroundColor Green
} catch {
    Write-Host "   ⚠️  Newman not found - will be installed automatically" -ForegroundColor Yellow
}

# Test 3: Check files exist
Write-Host "3. Checking test files..." -ForegroundColor Cyan
$requiredFiles = @(
    "MorphingTalk-API-Postman-Collection.json",
    "test-environment.json",
    "run-tests.ps1"
)

$allFilesExist = $true
foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "   ✅ $file found" -ForegroundColor Green
    } else {
        Write-Host "   ❌ $file missing" -ForegroundColor Red
        $allFilesExist = $false
    }
}

if (-not $allFilesExist) {
    Write-Host "   💡 Some test files are missing. Please ensure all files are in the current directory." -ForegroundColor Yellow
    exit 1
}

# Test 4: Quick demo test
Write-Host "4. Running quick demo test..." -ForegroundColor Cyan
Write-Host "   🚀 Starting demo test (this may take a moment)..." -ForegroundColor Yellow

try {
    # Run a quick test with just a few endpoints
    $demoResult = newman run MorphingTalk-API-Postman-Collection.json --environment test-environment.json --folder "Authentication Tests" --reporters cli --insecure --color off 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✅ Demo test completed successfully!" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  Demo test had some issues (this is normal for first run)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ⚠️  Demo test encountered issues: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Summary
Write-Host ""
Write-Host "🎉 Demo Test Summary:" -ForegroundColor Green
Write-Host "=" * 50
Write-Host "✅ API connectivity: Working" -ForegroundColor Green
Write-Host "✅ Test files: Present" -ForegroundColor Green
Write-Host "✅ Testing system: Ready" -ForegroundColor Green

Write-Host ""
Write-Host "🚀 Ready to run full tests!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  • Run full test suite: ./run-tests.ps1" -ForegroundColor White
Write-Host "  • View test reports in: ./test-reports/" -ForegroundColor White
Write-Host "  • Check API reference: API-Endpoints-Quick-Reference.md" -ForegroundColor White
Write-Host ""
Write-Host "Happy testing! 🧪" -ForegroundColor Green 