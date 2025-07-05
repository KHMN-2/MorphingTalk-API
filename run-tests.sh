#!/bin/bash

# MorphingTalk API Testing Script for Linux/Mac
# This script automates API testing using Newman (Postman CLI)

set -e

# Default values
BASE_URL="https://localhost:7218"
ENVIRONMENT="test-environment.json"
COLLECTION="MorphingTalk-API-Postman-Collection.json"
SKIP_INSTALL=false
VERBOSE=false
HELP=false

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
WHITE='\033[1;37m'
NC='\033[0m' # No Color

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -u|--base-url)
            BASE_URL="$2"
            shift 2
            ;;
        -e|--environment)
            ENVIRONMENT="$2"
            shift 2
            ;;
        -c|--collection)
            COLLECTION="$2"
            shift 2
            ;;
        -s|--skip-install)
            SKIP_INSTALL=true
            shift
            ;;
        -v|--verbose)
            VERBOSE=true
            shift
            ;;
        -h|--help)
            HELP=true
            shift
            ;;
        *)
            echo "Unknown option $1"
            exit 1
            ;;
    esac
done

# Help function
show_help() {
    echo -e "${GREEN}MorphingTalk API Testing Script Usage:${NC}"
    echo ""
    echo -e "${CYAN}Options:${NC}"
    echo -e "  ${WHITE}-u, --base-url <string>${NC}     API base URL (default: https://localhost:7218)"
    echo -e "  ${WHITE}-e, --environment <string>${NC}  Environment file (default: test-environment.json)"
    echo -e "  ${WHITE}-c, --collection <string>${NC}   Collection file (default: MorphingTalk-API-Postman-Collection.json)"
    echo -e "  ${WHITE}-s, --skip-install${NC}          Skip Newman installation check"
    echo -e "  ${WHITE}-v, --verbose${NC}               Enable verbose output"
    echo -e "  ${WHITE}-h, --help${NC}                  Show this help message"
    echo ""
    echo -e "${CYAN}Examples:${NC}"
    echo -e "  ${WHITE}./run-tests.sh${NC}"
    echo -e "  ${WHITE}./run-tests.sh -u 'http://localhost:5000'${NC}"
    echo -e "  ${WHITE}./run-tests.sh --verbose${NC}"
    echo -e "  ${WHITE}./run-tests.sh --skip-install${NC}"
    echo ""
}

# Show help if requested
if [ "$HELP" = true ]; then
    show_help
    exit 0
fi

echo -e "${GREEN}🚀 MorphingTalk API Testing Script${NC}"
echo "============================================================"

# Check if Newman is installed
if [ "$SKIP_INSTALL" = false ]; then
    echo -e "${YELLOW}🔍 Checking Newman installation...${NC}"
    
    if command -v newman &> /dev/null; then
        NEWMAN_VERSION=$(newman --version)
        echo -e "${GREEN}✅ Newman found: $NEWMAN_VERSION${NC}"
    else
        echo -e "${RED}❌ Newman not found. Installing...${NC}"
        
        # Check if npm is installed
        if ! command -v npm &> /dev/null; then
            echo -e "${RED}❌ npm not found. Please install Node.js and npm first.${NC}"
            echo -e "${YELLOW}   Download from: https://nodejs.org/${NC}"
            exit 1
        fi
        
        echo -e "${YELLOW}📦 Installing Newman via npm...${NC}"
        npm install -g newman
        npm install -g newman-reporter-html
        echo -e "${GREEN}✅ Newman installed successfully!${NC}"
    fi
fi

# Create test reports directory
if [ ! -d "./test-reports" ]; then
    mkdir -p "./test-reports"
    echo -e "${GREEN}📁 Created test-reports directory${NC}"
fi

# Create environment file if it doesn't exist
if [ ! -f "$ENVIRONMENT" ]; then
    echo -e "${YELLOW}🔧 Creating test environment file...${NC}"
    
    cat > "$ENVIRONMENT" << EOF
{
  "id": "morphingtalk-test-env",
  "name": "MorphingTalk Test Environment",
  "values": [
    {
      "key": "baseUrl",
      "value": "$BASE_URL",
      "enabled": true
    },
    {
      "key": "authToken",
      "value": "",
      "enabled": true
    },
    {
      "key": "userId",
      "value": "",
      "enabled": true
    },
    {
      "key": "conversationId",
      "value": "",
      "enabled": true
    },
    {
      "key": "messageId",
      "value": "",
      "enabled": true
    },
    {
      "key": "testEmail",
      "value": "test@example.com",
      "enabled": true
    },
    {
      "key": "testPassword",
      "value": "TestPassword123!",
      "enabled": true
    },
    {
      "key": "friendEmail",
      "value": "friend@example.com",
      "enabled": true
    }
  ]
}
EOF
    echo -e "${GREEN}✅ Environment file created: $ENVIRONMENT${NC}"
fi

# Check if collection file exists
if [ ! -f "$COLLECTION" ]; then
    echo -e "${RED}❌ Collection file not found: $COLLECTION${NC}"
    echo -e "${YELLOW}   Please ensure the Postman collection file is in the current directory.${NC}"
    exit 1
fi

# Display pre-test information
echo ""
echo -e "${CYAN}🎯 Test Configuration:${NC}"
echo -e "   Collection: ${WHITE}$COLLECTION${NC}"
echo -e "   Environment: ${WHITE}$ENVIRONMENT${NC}"
echo -e "   Base URL: ${WHITE}$BASE_URL${NC}"
echo -e "   Reports Directory: ${WHITE}./test-reports${NC}"
echo ""

