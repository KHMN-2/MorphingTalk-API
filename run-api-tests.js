const newman = require('newman');
const fs = require('fs');
const path = require('path');

// Configuration
const config = {
    collection: './MorphingTalk-API-Postman-Collection.json',
    environment: './test-environment.json',
    reporters: ['cli', 'json', 'html'],
    reporterOptions: {
        html: {
            export: './test-reports/html-report.html'
        },
        json: {
            export: './test-reports/json-report.json'
        }
    },
    insecure: true, // Ignore SSL certificate errors for local testing
    timeout: 30000,
    iterationCount: 1
};

// Create test reports directory if it doesn't exist
if (!fs.existsSync('./test-reports')) {
    fs.mkdirSync('./test-reports');
}

// Test results summary
let testResults = {
    timestamp: new Date().toISOString(),
    totalTests: 0,
    passedTests: 0,
    failedTests: 0,
    skippedTests: 0,
    errors: [],
    warnings: [],
    summary: {}
};

console.log('🚀 Starting MorphingTalk API Tests...');
console.log('=' .repeat(60));

// Run Newman collection
newman.run(config, function (err, summary) {
    if (err) {
        console.error('❌ Newman run failed:', err);
        process.exit(1);
    }

    console.log('\n📊 Test Results Summary:');
    console.log('=' .repeat(60));

    // Process test results
    const stats = summary.run.stats;
    testResults.totalTests = stats.tests.total;
    testResults.passedTests = stats.tests.passed;
    testResults.failedTests = stats.tests.failed;
    testResults.skippedTests = stats.tests.pending;

    // Display summary
    console.log(`✅ Total Tests: ${stats.tests.total}`);
    console.log(`✅ Passed: ${stats.tests.passed}`);
    console.log(`❌ Failed: ${stats.tests.failed}`);
    console.log(`⏭️  Skipped: ${stats.tests.pending}`);
    console.log(`🔄 Iterations: ${stats.iterations.total}`);
    console.log(`📨 Requests: ${stats.requests.total}`);
    console.log(`⚠️  Assertions: ${stats.assertions.total}`);

    // Check for failures
    if (stats.tests.failed > 0) {
        console.log('\n❌ Failed Tests:');
        console.log('-'.repeat(40));
        
        summary.run.failures.forEach((failure, index) => {
            console.log(`${index + 1}. ${failure.error.name}`);
            console.log(`   Request: ${failure.source.name}`);
            console.log(`   Error: ${failure.error.message}`);
            console.log(`   Test: ${failure.error.test}`);
            console.log('');
            
            testResults.errors.push({
                request: failure.source.name,
                error: failure.error.message,
                test: failure.error.test
            });
        });
    }

    // Process execution summary
    const executions = summary.run.executions;
    let endpointResults = {};
    
    executions.forEach((execution, index) => {
        const requestName = execution.item.name;
        const response = execution.response;
        const assertions = execution.assertions;
        
        endpointResults[requestName] = {
            status: response ? response.code : 'No Response',
            responseTime: response ? response.responseTime : 0,
            tests: {
                total: assertions ? assertions.length : 0,
                passed: assertions ? assertions.filter(a => !a.error).length : 0,
                failed: assertions ? assertions.filter(a => a.error).length : 0
            }
        };
    });

    testResults.summary = endpointResults;

    // Display endpoint results
    console.log('\n📋 Endpoint Test Results:');
    console.log('=' .repeat(60));
    
    Object.entries(endpointResults).forEach(([endpoint, result]) => {
        const status = result.status;
        const statusIcon = status === 200 ? '✅' : status >= 400 ? '❌' : '⚠️';
        const testIcon = result.tests.failed === 0 ? '✅' : '❌';
        
        console.log(`${statusIcon} ${endpoint}`);
        console.log(`   Status: ${status} | Response Time: ${result.responseTime}ms`);
        console.log(`   ${testIcon} Tests: ${result.tests.passed}/${result.tests.total} passed`);
        console.log('');
    });

    // Save detailed results
    const detailedResultsPath = './test-reports/detailed-results.json';
    fs.writeFileSync(detailedResultsPath, JSON.stringify(testResults, null, 2));

    // Generate feature status report
    generateFeatureStatusReport(testResults);

    // Final summary
    console.log('\n🎯 Test Completion Summary:');
    console.log('=' .repeat(60));
    
    const successRate = ((stats.tests.passed / stats.tests.total) * 100).toFixed(2);
    console.log(`📈 Success Rate: ${successRate}%`);
    
    if (stats.tests.failed === 0) {
        console.log('🎉 All tests passed! API is functioning correctly.');
    } else {
        console.log(`⚠️  ${stats.tests.failed} tests failed. Please check the detailed report.`);
    }

    console.log('\n📄 Reports Generated:');
    console.log(`   📊 HTML Report: ${path.resolve('./test-reports/html-report.html')}`);
    console.log(`   📋 JSON Report: ${path.resolve('./test-reports/json-report.json')}`);
    console.log(`   📝 Detailed Results: ${path.resolve('./test-reports/detailed-results.json')}`);
    console.log(`   📑 Feature Status: ${path.resolve('./test-reports/feature-status.md')}`);

    // Exit with appropriate code
    process.exit(stats.tests.failed > 0 ? 1 : 0);
});

