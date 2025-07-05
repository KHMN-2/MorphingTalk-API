# 🧪 MorphingTalk API Testing Suite

## 📋 Overview

Complete automated testing solution for MorphingTalk API with scripts that verify all features are working correctly. This suite includes:

- ✅ **60+ API Endpoints** tested automatically
- ✅ **Cross-platform scripts** (Windows, Linux, Mac)
- ✅ **Comprehensive reports** (HTML, JSON, Markdown)
- ✅ **Feature validation** with detailed status
- ✅ **Performance monitoring** and metrics

## 🚀 Quick Start

### 🎯 One-Command Testing

#### Windows (PowerShell)
```powershell
./run-tests.ps1
```

#### Linux/Mac (Bash)
```bash
chmod +x run-tests.sh && ./run-tests.sh
```

#### Node.js (Cross-platform)
```bash
npm install && npm test
```

## 📦 What's Included

### 📄 Core Files
| File | Purpose |
|------|---------|
| `MorphingTalk-API-Postman-Collection.json` | Complete API test collection |
| `test-environment.json` | Environment variables |
| `run-tests.ps1` | Windows PowerShell script |
| `run-tests.sh` | Linux/Mac bash script |
| `run-api-tests.js` | Node.js test runner |
| `package.json` | Dependencies and npm scripts |

### 📚 Documentation
| File | Purpose |
|------|---------|
| `Automated-Testing-Guide.md` | Complete automation guide |
| `Postman-Collection-Guide.md` | Manual testing guide |
| `API-Endpoints-Quick-Reference.md` | API reference |
| `README-Testing.md` | This file |

## 🎯 Features Tested

### ✅ Authentication System
- User registration and login
- OTP verification and password reset
- Firebase authentication
- Token management

### ✅ User Management
- Profile operations and updates
- User data retrieval
- Profile picture uploads
- Account management

### ✅ Conversation Management
- Create and manage conversations
- Add/remove participants
- Conversation settings

### ✅ Messaging System
- Text, voice, and image messages
- Message starring and deletion
- Reply functionality
- Translation features

### ✅ Friendship Features
- Add/remove friends
- User blocking system
- Friends list management

### ✅ Voice Training
- AI voice model training
- Training status monitoring
- Model management

### ✅ File Management
- Upload various file types
- File validation and storage
- Download functionality

### ✅ Call Management
- Voice/video call initiation
- Call status management
- Real-time features

### ✅ System Integration
- Firebase integration
- Webhook processing
- External service health

## 📊 Test Reports

After running tests, you'll get:

### 📈 HTML Report (`test-reports/html-report.html`)
- Visual dashboard with charts
- Detailed request/response data
- Performance metrics
- Interactive test results

### 📋 JSON Report (`test-reports/json-report.json`)
- Raw test data for analysis
- Statistical information
- Machine-readable format

### 📑 Feature Status (`test-reports/feature-status.md`)
- Feature-by-feature status
- Success rates by category
- Recommendations for fixes

## ⚙️ Configuration

### Environment Setup
Edit `test-environment.json`:
```json
{
  "baseUrl": "https://localhost:7218",
  "testEmail": "test@example.com",
  "testPassword": "TestPassword123!"
}
```

### Script Options

#### PowerShell (`run-tests.ps1`)
```powershell
# Basic run
./run-tests.ps1

# Custom URL
./run-tests.ps1 -BaseUrl "http://localhost:5000"

# Verbose output
./run-tests.ps1 -Verbose

# Skip Newman installation
./run-tests.ps1 -SkipInstall
```

#### Bash (`run-tests.sh`)
```bash
# Basic run
./run-tests.sh

# Custom URL  
./run-tests.sh -u "http://localhost:5000"

# Verbose output
./run-tests.sh --verbose

# Help
./run-tests.sh --help
```

#### Node.js
```bash
# Full test suite
npm test

# Quick test
npm run test:quick

# Verbose output
npm run test:verbose
```

## 🔧 Prerequisites

1. **Node.js & npm** (v14+)
   - Download: https://nodejs.org/

2. **MorphingTalk API running**
   - Default: `https://localhost:7218`
   - Configure in environment files

3. **Newman (auto-installed)**
   - Installs automatically when first run

## 📈 Test Results Interpretation

### ✅ Success Indicators
- Status code 200/201
- Proper JSON response structure
- Expected data in responses
- Authentication working
- Response times under limits

### ❌ Failure Indicators
- 4xx/5xx status codes
- Missing response fields
- Authentication failures
- Timeout errors
- Malformed responses

### 📊 Performance Metrics
- Response time tracking
- Request/response size
- Error rate monitoring
- Feature availability

## 🚨 Troubleshooting

### Common Issues

#### Newman Not Found
```bash
npm install -g newman newman-reporter-html
```

#### API Server Not Running
```bash
# Test manually
curl https://localhost:7218/api/User/GetAllUsers
```

#### SSL Certificate Errors
All scripts include `--insecure` flag for development

#### Authentication Failures
- Verify test credentials
- Check user registration
- Ensure database is accessible

### Debug Mode
```bash
# Enable verbose output for detailed logs
./run-tests.ps1 -Verbose
./run-tests.sh --verbose
```

## 🔄 CI/CD Integration

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
      - run: npm install -g newman
      - run: ./run-tests.sh
```

### Azure DevOps
```yaml
- script: './run-tests.sh'
  displayName: 'Run API Tests'
```

## 📞 Usage Examples

### Development Testing
```bash
# Test after code changes
./run-tests.ps1

# Test specific features
newman run collection.json --folder "Authentication Tests"
```

### Deployment Validation
```bash
# Test production endpoint
./run-tests.ps1 -BaseUrl "https://api.morphingtalk.com"

# Generate reports for review
npm test
```

### Performance Monitoring
```bash
# Regular health checks
npm run test:quick

# Detailed performance analysis
./run-tests.ps1 -Verbose
```

## 🎯 Best Practices

1. **Regular Testing**: Run before deployments
2. **Environment Maintenance**: Keep URLs current
3. **Credential Updates**: Refresh test accounts
4. **Report Reviews**: Check failures immediately
5. **Performance Monitoring**: Track response times

## 📚 Additional Resources

- [Automated Testing Guide](Automated-Testing-Guide.md) - Complete automation documentation
- [Postman Collection Guide](Postman-Collection-Guide.md) - Manual testing guide
- [API Reference](API-Endpoints-Quick-Reference.md) - Quick endpoint reference

## 🎉 Get Started Now!

1. **Choose your platform**:
   - Windows: `./run-tests.ps1`
   - Linux/Mac: `./run-tests.sh`
   - Node.js: `npm test`

2. **Review reports** in `test-reports/` folder

3. **Fix any issues** found by tests

4. **Re-run tests** to verify fixes

Your MorphingTalk API testing suite is ready! 🚀

---

## 📊 Quick Status Check

Run this command to get instant API health status:
```bash
# Windows
./run-tests.ps1 -Collection "Quick-Health-Check.json"

# Linux/Mac
./run-tests.sh --collection "Quick-Health-Check.json"

# Node.js
npm run test:quick
``` 