# MorphingTalk-API Endpoints Quick Reference

## Authentication Endpoints
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/Auth/register` | Register new user |
| POST | `/api/Auth/login` | User login |
| POST | `/api/Auth/login/firebase` | Firebase authentication |
| GET | `/api/Auth/SendOTP` | Send OTP for verification |
| POST | `/api/Auth/VerifyOTP` | Verify OTP |
| POST | `/api/Auth/CheckAccount` | Check if account exists |
| GET | `/api/Auth/ForgetEmail` | Initiate password reset |
| POST | `/api/Auth/VerifyOTPToForgetEmail` | Verify OTP for password reset |
| POST | `/api/Auth/ResetPassword` | Reset password |

## User Management Endpoints
| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/api/User/GetAllUsers` | Get all users |
| GET | `/api/User/GetLoggedInUser` | Get current user profile |
| GET | `/api/User/GetUserById/{id}` | Get user by ID |
| GET | `/api/User/GetUserByEmail/{email}` | Get user by email |
| PUT | `/api/User/UpdateUser` | Update user profile |
| PUT | `/api/User/UpdateUserById/{id}` | Update user by ID |
| PUT | `/api/User/UpdateProfilePicture` | Update profile picture |
| DELETE | `/api/User/DeleteLoggedUser` | Delete current user |

## Conversation Management Endpoints
| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/api/Chatting/conversations` | Get user's conversations |
| GET | `/api/Chatting/conversations/{conversationId}` | Get specific conversation |
| POST | `/api/Chatting/conversations` | Create new conversation |
| PUT | `/api/Chatting/conversations/{conversationId}` | Update conversation |
| DELETE | `/api/Chatting/conversations/{conversationId}` | Delete/leave conversation |
| POST | `/api/Chatting/conversations/{conversationId}/users` | Add user to conversation |
| DELETE | `/api/Chatting/conversations/{conversationId}/users/{email}` | Remove user from conversation |
| GET | `/api/Chatting/conversations/{conversationId}/users` | Get conversation users |

## Message Management Endpoints
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/Chatting/conversations/{conversationId}/messages` | Send message |
| GET | `/api/Chatting/conversations/{conversationId}/messages` | Get messages |
| DELETE | `/api/Chatting/messages/{messageId}` | Delete message |
| POST | `/api/Chatting/messages/{messageId}/star` | Star message |
| DELETE | `/api/Chatting/messages/{messageId}/star` | Unstar message |
| GET | `/api/Chatting/conversations/{conversationId}/messages/starred` | Get starred messages |

## Friendship Management Endpoints
| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/api/Friendship` | Get friends list |
| GET | `/api/Friendship/blocked` | Get blocked users |
| POST | `/api/Friendship/add/{friendEmail}` | Add friend |
| DELETE | `/api/Friendship/remove/{friendEmail}` | Remove friend |
| POST | `/api/Friendship/block/{userEmail}` | Block user |
| POST | `/api/Friendship/unblock/{userEmail}` | Unblock user |

## Voice Training Endpoints
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/VoiceTraining/train` | Train voice model |
| GET | `/api/VoiceTraining/status` | Get training status |
| DELETE | `/api/VoiceTraining/model` | Delete voice model |

## File Management Endpoints
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/File/document` | Upload document |
| POST | `/api/File/image` | Upload image |
| POST | `/api/File/video` | Upload video |
| POST | `/api/File/audio` | Upload audio |
| DELETE | `/api/File/{*relativePath}` | Delete file |

## Call Management Endpoints
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/Call/start` | Start call |
| POST | `/api/Call/respond` | Respond to call |
| POST | `/api/Call/end` | End call |
| GET | `/api/Call/status/{conversationId}` | Get call status |

## Webhook Endpoints
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/Webhook/inference-result` | AI inference result callback |
| POST | `/api/Webhook/training-result` | AI training result callback |

## Test Endpoints
| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/api/TestFirebase/verify-firebase` | Verify Firebase setup |
| POST | `/api/TestFirebase/test-token` | Test Firebase token |
| POST | `/api/Test/webhook-test` | Test webhook payload |

## Authentication Requirements
- 🔒 **Requires Authentication**: Most endpoints require Bearer token
- 🔓 **No Authentication**: 
  - All Auth endpoints
  - Test endpoints
  - Webhook endpoints
  - Some User endpoints (GetAllUsers, GetUserById, GetUserByEmail)

## Common Request/Response Formats

### Standard Response Format
```json
{
  "data": {},
  "message": "Success message",
  "success": true,
  "statusCode": 200
}
```

### Authentication Response
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error message",
  "statusCode": 400
}
```

## Message Types
- **Type 0**: Text Message
- **Type 1**: Voice Message
- **Type 2**: Image Message

## Supported File Formats
- **Audio**: WAV, M4A, MP3, FLAC
- **Image**: JPG, PNG, GIF
- **Video**: MP4, AVI, MOV
- **Document**: PDF, DOC, DOCX, TXT

## Status Codes
- **200**: Success
- **201**: Created
- **204**: No Content
- **400**: Bad Request
- **401**: Unauthorized
- **403**: Forbidden
- **404**: Not Found
- **409**: Conflict
- **500**: Internal Server Error

## Base URL
- **Development**: `https://localhost:7218`
- **Production**: Update as needed

## Notes
- All authenticated endpoints require `Authorization: Bearer {token}` header
- File uploads use `multipart/form-data` content type
- JSON requests use `application/json` content type
- Some endpoints depend on external services (AI service, Firebase)
- Real-time features use SignalR for notifications 