function generateFeatureStatusReport(results) {
    const featureGroups = {
        'Authentication': ['Register User', 'Login User', 'Check Account'],
        'User Management': ['Get Logged In User', 'Get All Users', 'Update User Profile'],
        'Conversations': ['Create Conversation', 'Get My Conversations', 'Get Conversation by ID'],
        'Messaging': ['Send Text Message', 'Get Messages', 'Star Message'],
        'Friendship': ['Get Friends', 'Get Blocked Users'],
        'Voice Training': ['Get Training Status'],
        'System Tests': ['Test Firebase Setup', 'Test Webhook Payload']
    };

    let report = `# MorphingTalk API Feature Status Report\n\n`;
    report += `**Generated:** ${new Date().toISOString()}\n\n`;
    report += `## Overall Summary\n\n`;
    report += `- **Total Tests:** ${results.totalTests}\n`;
    report += `- **Passed:** ${results.passedTests}\n`;
    report += `- **Failed:** ${results.failedTests}\n`;
    report += `- **Success Rate:** ${((results.passedTests / results.totalTests) * 100).toFixed(2)}%\n\n`;

    report += `## Feature Status\n\n`;

    Object.entries(featureGroups).forEach(([feature, endpoints]) => {
        report += `### ${feature}\n\n`;
        
        let featurePassed = 0;
        let featureTotal = 0;
        
        endpoints.forEach(endpoint => {
            const result = results.summary[endpoint];
            if (result) {
                featureTotal++;
                if (result.tests.failed === 0 && result.status === 200) {
                    featurePassed++;
                    report += `- ✅ **${endpoint}** - PASSED\n`;
                } else {
                    report += `- ❌ **${endpoint}** - FAILED (Status: ${result.status})\n`;
                }
            }
        });
        
        const featureSuccess = featureTotal > 0 ? ((featurePassed / featureTotal) * 100).toFixed(2) : 0;
        report += `\n**${feature} Success Rate:** ${featureSuccess}%\n\n`;
    });

    if (results.errors.length > 0) {
        report += `## Failed Tests Details\n\n`;
        results.errors.forEach((error, index) => {
            report += `### ${index + 1}. ${error.request}\n`;
            report += `**Error:** ${error.error}\n`;
            report += `**Test:** ${error.test}\n\n`;
        });
    }

    report += `## Recommendations\n\n`;
    if (results.failedTests === 0) {
        report += `✅ All tests are passing! The API is functioning correctly across all features.\n\n`;
    } else {
        report += `⚠️ Some tests are failing. Please review the failed tests and:\n\n`;
        report += `1. Check API server status and connectivity\n`;
        report += `2. Verify database connections\n`;
        report += `3. Ensure required services are running\n`;
        report += `4. Review authentication tokens and permissions\n`;
        report += `5. Check external service dependencies\n\n`;
    }

    fs.writeFileSync('./test-reports/feature-status.md', report);
} 