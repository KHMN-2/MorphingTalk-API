# MorphingTalk-API Database Schema Design & Description

## Overview

The MorphingTalk-API uses a sophisticated database schema designed to support real-time chat functionality with advanced voice morphing capabilities. The schema implements clean separation of concerns, polymorphic message handling, and complex social relationship management.

## Database Technology Stack

- **Database Engine**: SQL Server
- **ORM**: Entity Framework Core 9.0.3
- **Migration Strategy**: Code-First with EF Migrations
- **Connection**: Encrypted connection with TrustServerCertificate enabled

## Schema Architecture

### Core Design Principles

1. **Domain-Driven Design**: Entities reflect business domain concepts
2. **Polymorphic Inheritance**: Table-Per-Hierarchy (TPH) for message types
3. **Many-to-Many Relationships**: Proper junction tables with additional metadata
4. **Data Integrity**: Comprehensive foreign key constraints and cascade rules
5. **Extensibility**: Designed for future feature additions

## Entity Relationship Overview

```
Users (Identity) ←→ ConversationUsers ←→ Conversations
     │                    │                    │
     │                    ↓                    │
     │                Messages               │
     │              (Text/Voice)             │
     │                                       │
     ↓                                       │
UserVoiceModels                             │
     │                                       │
     └── AI Training Data                    │
                                             │
Users ←→ Friendships (Social Graph)         │
                                             │
File Storage ←── Voice/Image Files ←────────┘
```

## Detailed Entity Specifications

### 1. User Entity (Extended Identity)

**Table**: `AspNetUsers` (Identity Framework)

```sql
CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) PRIMARY KEY,
    -- Identity Framework fields
    UserName NVARCHAR(256),
    NormalizedUserName NVARCHAR(256),
    Email NVARCHAR(256),
    NormalizedEmail NVARCHAR(256),
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX),
    SecurityStamp NVARCHAR(MAX),
    ConcurrencyStamp NVARCHAR(MAX),
    PhoneNumber NVARCHAR(MAX),
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEnd DATETIMEOFFSET,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL,
    
    -- Custom User Properties
    FullName NVARCHAR(MAX) NOT NULL,
    CreatedOn DATETIME2 NOT NULL,
    LastUpdatedOn DATETIME2 NOT NULL,
    IsDeactivated BIT NOT NULL DEFAULT 0,
    IsFirstLogin BIT NULL,
    Gender NVARCHAR(MAX) NULL,
    NativeLanguage NVARCHAR(MAX) NULL,
    AboutStatus NVARCHAR(MAX) NULL,
    ProfilePicturePath NVARCHAR(MAX) NULL,
    PastProfilePicturePaths NVARCHAR(MAX) NULL, -- JSON array
    IsOnline BIT NOT NULL DEFAULT 0,
    LastSeen DATETIME2 NULL,
    IsTrainedVoice BIT NOT NULL DEFAULT 0,
    VoiceModelId NVARCHAR(450) NULL, -- FK to UserVoiceModels
    UseRobotVoice BIT NOT NULL DEFAULT 1,
    MuteNotifications BIT NOT NULL DEFAULT 0,
    TranslateMessages BIT NOT NULL DEFAULT 0
);
```

**Key Features:**
- Extends `IdentityUser` for authentication/authorization
- Rich profile information including social features
- Voice processing preferences and AI model linking
- Online presence tracking
- Profile picture versioning support

### 2. Conversation Entity

**Table**: `Conversations`

```sql
CREATE TABLE Conversations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Type INT NOT NULL, -- 0: OneToOne, 1: Group
    Name NVARCHAR(MAX) NULL,
    GroupImageUrl NVARCHAR(MAX) NULL,
    Description NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL,
    LastActivityAt DATETIME2 NOT NULL
);
```

**Conversation Types:**
- **OneToOne**: Direct messaging between two users
- **Group**: Multi-participant chat rooms

