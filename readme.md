# MorphingTalk-API

## Project Overview

**MorphingTalk-API** is a sophisticated real-time chat application backend service with advanced voice morphing capabilities. The application enables users to communicate through text and voice messages with AI-powered voice transformation features, built using .NET Core and modern software architecture patterns.

## 🚀 Features

- **Real-time Communication**: WebSocket-based chat using SignalR
- **Voice Morphing**: AI-powered voice transformation capabilities
- **Multi-language Support**: Text translation services integration
- **Authentication**: JWT-based authentication with Firebase integration
- **File Management**: Audio, image, and document upload/download
- **Friend System**: User friendship management with blocking capabilities
- **Message Types**: Support for text, voice, and image messages
- **WebRTC Integration**: Voice and video calling capabilities

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
├── Domain/           # Core business entities and rules
├── Application/      # Use cases, DTOs, interfaces, and services  
├── Infrastructure/   # Data access, external services, repositories
└── MorphingTalk-API/ # Web API layer, controllers, configurations
```

### Key Patterns Implemented

- **Repository Pattern**: For data access abstraction
- **Dependency Injection**: Comprehensive DI configuration
- **CQRS-like Structure**: Separation of commands and queries
- **Entity Framework Core**: Code-first database approach
- **SignalR Hubs**: Real-time communication management

## 🛠️ Technology Stack

- **.NET 8.0**: Backend framework
- **Entity Framework Core**: ORM and database management
- **SignalR**: Real-time communication
- **SQL Server**: Primary database
- **Firebase Authentication**: User authentication
- **Azure Translator**: Text translation services
- **JWT**: Token-based authentication
- **AutoMapper**: Object mapping
- **Swagger/OpenAPI**: API documentation

## ⚙️ Prerequisites

Before running this application, ensure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- Firebase project for authentication
- Azure Translator service (optional, for translation features)
- AI service for voice morphing (optional)

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/MorphingTalk-API.git
cd MorphingTalk-API
```

### 2. Configure Database

Update the connection string in `MorphingTalk-API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_DB_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
  }
}
```

### 3. Configure Services

Update the configuration in `MorphingTalk-API/appsettings.json`:

```json
{
  "JWT": {
    "Issuer": "your-app-name",
    "Audience": "your-app-name",
    "signingKey": "YOUR_JWT_SIGNING_KEY_MINIMUM_256_BITS"
  },
  "Firebase": {
    "ProjectId": "your-firebase-project-id",
    "ServiceAccountKeyPath": "firebase-service-account-key.json"
  },
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password",
    "EnableSsl": true
  },
  "AzureTranslator": {
    "SubscriptionKey": "YOUR_AZURE_TRANSLATOR_API_KEY",
    "SubscriptionRegion": "your-region",
    "Endpoint": "https://api.cognitive.microsofttranslator.com"
  },
  "AIBaseLink": "http://your-ai-service-url:port",
  "AIJWTSecret": "YOUR_AI_SERVICE_JWT_TOKEN"
}
```

### 4. Set Up Firebase

1. Create a Firebase project at [Firebase Console](https://console.firebase.google.com/)
2. Enable Authentication and configure your preferred sign-in methods
3. Generate a service account key:
   - Go to Project Settings > Service Accounts
   - Click "Generate new private key"
   - Save the JSON file as `firebase-service-account-key.json` in the `MorphingTalk-API` folder

### 5. Database Migration

```bash
cd MorphingTalk-API
dotnet ef database update
```

### 6. Run the Application

```bash
dotnet run
```

The API will be available at `https://localhost:7218` (or the port specified in `launchSettings.json`).

## 📖 API Documentation

Once the application is running, you can access:

- **Swagger UI**: `https://localhost:7218/swagger`
- **API Endpoints**: See `API-Endpoints-Quick-Reference.md` for detailed endpoint documentation

## 🔧 Configuration Guide

### JWT Configuration

Generate a secure signing key (minimum 256 bits):

```csharp
// Example: Generate a secure key
var key = new byte[64];
using (var rng = RandomNumberGenerator.Create())
{
    rng.GetBytes(key);
}
var base64Key = Convert.ToBase64String(key);
```

### SMTP Configuration

For email services, configure Gmail App Password:

1. Enable 2-Factor Authentication on your Gmail account
2. Generate an App Password
3. Use the App Password in the `SmtpSettings.Password` field

### Azure Translator Setup

1. Create an Azure Translator resource
2. Get the subscription key and region
3. Update the `AzureTranslator` configuration

## 🧪 Testing

### Running Unit Tests

```bash
dotnet test Tests/
```

### API Testing with Postman

1. Import the Postman collection: `MorphingTalk-API-Postman-Collection.json`
2. Set up environment variables as described in `Postman-Collection-Guide.md`
3. Run the automated test suite

### PowerShell Testing

```bash
.\run-tests.ps1
```

## 📁 Project Structure

```
MorphingTalk-API/
├── Domain/
│   ├── Entities/
│   │   ├── Users/          # User entities
│   │   ├── Chatting/       # Message and conversation entities
│   │   └── AIModels/       # Voice model entities
├── Application/
│   ├── DTOs/               # Data Transfer Objects
│   ├── Interfaces/         # Service and repository interfaces
│   ├── Services/           # Business logic services
│   └── Hubs/              # SignalR hubs
├── Infrastructure/
│   ├── Data/              # Database context and migrations
│   └── Repositories/      # Data access implementations
└── MorphingTalk-API/
    ├── Controllers/       # API controllers
    ├── Extensions/        # Service registration extensions
    └── wwwroot/          # Static files and uploads
```

## 🔐 Security Considerations

- **Environment Variables**: Store sensitive data in environment variables or Azure Key Vault
- **HTTPS**: Always use HTTPS in production
- **CORS**: Configure CORS policies appropriately
- **JWT Expiration**: Set appropriate token expiration times
- **File Upload Validation**: Implement proper file validation and size limits
- **Rate Limiting**: Consider implementing rate limiting for API endpoints

## 🚀 Deployment

### Prerequisites for Deployment

1. Configure production database
2. Set up production environment variables
3. Configure domain and SSL certificates
4. Set up file storage (Azure Blob Storage recommended)

### Environment Variables

Create the following environment variables for production:

```bash
ConnectionStrings__DefaultConnection=your_production_db_connection
JWT__SigningKey=your_secure_jwt_key
Firebase__ProjectId=your_firebase_project_id
SmtpSettings__UserName=your_email
SmtpSettings__Password=your_email_app_password
AzureTranslator__SubscriptionKey=your_azure_key
AIBaseLink=your_ai_service_url
AIJWTSecret=your_ai_jwt_token
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions:

- Create an issue in this repository
- Check the documentation files in the repository
- Review the API endpoint documentation

## 🔄 Version History

- **v1.0.0**: Initial release with core chat functionality
- **v1.1.0**: Added voice morphing capabilities
- **v1.2.0**: Implemented translation services
- **v1.3.0**: Added friend system and blocking features

---

**Note**: This project was developed as a graduation project demonstrating modern software architecture patterns and real-time communication technologies. 