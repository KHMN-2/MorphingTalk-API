# MorphingTalk-API: Real-Time Chat Application with AI Voice Morphing
## Graduation Project Documentation

### Student Information
- **Project Title**: MorphingTalk-API - Advanced Real-Time Communication Platform
- **Technology Stack**: .NET 8.0, Entity Framework Core, SignalR, SQL Server
- **Project Type**: Backend API Service with AI Integration

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [System Architecture](#system-architecture)
3. [Database Design](#database-design)
4. [Implementation Analysis](#implementation-analysis)
5. [Technical Specifications](#technical-specifications)
6. [Features & Functionality](#features--functionality)
7. [Security Implementation](#security-implementation)
8. [Performance & Scalability](#performance--scalability)
9. [Conclusion](#conclusion)

---

## Project Overview

### 1.1 Project Description
MorphingTalk-API is a sophisticated backend service for a real-time chat application that incorporates cutting-edge AI voice morphing technology. The system enables users to communicate through text and voice messages with the unique capability of transforming their voice using AI-powered voice synthesis and morphing.

### 1.2 Key Objectives
- **Real-Time Communication**: Implement WebSocket-based messaging using SignalR
- **AI Voice Integration**: Integrate external AI services for voice training and morphing
- **Scalable Architecture**: Design using Clean Architecture principles
- **Advanced Social Features**: Friend management, blocking, and group conversations
- **Multi-Language Support**: Voice message translation capabilities

### 1.3 Technology Stack
```
Backend Framework: .NET 8.0 (ASP.NET Core Web API)
Database: SQL Server with Entity Framework Core 9.0.3
Real-Time Communication: SignalR
Authentication: ASP.NET Core Identity + JWT
AI Integration: External AI Service via HTTP Client
File Storage: Local file system (wwwroot)
API Documentation: OpenAPI/Swagger with Scalar
```

---

## System Architecture

### 2.1 Clean Architecture Implementation
The project follows Clean Architecture principles with clear separation of concerns:

```
├── Domain/           # Business entities and core logic
├── Application/      # Use cases, services, and interfaces
├── Infrastructure/   # Data access and external services
└── MorphingTalk-API/ # Web API controllers and configuration
```

### 2.2 Architecture Benefits
- **Dependency Inversion**: Core business logic is independent of external frameworks
- **Testability**: Clear boundaries enable comprehensive unit testing
- **Maintainability**: Changes in one layer don't affect others
- **Scalability**: Easy to extend functionality without breaking existing code

---

## Database Design

### 3.1 Schema Overview
The database schema implements sophisticated relationships to support real-time chat with AI voice morphing:

**Core Entities:**
- **Users**: Extended ASP.NET Identity with rich profile features
- **Conversations**: Support for one-to-one and group chats
- **Messages**: Polymorphic design supporting text and voice messages
- **Friendships**: Social graph with blocking capabilities
- **UserVoiceModels**: AI voice training integration

### 3.2 Entity Relationship Design

#### User Entity (Extended Identity)
```sql
-- Core user properties extending IdentityUser
FullName NVARCHAR(MAX) NOT NULL
NativeLanguage NVARCHAR(MAX) -- For AI voice training
IsTrainedVoice BIT -- Voice model status
VoiceModelId NVARCHAR(450) -- FK to UserVoiceModels
UseRobotVoice BIT DEFAULT 1 -- Voice preference
TranslateMessages BIT DEFAULT 0 -- Translation preference
IsOnline BIT DEFAULT 0 -- Presence tracking
LastSeen DATETIME2 -- Activity tracking
```

#### Message Polymorphism (Table-Per-Hierarchy)
```sql
-- Base Message table with discriminator
CREATE TABLE Messages (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    MessageType NVARCHAR(8) NOT NULL, -- 'Text' or 'Voice'
    ConversationId UNIQUEIDENTIFIER NOT NULL,
    ConversationUserId UNIQUEIDENTIFIER NOT NULL,
    SentAt DATETIME2 NOT NULL,
    Status INT NOT NULL, -- Sent/Delivered/Read
    
    -- TextMessage fields
    Content NVARCHAR(MAX) NULL,
    
    -- VoiceMessage fields
    VoiceUrl NVARCHAR(MAX) NULL,
    IsTranslated BIT NULL,
    TranslatedVoiceUrl NVARCHAR(MAX) NULL,
    DurationSeconds INT NULL
);
```

#### AI Voice Integration
```sql
CREATE TABLE UserVoiceModels (
    Id NVARCHAR(450) PRIMARY KEY,
    TaskId NVARCHAR(MAX) NOT NULL, -- AI service reference
    Name NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    Status INT NOT NULL -- Training/Completed/Failed
);
```

### 3.3 Key Relationships
- **User ↔ ConversationUser**: Many-to-many via junction table
- **Conversation ↔ Message**: One-to-many with cascade rules
- **User ↔ Friendship**: Self-referencing many-to-many
- **User ↔ UserVoiceModel**: One-to-one optional relationship

---

## Implementation Analysis

### 4.1 Real-Time Communication (SignalR)

**ChatHub Implementation:**
```csharp
[Authorize]
public class ChatHub : Hub
{
    // Group management for conversations
    public async Task JoinConversation(string conversationId)
    
    // WebRTC signaling for voice/video calls
    public async Task SendOffer(string conversationId, string targetUserId, object offer)
    
    // Typing indicators
    public async Task StartTyping(string conversationId)
    
    // Online presence tracking
    public async Task SetOnlineStatus(bool isOnline)
}
```

**Key Features:**
- Thread-safe concurrent collections for user state management
- WebRTC signaling for voice/video calls
- Real-time typing indicators
- Online presence tracking

### 4.2 Voice Processing Pipeline

**Training Process:**
1. User uploads audio file (.wav, .m4a, .mp3, .flac)
2. File validation and format checking
3. Multipart form submission to AI service
4. Asynchronous training status tracking
5. Database model status updates via webhooks

**Voice Morphing Process:**
1. Voice message recorded by user
2. Original file stored in user directory
3. AI service processes voice morphing (if model available)
4. Translation service (if enabled)
5. Real-time delivery to conversation participants

### 4.3 Security Implementation

**Authentication & Authorization:**
```csharp
// JWT Configuration
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey)
        };
    });

// Identity Configuration
services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    // ... additional requirements
});
```

---

## Technical Specifications

### 5.1 API Endpoints

**Authentication:**
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User authentication
- `POST /api/auth/refresh` - Token refresh

**Chat Management:**
- `GET /api/chatting/conversations` - Get user conversations
- `POST /api/chatting/conversations/{id}/messages` - Send message
- `GET /api/chatting/conversations/{id}/messages` - Retrieve messages

**Voice Training:**
- `POST /api/voicetraining/train` - Upload training audio
- `GET /api/voicetraining/status` - Check training status
- `DELETE /api/voicetraining/model` - Delete voice model

**Social Features:**
- `POST /api/friendship/send-request` - Send friend request
- `PUT /api/friendship/accept` - Accept friend request
- `PUT /api/friendship/block` - Block user

### 5.2 File Storage Structure
```
wwwroot/
├── Uploads/
│   ├── [UserId]/
│   │   ├── audios/        # Original voice messages
│   │   └── images/        # Profile pictures
│   └── profile-pictures/  # Global profile storage
└── translated_audio/      # AI-processed voice files
```

### 5.3 Configuration Management
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "SQL Server connection string"
  },
  "JWT": {
    "Issuer": "morphingtalk",
    "Audience": "morphingtalk",
    "SigningKey": "secure-signing-key"
  },
  "AIBaseLink": "http://34.231.46.228:6969",
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587
  }
}
```

---

## Features & Functionality

### 6.1 Core Features
- **Real-Time Messaging**: Instant text and voice communication
- **AI Voice Morphing**: Transform voice using trained AI models
- **Multi-Language Translation**: Automatic voice message translation
- **Group Conversations**: Multi-participant chat rooms with admin controls
- **Friend Management**: Social graph with blocking capabilities
- **WebRTC Integration**: Voice and video calling support

### 6.2 Advanced Features
- **Typing Indicators**: Real-time typing status
- **Online Presence**: User activity tracking
- **Message Status**: Sent/Delivered/Read receipts
- **File Attachments**: Image and audio file sharing
- **Robot Voice**: Fallback synthetic voice option
- **Per-Conversation Settings**: Individual user preferences

---

## Performance & Scalability

### 7.1 Performance Optimizations
- **Async/Await Pattern**: Non-blocking operations throughout
- **Connection Pooling**: EF Core database connection optimization
- **SignalR Groups**: Efficient message broadcasting
- **HttpClient Factory**: Proper HTTP client lifecycle management

### 7.2 Scalability Considerations
- **Clean Architecture**: Easy to extend and modify
- **Repository Pattern**: Abstracted data access layer
- **Dependency Injection**: Loose coupling between components
- **Microservice Ready**: External AI service integration pattern

---

## Conclusion

### 8.1 Project Achievements
The MorphingTalk-API successfully demonstrates:

1. **Advanced Software Architecture**: Implementation of Clean Architecture principles with proper separation of concerns
2. **Cutting-Edge Technology Integration**: Successful integration of AI voice processing services
3. **Real-Time Communication**: Robust SignalR implementation for instant messaging
4. **Scalable Database Design**: Sophisticated schema supporting complex relationships
5. **Security Best Practices**: Comprehensive authentication and authorization implementation
6. **Production-Ready Code**: Industry-standard patterns and practices

### 8.2 Technical Innovation
- **AI Voice Morphing**: Innovative integration of external AI services for voice transformation
- **Polymorphic Message Design**: Elegant solution for handling multiple message types
- **Social Graph Implementation**: Advanced friendship and blocking mechanisms
- **Real-Time Features**: Comprehensive WebSocket-based communication

### 8.3 Academic Value
This project demonstrates mastery of:
- Modern .NET development practices
- Database design and Entity Framework Core
- Real-time web applications using SignalR
- RESTful API design and implementation
- External service integration patterns
- Security implementation in web applications
- Clean code principles and software architecture

The MorphingTalk-API represents a production-ready, enterprise-level backend service that showcases advanced programming skills and modern software development practices suitable for a graduation project demonstration.

---

### References
- Microsoft .NET 8.0 Documentation
- Entity Framework Core Documentation
- ASP.NET Core SignalR Documentation
- JWT Authentication Best Practices
- Clean Architecture Principles by Robert C. Martin 