### 3. ConversationUser Entity (Junction Table)

**Table**: `ConversationUsers`

```sql
CREATE TABLE ConversationUsers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId NVARCHAR(450) NOT NULL,
    ConversationId UNIQUEIDENTIFIER NOT NULL,
    JoinedAt DATETIME2 NOT NULL,
    Role INT NOT NULL, -- 0: Admin, 1: Member
    LeftConversation BIT NOT NULL DEFAULT 0,
    UseRobotVoice BIT NOT NULL DEFAULT 1,
    TranslateMessages BIT NOT NULL DEFAULT 0,
    muteNotifications BIT NOT NULL DEFAULT 0,
    
    CONSTRAINT FK_ConversationUsers_Users FOREIGN KEY (UserId) 
        REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ConversationUsers_Conversations FOREIGN KEY (ConversationId) 
        REFERENCES Conversations(Id) ON DELETE CASCADE,
    CONSTRAINT IX_ConversationUsers_ConversationId_UserId 
        UNIQUE (ConversationId, UserId)
);
```

**Key Features:**
- Manages many-to-many relationship between Users and Conversations
- Role-based permissions (Admin/Member)
- Per-conversation user preferences
- Soft delete via `LeftConversation` flag
- Individual notification and translation settings

### 4. Message Entity (Polymorphic Base)

**Table**: `Messages` (Table-Per-Hierarchy)

```sql
CREATE TABLE Messages (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ConversationId UNIQUEIDENTIFIER NOT NULL,
    ConversationUserId UNIQUEIDENTIFIER NOT NULL,
    SentAt DATETIME2 NOT NULL,
    Status INT NOT NULL, -- 0: Sent, 1: Delivered, 2: Read
    MessageType NVARCHAR(8) NOT NULL, -- Discriminator: 'Text' or 'Voice'
    
    -- TextMessage Properties
    Content NVARCHAR(MAX) NULL,
    
    -- VoiceMessage Properties
    VoiceUrl NVARCHAR(MAX) NULL,
    IsTranslated BIT NULL,
    TranslatedVoiceUrl NVARCHAR(MAX) NULL,
    DurationSeconds INT NULL,
    
    CONSTRAINT FK_Messages_Conversations FOREIGN KEY (ConversationId) 
        REFERENCES Conversations(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Messages_ConversationUsers FOREIGN KEY (ConversationUserId) 
        REFERENCES ConversationUsers(Id) ON DELETE CASCADE
);
```

**Polymorphic Design:**
- **Base Class**: `Message` with common properties
- **Text Messages**: Additional `Content` field
- **Voice Messages**: URL references, translation support, duration tracking
- **Discriminator**: `MessageType` column determines concrete type

### 5. TextMessage Entity

**Properties Added:**
```csharp
public class TextMessage : Message
{
    public string Content { get; set; }
    public MessageType Type => MessageType.Text;
}
```

### 6. VoiceMessage Entity

**Properties Added:**
```csharp
public class VoiceMessage : Message
{
    public string VoiceUrl { get; set; }
    public bool IsTranslated { get; set; }
    public string? TranslatedVoiceUrl { get; set; }
    public int? DurationSeconds { get; set; }
    public MessageType Type => MessageType.Voice;
}
```

**Advanced Voice Features:**
- Original voice file storage
- Translated version support
- Duration tracking for UI/UX
- Morphed voice integration

### 7. Friendship Entity

**Table**: `Friendships`

```sql
CREATE TABLE Friendships (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId1 NVARCHAR(450) NOT NULL,
    UserId2 NVARCHAR(450) NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    IsBlocked BIT NOT NULL DEFAULT 0,
    BlockedByUserId NVARCHAR(450) NULL,
    
    CONSTRAINT FK_Friendships_Users1 FOREIGN KEY (UserId1) 
        REFERENCES AspNetUsers(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Friendships_Users2 FOREIGN KEY (UserId2) 
        REFERENCES AspNetUsers(Id) ON DELETE RESTRICT,
    CONSTRAINT IX_Friendships_UserId1_UserId2 UNIQUE (UserId1, UserId2)
);
```

