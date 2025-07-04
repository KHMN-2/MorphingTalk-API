# MorphingTalk-API Postman Collection Guide

## Overview
This comprehensive Postman collection covers all functionalities and APIs of the MorphingTalk-API system, including:
- Authentication & User Management
- Chatting & Conversations
- Voice Training
- File Management
- Call Management
- Friendship Management
- Webhooks
- Test Endpoints

## Setup Instructions

### 1. Import the Collection
1. Open Postman
2. Click "Import" button
3. Upload the `MorphingTalk-API-Postman-Collection.json` file
4. The collection will be imported with all endpoints organized by functionality

### 2. Environment Setup
Set up the following environment variables in Postman:

| Variable | Value | Description |
|----------|-------|-------------|
| `baseUrl` | `https://localhost:7218` | API base URL (adjust port if needed) |
| `authToken` | *(empty initially)* | JWT token from login response |
| `userId` | *(empty initially)* | User ID from login response |
| `conversationId` | *(empty initially)* | Conversation ID for testing |
| `messageId` | *(empty initially)* | Message ID for testing |

### 3. SSL Certificate Setup
If testing with HTTPS locally, you may need to:
- Disable SSL certificate verification in Postman settings
- Or configure proper SSL certificates for your development environment

## Testing Workflow

### Phase 1: Authentication & User Setup

#### 1. Register a New User
- **Endpoint**: `POST /api/Auth/register`
- **Purpose**: Create a test user account
- **Body Example**:
```json
{
  "email": "test@example.com",
  "password": "TestPassword123!",
  "fullName": "Test User",
  "gender": "male",
  "nativeLanguage": "en",
  "aboutStatus": "Hello, I'm testing MorphingTalk!",
  "profilePicturePath": "/uploads/profile-pictures/default.jpg"
}
```

#### 2. Login User
- **Endpoint**: `POST /api/Auth/login`
- **Purpose**: Authenticate and get JWT token
- **Important**: Copy the `token` from response and set it as `authToken` environment variable

#### 3. Get User Profile
- **Endpoint**: `GET /api/User/GetLoggedInUser`
- **Purpose**: Verify authentication and get user details
- **Important**: Copy the user ID from response and set it as `userId` environment variable

### Phase 2: File Management Testing

#### 1. Upload Test Files
Test all file upload endpoints:
- `POST /api/File/image` - Upload profile pictures or message images
- `POST /api/File/audio` - Upload audio files for voice messages
- `POST /api/File/video` - Upload video files
- `POST /api/File/document` - Upload document files

**Note**: You'll need actual files for testing. The response will give you file paths to use in other endpoints.

### Phase 3: User Management Testing

#### 1. Update User Profile
- **Endpoint**: `PUT /api/User/UpdateUser`
- **Purpose**: Test user profile updates

#### 2. Update Profile Picture
- **Endpoint**: `PUT /api/User/UpdateProfilePicture`
- **Purpose**: Test profile picture upload (form-data with file)

### Phase 4: Friendship Management Testing

#### 1. Add Friends
- **Endpoint**: `POST /api/Friendship/add/{friendEmail}`
- **Purpose**: Test friend addition
- **Note**: You'll need another registered user's email

#### 2. Get Friends List
- **Endpoint**: `GET /api/Friendship`
- **Purpose**: Verify friends were added

#### 3. Block/Unblock Users
- Test blocking and unblocking functionality
- Get blocked users list

### Phase 5: Conversation & Messaging Testing

#### 1. Create Conversation
- **Endpoint**: `POST /api/Chatting/conversations`
- **Purpose**: Create a test conversation
- **Important**: Copy the conversation ID from response and set it as `conversationId` environment variable

#### 2. Send Different Message Types

**Text Message**:
```json
{
  "type": 0,
  "text": "Hello! This is a test message.",
  "needTranslation": true
}
```

**Voice Message**:
```json
{
  "type": 1,
  "voiceFileUrl": "/uploads/audio/your-uploaded-audio.wav",
  "durationSeconds": 5,
  "useRobotVoice": false,
  "needTranslation": true
}
```

**Image Message**:
```json
{
  "type": 2,
  "imageUrl": "/uploads/images/your-uploaded-image.jpg"
}
```

#### 3. Message Management
- Get messages from conversation
- Star/unstar messages
- Delete messages
- Send reply messages

### Phase 6: Voice Training Testing