# Ask for confirmation before running tests
echo -e "${YELLOW}Ready to run tests? (y/N)${NC}"
read -r confirmation
if [[ ! "$confirmation" =~ ^[Yy]$ ]]; then
    echo -e "${YELLOW}❌ Tests cancelled by user.${NC}"
    exit 0
fi

echo ""
echo -e "${GREEN}🔥 Starting API Tests...${NC}"
echo "============================================================"

# Prepare Newman command
NEWMAN_CMD="newman run $COLLECTION --environment $ENVIRONMENT --reporters cli,json,html --reporter-html-export ./test-reports/html-report.html --reporter-json-export ./test-reports/json-report.json --insecure --timeout 30000 --color on"

if [ "$VERBOSE" = true ]; then
    NEWMAN_CMD="$NEWMAN_CMD --verbose"
fi

# Run Newman
echo -e "${BLUE}Running command: $NEWMAN_CMD${NC}"
echo ""

if eval "$NEWMAN_CMD"; then
    EXIT_CODE=0
else
    EXIT_CODE=$?
fi

echo ""
echo -e "${GREEN}📊 Test Execution Complete!${NC}"
echo "============================================================"

# Generate additional reports
echo -e "${YELLOW}📄 Generating additional reports...${NC}"

# Check if reports were generated
if [ -f "./test-reports/json-report.json" ]; then
    echo -e "${GREEN}✅ JSON report generated successfully${NC}"
fi

if [ -f "./test-reports/html-report.html" ]; then
    echo -e "${GREEN}✅ HTML report generated successfully${NC}"
    
    # Ask if user wants to open HTML report
    echo -e "${YELLOW}Open HTML report in browser? (y/N)${NC}"
    read -r open_report
    if [[ "$open_report" =~ ^[Yy]$ ]]; then
        # Try to open with different browsers
        if command -v xdg-open &> /dev/null; then
            xdg-open "./test-reports/html-report.html"
        elif command -v open &> /dev/null; then
            open "./test-reports/html-report.html"
        else
            echo -e "${YELLOW}   Please open ./test-reports/html-report.html manually${NC}"
        fi
    fi
fi

# Display summary
echo ""
echo -e "${CYAN}📋 Test Summary:${NC}"

if [ $EXIT_CODE -eq 0 ]; then
    echo -e "${GREEN}🎉 All tests passed successfully!${NC}"
    echo -e "${GREEN}   Your API is functioning correctly.${NC}"
else
    echo -e "${YELLOW}⚠️  Some tests failed.${NC}"
    echo -e "${YELLOW}   Please check the reports for detailed information.${NC}"
fi

echo ""
echo -e "${CYAN}📄 Generated Reports:${NC}"
echo -e "   📊 HTML Report: ${WHITE}$(pwd)/test-reports/html-report.html${NC}"
echo -e "   📋 JSON Report: ${WHITE}$(pwd)/test-reports/json-report.json${NC}"

# Performance summary
echo ""
echo -e "${GREEN}🏁 Testing completed at $(date)${NC}"

# Create a simple feature status report
if [ -f "./test-reports/json-report.json" ]; then
    echo -e "${YELLOW}📑 Generating feature status report...${NC}"
    
    # Extract basic stats from JSON report
    TOTAL_TESTS=$(jq '.run.stats.tests.total' "./test-reports/json-report.json" 2>/dev/null || echo "0")
    PASSED_TESTS=$(jq '.run.stats.tests.passed' "./test-reports/json-report.json" 2>/dev/null || echo "0")
    FAILED_TESTS=$(jq '.run.stats.tests.failed' "./test-reports/json-report.json" 2>/dev/null || echo "0")
    
    cat > "./test-reports/feature-status.md" << EOF
# MorphingTalk API Feature Status Report

**Generated:** $(date)

## Overall Summary

- **Total Tests:** $TOTAL_TESTS
- **Passed:** $PASSED_TESTS
- **Failed:** $FAILED_TESTS
- **Success Rate:** $(echo "scale=2; $PASSED_TESTS * 100 / $TOTAL_TESTS" | bc -l 2>/dev/null || echo "N/A")%

## Test Results

$(if [ $EXIT_CODE -eq 0 ]; then echo "✅ All tests passed! API is functioning correctly."; else echo "⚠️ Some tests failed. Please review the detailed reports."; fi)

## Reports Location

- HTML Report: ./test-reports/html-report.html
- JSON Report: ./test-reports/json-report.json
- Feature Status: ./test-reports/feature-status.md

## Next Steps

$(if [ $EXIT_CODE -ne 0 ]; then echo "1. Review failed tests in the HTML report
2. Check API server status and connectivity
3. Verify database connections
4. Ensure required services are running"; else echo "🎉 All systems are working correctly!"; fi)
EOF
    
    echo -e "${GREEN}✅ Feature status report generated${NC}"
fi

echo ""
echo -e "${CYAN}📄 All Reports:${NC}"
echo -e "   📊 HTML Report: ${WHITE}$(pwd)/test-reports/html-report.html${NC}"
echo -e "   📋 JSON Report: ${WHITE}$(pwd)/test-reports/json-report.json${NC}"
echo -e "   📑 Feature Status: ${WHITE}$(pwd)/test-reports/feature-status.md${NC}"

exit $EXIT_CODE 