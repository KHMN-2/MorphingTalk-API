# MorphingTalk API Automated Testing Guide

## 🎯 Overview

This guide provides comprehensive automated testing for all MorphingTalk API features using Newman (Postman CLI) and custom test scripts. The testing suite validates API functionality, performance, and feature availability.

## 📋 Features Tested

- ✅ **Authentication System** - Register, login, OTP verification, password reset
- ✅ **User Management** - Profile operations, user data retrieval
- ✅ **Conversation Management** - Create, update, manage conversations
- ✅ **Messaging System** - Text, voice, image messages with translation
- ✅ **Friendship Features** - Add/remove friends, blocking system
- ✅ **Voice Training** - AI voice model training and management
- ✅ **File Management** - Upload/download various file types
- ✅ **Call Management** - Voice/video call initiation and control
- ✅ **System Integration** - Firebase, webhooks, external services

## 🚀 Quick Start

### Prerequisites

1. **Node.js & npm** (v14+ recommended)
   - Download from: https://nodejs.org/
   
2. **Your API Server Running**
   - Ensure MorphingTalk API is running on `https://localhost:7218`
   - Or update the base URL in environment files

### Option 1: Automated Setup (Recommended)

#### Windows (PowerShell)
```powershell
# Run the PowerShell script
./run-tests.ps1

# With custom base URL
./run-tests.ps1 -BaseUrl "http://localhost:5000"

# With verbose output
./run-tests.ps1 -Verbose
```

#### Linux/Mac (Bash)
```bash
# Make script executable
chmod +x run-tests.sh

# Run the bash script
./run-tests.sh

# With custom base URL
./run-tests.sh -u "http://localhost:5000"

# With verbose output
./run-tests.sh --verbose
```

#### Node.js (Cross-platform)
```bash
# Install dependencies
npm install

# Run comprehensive tests
npm test

# Quick test run
npm run test:quick
```

### Option 2: Manual Setup

1. **Install Newman**
```bash
npm install -g newman newman-reporter-html
```

2. **Run Tests**
```bash
newman run MorphingTalk-API-Postman-Collection.json \
  --environment test-environment.json \
  --reporters cli,json,html \
  --reporter-html-export ./test-reports/html-report.html \
  --reporter-json-export ./test-reports/json-report.json \
  --insecure
```

## 📁 File Structure

```
MorphingTalk-API-Tests/
├── 📄 MorphingTalk-API-Postman-Collection.json    # Main test collection
├── 📄 test-environment.json                       # Environment variables
├── 📄 package.json                               # Node.js dependencies
├── 📄 run-api-tests.js                          # Node.js test runner
├── 📄 run-tests.ps1                             # PowerShell script
├── 📄 run-tests.sh                              # Bash script
├── 📄 Automated-Testing-Guide.md               # This guide
├── 📄 Postman-Collection-Guide.md              # Manual testing guide
├── 📄 API-Endpoints-Quick-Reference.md         # API reference
└── 📁 test-reports/                            # Generated reports
    ├── 📊 html-report.html                     # Visual test report
    ├── 📋 json-report.json                     # Detailed JSON results
    ├── 📝 detailed-results.json                # Custom analysis
    └── 📑 feature-status.md                    # Feature status summary
```

## 🔧 Configuration

### Environment Variables

Edit `test-environment.json` to customize test settings:

```json
{
  "baseUrl": "https://localhost:7218",    // Your API URL
  "testEmail": "test@example.com",        // Test user email
  "testPassword": "TestPassword123!",     // Test user password
  "friendEmail": "friend@example.com"     // Friend test email
}
```

### Script Parameters

#### PowerShell Script (`run-tests.ps1`)
- `-BaseUrl` - API base URL
- `-Environment` - Environment file path
- `-Collection` - Collection file path
- `-SkipInstall` - Skip Newman installation check
- `-Verbose` - Enable detailed output

#### Bash Script (`run-tests.sh`)
- `-u, --base-url` - API base URL
- `-e, --environment` - Environment file path
- `-c, --collection` - Collection file path
- `-s, --skip-install` - Skip Newman installation check
- `-v, --verbose` - Enable detailed output
- `-h, --help` - Show help message

## 📊 Test Reports

### HTML Report (`test-reports/html-report.html`)
- 📈 Visual test results with charts
- 🔍 Detailed request/response data
- ⏱️ Performance metrics
- 🎨 Color-coded pass/fail status

### JSON Report (`test-reports/json-report.json`)
- 📋 Raw test data for analysis
- 🔢 Statistical information
- 🕒 Timing details
- 🔧 Machine-readable format

### Feature Status (`test-reports/feature-status.md`)
- ✅ Feature-by-feature status
- 📊 Success rates by category
- 🎯 Recommendations for fixes
- 📈 Overall health summary

## 🧪 Test Categories

### 1. Authentication Tests
```javascript
// Test Coverage:
- User Registration ✅
- User Login ✅
- OTP Verification ✅
- Password Reset ✅
- Account Checking ✅
- Firebase Integration ✅
```

### 2. User Management Tests
```javascript
// Test Coverage:
- Get User Profile ✅
- Update User Data ✅
- User List Retrieval ✅
- Profile Picture Upload ✅
- User Deletion ✅
```