#### 1. Train Voice Model
- **Endpoint**: `POST /api/VoiceTraining/train`
- **Purpose**: Upload voice sample for training
- **Required**: Audio file (WAV, M4A, MP3, or FLAC format)

#### 2. Check Training Status
- **Endpoint**: `GET /api/VoiceTraining/status`
- **Purpose**: Monitor training progress

#### 3. Delete Voice Model
- **Endpoint**: `DELETE /api/VoiceTraining/model`
- **Purpose**: Remove trained voice model

### Phase 7: Call Management Testing

#### 1. Start Call
- **Endpoint**: `POST /api/Call/start`
- **Purpose**: Initiate a call
- **Note**: Requires another user ID as target

#### 2. Respond to Call
- **Endpoint**: `POST /api/Call/respond`
- **Purpose**: Accept or decline call

#### 3. End Call
- **Endpoint**: `POST /api/Call/end`
- **Purpose**: Terminate active call

### Phase 8: Webhook Testing

#### 1. AI Inference Result
- **Endpoint**: `POST /api/Webhook/inference-result`
- **Purpose**: Test AI translation webhook
- **Note**: This simulates external AI service callback

#### 2. AI Training Result
- **Endpoint**: `POST /api/Webhook/training-result`
- **Purpose**: Test voice training completion webhook

### Phase 9: Advanced Features Testing

#### 1. OTP Flow Testing
- Send OTP for email verification
- Verify OTP
- Test forget password flow

#### 2. Firebase Authentication
- Test Firebase ID token login
- Verify Firebase setup

#### 3. Conversation Management
- Add/remove users from conversations
- Update conversation details
- Leave/delete conversations

## Testing Tips

### 1. Authentication
- Always ensure `authToken` is set in environment variables
- Most endpoints require authentication (Authorization: Bearer token)
- Token expires after a certain time - re-login if needed

### 2. File Testing
- Keep uploaded file paths handy for message testing
- Test different file formats for voice training
- Verify file upload success before using in messages

### 3. Conversation Flow
- Create conversations before testing messages
- Add friends before creating group conversations
- Test message ordering and retrieval

### 4. Error Handling
- Test with invalid data to verify error responses
- Test authentication failures
- Test permission restrictions

### 5. Real-time Features
- Some features use SignalR for real-time updates
- Test call invitations and responses
- Test message notifications

## Common Test Scenarios

### Scenario 1: Complete User Journey
1. Register → Login → Update Profile → Upload Profile Picture
2. Add Friends → Create Conversation → Send Messages
3. Train Voice Model → Send Voice Messages
4. Test Call Features

### Scenario 2: Content Management
1. Upload various file types
2. Send different message types
3. Test message management (star, delete, reply)
4. Test conversation management

### Scenario 3: AI Features
1. Upload voice for training
2. Monitor training status
3. Send voice messages with translation
4. Test webhook callbacks

### Scenario 4: Social Features
1. Add/remove friends
2. Block/unblock users
3. Group conversation management
4. Call management

## Troubleshooting

### Common Issues
1. **401 Unauthorized**: Check if authToken is set correctly
2. **404 Not Found**: Verify endpoint URLs and IDs
3. **400 Bad Request**: Check request body format and required fields
4. **SSL Issues**: Disable SSL verification in Postman settings for local testing

### Response Codes
- **200 OK**: Success
- **201 Created**: Resource created successfully
- **400 Bad Request**: Invalid request data
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Permission denied
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server error

## Environment Variables Reference

| Variable | Usage | Set From |
|----------|-------|----------|
| `baseUrl` | All endpoints | Manual setup |
| `authToken` | Authentication | Login response |
| `userId` | User-specific operations | Login/user profile response |
| `conversationId` | Conversation operations | Create conversation response |
| `messageId` | Message operations | Send message response |

## Advanced Testing

### Performance Testing
- Test with large numbers of messages
- Test file upload with large files
- Test concurrent conversation operations

### Security Testing
- Test with invalid tokens
- Test permission boundaries
- Test input validation

### Integration Testing
- Test complete workflows end-to-end
- Test AI service integration
- Test Firebase integration

## Notes
- Some endpoints may require external services (AI service, Firebase)
- Voice training requires actual audio files
- Translation features depend on external AI service
- Real-time features use SignalR connections
- File uploads require actual files in the request body

This collection provides comprehensive testing coverage for all MorphingTalk-API functionalities. Start with basic authentication and gradually test more complex features as you build up your test data and environment. 