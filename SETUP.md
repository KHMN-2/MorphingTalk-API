# MorphingTalk-API Setup Guide

This guide will help you set up the MorphingTalk-API project from scratch.

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- Git for version control

## Step-by-Step Setup

### 1. Clone and Setup Project

```bash
git clone <your-repository-url>
cd MorphingTalk-API
```

### 2. Database Configuration

#### Create Database
1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Create a new database (e.g., `MorphingTalkDB`)

#### Configure Connection String
1. Copy `MorphingTalk-API/appsettings.template.json` to `MorphingTalk-API/appsettings.json`
2. Update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MorphingTalkDB;Trusted_Connection=true;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
  }
}
```

For SQL Server Authentication, use:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MorphingTalkDB;User Id=your_username;Password=your_password;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
  }
}
```

### 3. Firebase Configuration

#### Create Firebase Project
1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Click "Add project" and follow the setup wizard
3. Enable Authentication:
   - Go to Authentication > Sign-in method
   - Enable Email/Password and Google (recommended)

#### Generate Service Account Key
1. Go to Project Settings > Service Accounts
2. Click "Generate new private key"
3. Save the downloaded JSON file as `firebase-service-account-key.json` in the `MorphingTalk-API` folder

#### Update Configuration
Update the Firebase section in `appsettings.json`:
```json
{
  "Firebase": {
    "ProjectId": "your-project-id",
    "ServiceAccountKeyPath": "firebase-service-account-key.json"
  }
}
```

### 4. JWT Configuration

Generate a secure JWT signing key. You can use online tools or PowerShell:

```powershell
# Generate a secure 512-bit key
$bytes = New-Object byte[] 64
[System.Security.Cryptography.RNGCryptoServiceProvider]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

Update the JWT section in `appsettings.json`:
```json
{
  "JWT": {
    "Issuer": "morphingtalk",
    "Audience": "morphingtalk",
    "signingKey": "YOUR_GENERATED_KEY_HERE"
  }
}
```

### 5. Email Configuration (Optional)

To enable email notifications, configure SMTP settings:

#### For Gmail:
1. Enable 2-Factor Authentication
2. Generate an App Password
3. Update configuration:

```json
{
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password",
    "EnableSsl": true
  }
}
```

### 6. Azure Translator (Optional)

For translation features:

1. Create Azure Translator resource in Azure Portal
2. Get the subscription key and region
3. Update configuration:

```json
{
  "AzureTranslator": {
    "SubscriptionKey": "your-subscription-key",
    "SubscriptionRegion": "your-region",
    "Endpoint": "https://api.cognitive.microsofttranslator.com"
  }
}
```

### 7. AI Service Configuration (Optional)

If you have an AI service for voice morphing:

```json
{
  "AIBaseLink": "http://your-ai-service-url:port",
  "AIJWTSecret": "your-ai-service-token"
}
```

### 8. Database Migration

Run the following commands to create the database schema:

```bash
cd MorphingTalk-API
dotnet ef database update
```

If you encounter issues, ensure Entity Framework tools are installed:
```bash
dotnet tool install --global dotnet-ef
```

### 9. Build and Run

```bash
# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project MorphingTalk-API
```

The API will start at `https://localhost:7218` (or check the console output for the actual port).

### 10. Verify Setup

1. Navigate to `https://localhost:7218/swagger` to see the API documentation
2. Try registering a new user through the API
3. Test the authentication endpoints

## Configuration Files Summary

After setup, your configuration structure should look like this:

```
MorphingTalk-API/
├── appsettings.json (your actual config - DO NOT COMMIT)
├── appsettings.template.json (template for others)
├── firebase-service-account-key.json (DO NOT COMMIT)
└── ... other files
```

## Security Checklist

- [ ] Changed all default keys and passwords
- [ ] Generated secure JWT signing key
- [ ] Configured Firebase properly
- [ ] Set up proper database permissions
- [ ] Verified .gitignore excludes sensitive files
- [ ] Used environment variables for production

## Troubleshooting

### Common Issues

1. **Database Connection Issues**
   - Verify SQL Server is running
   - Check connection string format
   - Ensure database exists

2. **Firebase Authentication Issues**
   - Verify project ID is correct
   - Check service account key file path
   - Ensure Firebase Authentication is enabled

3. **Migration Issues**
   - Run `dotnet ef database drop` to reset (WARNING: deletes all data)
   - Run `dotnet ef database update` again

4. **Port Already in Use**
   - Change port in `launchSettings.json`
   - Or kill the process using the port

### Getting Help

- Check the main README.md for additional information
- Review the API documentation at `/swagger`
- Check the documentation files in the repository
- Create an issue if you encounter problems

## Development Environment

For development, you may want to:

1. Set up multiple environments (Development, Staging, Production)
2. Use different databases for each environment
3. Set up continuous integration/deployment
4. Configure logging and monitoring

## Next Steps

After successful setup:

1. Explore the API endpoints using Swagger
2. Test with the provided Postman collection
3. Set up your frontend application
4. Configure production deployment

---

**Note**: Keep your `appsettings.json` and `firebase-service-account-key.json` files secure and never commit them to version control. 