### 3. Conversation Tests
```javascript
// Test Coverage:
- Create Conversations ✅
- List Conversations ✅
- Update Conversations ✅
- Add/Remove Participants ✅
- Conversation Deletion ✅
```

### 4. Messaging Tests
```javascript
// Test Coverage:
- Send Text Messages ✅
- Send Voice Messages ✅
- Send Image Messages ✅
- Message Starring ✅
- Message Deletion ✅
- Reply Messages ✅
- Translation Features ✅
```

### 5. Friendship Tests
```javascript
// Test Coverage:
- Add Friends ✅
- Remove Friends ✅
- Block Users ✅
- Unblock Users ✅
- Friends List ✅
- Blocked Users List ✅
```

### 6. Voice Training Tests
```javascript
// Test Coverage:
- Voice Model Training ✅
- Training Status ✅
- Model Deletion ✅
- AI Integration ✅
```

### 7. File Management Tests
```javascript
// Test Coverage:
- Document Upload ✅
- Image Upload ✅
- Audio Upload ✅
- Video Upload ✅
- File Deletion ✅
```

### 8. System Tests
```javascript
// Test Coverage:
- Firebase Setup ✅
- Webhook Processing ✅
- API Health Check ✅
- External Services ✅
```

## 🎭 Test Scenarios

### Scenario 1: Complete User Journey
1. **Register** → **Login** → **Update Profile**
2. **Add Friends** → **Create Conversation**
3. **Send Messages** → **Test Features**
4. **Upload Files** → **Train Voice**

### Scenario 2: API Reliability
1. **Authentication Flow**
2. **Data Persistence**
3. **Error Handling**
4. **Performance Metrics**

### Scenario 3: Feature Integration
1. **Cross-feature Dependencies**
2. **Real-time Updates**
3. **External Service Integration**
4. **Security Validation**

## 🔍 Test Validation

Each test validates:
- ✅ **Status Codes** - Correct HTTP responses
- ✅ **Response Structure** - Expected JSON format
- ✅ **Data Types** - Proper field types
- ✅ **Business Logic** - Feature-specific rules
- ✅ **Performance** - Response time limits
- ✅ **Security** - Authentication requirements

## 📈 Success Criteria

### Pass Criteria
- ✅ Status code 200/201 for success operations
- ✅ Proper JSON response structure
- ✅ Expected data in response body
- ✅ Response time under thresholds
- ✅ Authentication working correctly

### Fail Criteria
- ❌ 4xx/5xx status codes (unless expected)
- ❌ Missing required response fields
- ❌ Authentication failures
- ❌ Timeout errors
- ❌ Unexpected response format

## 🚨 Troubleshooting

### Common Issues

#### 1. Newman Not Found
```bash
# Solution: Install Newman globally
npm install -g newman newman-reporter-html
```

#### 2. API Server Not Running
```bash
# Check if API is running
curl https://localhost:7218/api/health

# Or test with browser
https://localhost:7218/api/User/GetAllUsers
```

#### 3. SSL Certificate Errors
```bash
# Run with --insecure flag (for development)
newman run collection.json --insecure
```

#### 4. Authentication Failures
- Check test email/password in environment
- Verify user registration completed
- Ensure token is stored correctly

#### 5. Database Connection Issues
- Verify database is running
- Check connection strings
- Ensure migrations are applied

### Debug Mode

#### Enable Verbose Output
```bash
# PowerShell
./run-tests.ps1 -Verbose

# Bash
./run-tests.sh --verbose

# Newman direct
newman run collection.json --verbose
```

#### Check Specific Endpoint
```bash
# Test single endpoint
newman run collection.json --folder "Authentication Tests"
```

## 📅 Continuous Integration

### GitHub Actions
```yaml
name: API Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-node@v2
        with:
          node-version: '16'
      - run: npm install -g newman
      - run: ./run-tests.sh
```

### Azure DevOps
```yaml
- task: Npm@1
  inputs:
    command: 'custom'
    customCommand: 'install -g newman newman-reporter-html'
- script: './run-tests.sh'
  displayName: 'Run API Tests'
```

## 📞 Support

### Getting Help
1. Check test reports for detailed error messages
2. Review API endpoint documentation
3. Verify server logs for backend issues
4. Test manually with Postman first

### Common Commands
```bash
# View help
./run-tests.sh --help
./run-tests.ps1 -?

# Test specific feature
newman run collection.json --folder "Authentication Tests"

# Generate reports only
newman run collection.json --reporters html,json

# Test with custom environment
newman run collection.json --environment custom-env.json
```

## 🎯 Best Practices

1. **Run Tests Regularly** - Before deployments
2. **Update Test Data** - Keep emails/passwords current
3. **Monitor Performance** - Track response times
4. **Review Failures** - Investigate failed tests immediately
5. **Maintain Environment** - Update URLs and credentials

## 🔄 Maintenance

### Updating Tests
1. Modify Postman collection as needed
2. Update environment variables
3. Add new test scenarios
4. Refresh test data periodically

### Version Control
- Keep collection and environment files in git
- Track changes to test scripts
- Document test updates
- Maintain backward compatibility

---

## 🎉 Ready to Test!

Your MorphingTalk API testing suite is ready! Choose your preferred method:

- **Quick Start**: `./run-tests.ps1` or `./run-tests.sh`
- **Node.js**: `npm test`
- **Manual**: `newman run collection.json`

Happy Testing! 🚀 