**Social Features:**
- Bidirectional friendship representation
- Blocking mechanism with attribution
- Unique constraint prevents duplicate relationships
- Restricted delete to maintain relationship history

### 8. UserVoiceModel Entity (AI Integration)

**Table**: `UserVoiceModels`

```sql
CREATE TABLE UserVoiceModels (
    Id NVARCHAR(450) PRIMARY KEY,
    TaskId NVARCHAR(MAX) NOT NULL,
    Name NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    Status INT NOT NULL -- 0: Training, 1: Completed, 2: Failed
);
```

**AI Training Pipeline:**
- **TaskId**: Reference to external AI service training job
- **Status Tracking**: Training/Completed/Failed states
- **Model Naming**: User-friendly identification
- **Timestamp Tracking**: Creation and completion monitoring

## Relationship Specifications

### 1. User ↔ ConversationUser (One-to-Many)

```csharp
// In User entity
public ICollection<ConversationUser> ConversationUsers { get; set; }

// In ConversationUser entity
public string UserId { get; set; }
public User User { get; set; }
```

**Cascade Behavior**: `CASCADE` - When user is deleted, all conversation memberships are removed

### 2. Conversation ↔ ConversationUser (One-to-Many)

```csharp
// In Conversation entity
public ICollection<ConversationUser> ConversationUsers { get; set; }

// In ConversationUser entity
public Guid ConversationId { get; set; }
public Conversation Conversation { get; set; }
```

**Cascade Behavior**: `CASCADE` - When conversation is deleted, all memberships are removed

### 3. ConversationUser ↔ Message (One-to-Many)

```csharp
// In ConversationUser entity
public ICollection<Message> Messages { get; set; }

// In Message entity
public Guid ConversationUserId { get; set; }
public ConversationUser ConversationUser { get; set; }
```

**Cascade Behavior**: `CASCADE` - When conversation membership is removed, all messages are deleted

### 4. Conversation ↔ Message (One-to-Many)

```csharp
// In Conversation entity
public ICollection<Message> Messages { get; set; }

// In Message entity
public Guid ConversationId { get; set; }
public Conversation Conversation { get; set; }
```

**Cascade Behavior**: `RESTRICT` - Prevents conversation deletion if messages exist

### 5. User ↔ Friendship (Many-to-Many via Junction)

```csharp
// In Friendship entity
public string UserId1 { get; set; }
public string UserId2 { get; set; }
public User User1 { get; set; }
public User User2 { get; set; }
```

**Cascade Behavior**: `RESTRICT` - Maintains friendship history even after user account changes

### 6. User ↔ UserVoiceModel (One-to-One)

```csharp
// In User entity
public string? VoiceModelId { get; set; }
public UserVoiceModel? VoiceModel { get; set; }

// In UserVoiceModel entity (implicitly defined)
```

**Cascade Behavior**: `SET NULL` - Voice model deletion doesn't affect user account

## Indexing Strategy

### Primary Indexes

1. **All Primary Keys**: Clustered indexes for optimal performance
2. **Foreign Key Indexes**: Non-clustered indexes on all FK columns
3. **Identity Indexes**: Built-in ASP.NET Identity indexes

### Custom Indexes

1. **ConversationUser Composite**: `(ConversationId, UserId)` - Unique constraint
2. **Friendship Composite**: `(UserId1, UserId2)` - Prevents duplicate relationships
3. **Message Conversation**: `ConversationId` - Fast message retrieval
4. **User Online Status**: `IsOnline, LastSeen` - Presence queries

## Data Integrity Rules

### Constraints & Validations

1. **Required Fields**: All `[Required]` attributes enforce NOT NULL
2. **Unique Constraints**: Prevent duplicate relationships
3. **Foreign Key Constraints**: Maintain referential integrity
4. **Check Constraints**: Enum value validation (implicitly via EF)

