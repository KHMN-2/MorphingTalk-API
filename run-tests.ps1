# MorphingTalk API Testing Script
# This script automates API testing using Newman (Postman CLI)

param(
    [string]$BaseUrl = "https://localhost:7218",
    [string]$Environment = "test-environment.json",
    [string]$Collection = "MorphingTalk-API-Postman-Collection.json",
    [switch]$SkipInstall,
    [switch]$Verbose
)

Write-Host "🚀 MorphingTalk API Testing Script" -ForegroundColor Green
Write-Host "=" * 60

# Check if Newman is installed
if (-not $SkipInstall) {
    Write-Host "🔍 Checking Newman installation..." -ForegroundColor Yellow
    
    try {
        $newmanVersion = newman --version
        Write-Host "✅ Newman found: $newmanVersion" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Newman not found. Installing..." -ForegroundColor Red
        Write-Host "📦 Installing Newman via npm..." -ForegroundColor Yellow
        
        try {
            npm install -g newman
            npm install -g newman-reporter-html
            Write-Host "✅ Newman installed successfully!" -ForegroundColor Green
        }
        catch {
            Write-Host "❌ Failed to install Newman. Please install Node.js and npm first." -ForegroundColor Red
            Write-Host "   Download from: https://nodejs.org/" -ForegroundColor Yellow
            exit 1
        }
    }
}

# Create test reports directory
if (-not (Test-Path "./test-reports")) {
    New-Item -ItemType Directory -Path "./test-reports" | Out-Null
    Write-Host "📁 Created test-reports directory" -ForegroundColor Green
}

# Create environment file if it doesn't exist
if (-not (Test-Path $Environment)) {
    Write-Host "🔧 Creating test environment file..." -ForegroundColor Yellow
    
    $envContent = @{
        "id" = "morphingtalk-test-env"
        "name" = "MorphingTalk Test Environment"
        "values" = @(
            @{
                "key" = "baseUrl"
                "value" = $BaseUrl
                "enabled" = $true
            },
            @{
                "key" = "authToken"
                "value" = ""
                "enabled" = $true
            },
            @{
                "key" = "userId"
                "value" = ""
                "enabled" = $true
            },
            @{
                "key" = "conversationId"
                "value" = ""
                "enabled" = $true
            },
            @{
                "key" = "messageId"
                "value" = ""
                "enabled" = $true
            },
            @{
                "key" = "testEmail"
                "value" = "test@example.com"
                "enabled" = $true
            },
            @{
                "key" = "testPassword"
                "value" = "TestPassword123!"
                "enabled" = $true
            },
            @{
                "key" = "friendEmail"
                "value" = "friend@example.com"
                "enabled" = $true
            }
        )
    }
    
    $envContent | ConvertTo-Json -Depth 10 | Out-File -FilePath $Environment -Encoding UTF8
    Write-Host "✅ Environment file created: $Environment" -ForegroundColor Green
}

# Check if collection file exists
if (-not (Test-Path $Collection)) {
    Write-Host "❌ Collection file not found: $Collection" -ForegroundColor Red
    Write-Host "   Please ensure the Postman collection file is in the current directory." -ForegroundColor Yellow
    exit 1
}

# Display pre-test information
Write-Host ""
Write-Host "🎯 Test Configuration:" -ForegroundColor Cyan
Write-Host "   Collection: $Collection" -ForegroundColor White
Write-Host "   Environment: $Environment" -ForegroundColor White
Write-Host "   Base URL: $BaseUrl" -ForegroundColor White
Write-Host "   Reports Directory: ./test-reports" -ForegroundColor White
Write-Host ""

# Ask for confirmation before running tests
$confirmation = Read-Host "Ready to run tests? (y/N)"
if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
    Write-Host "❌ Tests cancelled by user." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "🔥 Starting API Tests..." -ForegroundColor Green
Write-Host "=" * 60

# Prepare Newman command
$newmanArgs = @(
    "run", $Collection,
    "--environment", $Environment,
    "--reporters", "cli,json,html",
    "--reporter-html-export", "./test-reports/html-report.html",
    "--reporter-json-export", "./test-reports/json-report.json",
    "--insecure",
    "--timeout", "30000",
    "--color", "on"
)

if ($Verbose) {
    $newmanArgs += "--verbose"
}

# Run Newman
try {
    & newman $newmanArgs
    $exitCode = $LASTEXITCODE
    
    Write-Host ""
    Write-Host "📊 Test Execution Complete!" -ForegroundColor Green
    Write-Host "=" * 60
    
    # Generate additional reports
    Write-Host "📄 Generating additional reports..." -ForegroundColor Yellow
    
    # Check if reports were generated
    if (Test-Path "./test-reports/json-report.json") {
        Write-Host "✅ JSON report generated successfully" -ForegroundColor Green
    }
    
    if (Test-Path "./test-reports/html-report.html") {
        Write-Host "✅ HTML report generated successfully" -ForegroundColor Green
        
        # Ask if user wants to open HTML report
        $openReport = Read-Host "Open HTML report in browser? (y/N)"
        if ($openReport -eq 'y' -or $openReport -eq 'Y') {
            Start-Process "./test-reports/html-report.html"
        }
    }
    
    # Display summary
    Write-Host ""
    Write-Host "📋 Test Summary:" -ForegroundColor Cyan
    
    if ($exitCode -eq 0) {
        Write-Host "🎉 All tests passed successfully!" -ForegroundColor Green
        Write-Host "   Your API is functioning correctly." -ForegroundColor Green
    } else {
        Write-Host "⚠️  Some tests failed." -ForegroundColor Yellow
        Write-Host "   Please check the reports for detailed information." -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "📄 Generated Reports:" -ForegroundColor Cyan
    Write-Host "   📊 HTML Report: $(Resolve-Path './test-reports/html-report.html')" -ForegroundColor White
    Write-Host "   📋 JSON Report: $(Resolve-Path './test-reports/json-report.json')" -ForegroundColor White
    
    # Performance summary
    Write-Host ""
    Write-Host "🏁 Testing completed at $(Get-Date)" -ForegroundColor Green
    
    exit $exitCode
    
} catch {
    Write-Host "❌ Error running tests: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Function to display help
function Show-Help {
    Write-Host "MorphingTalk API Testing Script Usage:" -ForegroundColor Green
    Write-Host ""
    Write-Host "Parameters:" -ForegroundColor Cyan
    Write-Host "  -BaseUrl <string>     API base URL (default: https://localhost:7218)" -ForegroundColor White
    Write-Host "  -Environment <string> Environment file (default: test-environment.json)" -ForegroundColor White
    Write-Host "  -Collection <string>  Collection file (default: MorphingTalk-API-Postman-Collection.json)" -ForegroundColor White
    Write-Host "  -SkipInstall          Skip Newman installation check" -ForegroundColor White
    Write-Host "  -Verbose              Enable verbose output" -ForegroundColor White
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Cyan
    Write-Host "  ./run-tests.ps1" -ForegroundColor White
    Write-Host "  ./run-tests.ps1 -BaseUrl 'http://localhost:5000'" -ForegroundColor White
    Write-Host "  ./run-tests.ps1 -Verbose" -ForegroundColor White
    Write-Host ""
} 