### Business Rules Enforcement

1. **Conversation Types**: Validated at application layer
2. **Message Status Flow**: Sent → Delivered → Read progression
3. **Role Permissions**: Admin/Member role validation
4. **Voice Model Status**: Training state machine validation

## Performance Considerations

### Query Optimization

1. **Lazy Loading**: Configured for navigation properties
2. **Include Strategies**: Explicit loading for complex queries
3. **Connection Pooling**: EF Core connection pooling enabled
4. **Async Operations**: All database operations use async/await

### Scalability Features

1. **Polymorphic Queries**: Efficient TPH inheritance queries
2. **Batch Operations**: EF Core batch update/delete support
3. **Change Tracking**: Optimized for high-frequency updates
4. **Memory Management**: Proper DbContext lifecycle management

## Security Considerations

### Data Protection

1. **Connection Encryption**: TLS encrypted database connections
2. **Password Security**: ASP.NET Identity password hashing
3. **Input Validation**: EF Core parameter binding prevents SQL injection
4. **Soft Deletes**: Conversation membership using flags vs hard deletes

### Access Control

1. **Row-Level Security**: Conversation membership validation
2. **User Isolation**: All queries filtered by authenticated user context
3. **Role-Based Access**: Admin/Member permissions in conversations
4. **Blocking Mechanism**: Social graph filtering for blocked users

## Migration History & Evolution

### Key Migrations Timeline

1. **Initial Database** (2025-04-25): Basic chat functionality
2. **Duration Changes** (2025-04-25): Voice message duration tracking
3. **Friendship System** (2025-04-25): Social relationships
4. **User Data Enhancement** (2025-04-27): Rich user profiles
5. **Translation Support** (2025-06-16): Multi-language voice messages
6. **User Settings** (2025-06-17): Personalization features
7. **Blocking Features** (2025-06-27): Advanced social controls
8. **Voice Model Integration** (2025-06-27): AI voice training

### Current Schema Version

**Latest Migration**: `20250627202502_AddVoiceModelIdToUser`
**Total Tables**: 8 core tables + ASP.NET Identity tables
**Total Relationships**: 12 foreign key relationships

## File Storage Integration

### Physical File Management

```
wwwroot/
├── Uploads/
│   ├── [UserId]/
│   │   ├── audios/        # Original voice messages
│   │   └── images/        # Profile pictures
│   ├── audios/
│   │   └── [UserId]/      # User-specific audio files
│   └── profile-pictures/  # Profile picture storage
└── translated_audio/      # AI-processed voice files
```

### Database-File Relationship

- **ProfilePicturePath**: References physical file location
- **PastProfilePicturePaths**: JSON array of historical profile pictures
- **VoiceUrl**: Original voice message file path
- **TranslatedVoiceUrl**: AI-processed voice file path

## Future Schema Enhancements

### Planned Extensions

1. **Message Reactions**: Emoji reactions to messages
2. **Message Threading**: Reply-to-message functionality
3. **File Attachments**: Generic file sharing capability
4. **Call History**: WebRTC call logging
5. **Message Encryption**: End-to-end encryption support
6. **Conversation Templates**: Reusable conversation setups
7. **Advanced AI Models**: Multiple voice model support per user

### Scalability Roadmap

1. **Read Replicas**: Database read scaling
2. **Sharding Strategy**: User-based data partitioning
3. **Message Archiving**: Historical data management
4. **Caching Layer**: Redis integration for frequent queries
5. **Event Sourcing**: Message history reconstruction capability

## Conclusion

The MorphingTalk-API database schema represents a sophisticated, well-architected foundation for a modern chat application with AI-powered voice morphing capabilities. The design balances complexity with maintainability, providing robust support for real-time communication, social features, and advanced AI integration while maintaining high performance and data integrity standards. 