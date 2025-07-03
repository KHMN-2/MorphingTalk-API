# Message Features Implementation

## Overview
This document outlines the implementation of three new message features: Reply, Star, and Soft Delete functionality.

## Features Implemented

### 1. Reply to Messages
- **Description**: Users can reply to any message in a conversation
- **Implementation**:
  - Added `ReplyToMessageId` property to the `Message` entity
  - Added `ReplyToMessage` navigation property for self-referencing relationship
  - Updated all message handlers (Text, Voice, Image) to support reply functionality
  - Updated DTOs to include reply information
  - Messages now show the original message they're replying to

**API Usage**:
```json
POST /api/Chatting/conversations/{conversationId}/messages
{
  "type": "Text",
  "text": "This is a reply",
  "replyToMessageId": "guid-of-original-message",
  "needTranslation": false
}
```

### 2. Star Messages
- **Description**: Users can star/unstar messages for easy access
- **Implementation**:
  - Added `StarredBy` property (List<string>) to store user IDs who starred the message
  - Added star/unstar endpoints
  - Added endpoint to get starred messages for a conversation
  - Starred messages are excluded from starred list when deleted

**API Endpoints**:
- `POST /api/Chatting/messages/{messageId}/star` - Star a message
- `DELETE /api/Chatting/messages/{messageId}/star` - Unstar a message
- `GET /api/Chatting/conversations/{conversationId}/messages/starred` - Get starred messages

### 3. Soft Delete Messages
- **Description**: Messages are flagged as deleted instead of being permanently removed
- **Implementation**:
  - Added `IsDeleted` boolean property to Message entity
  - Added `DeletedAt` DateTime property to track when message was deleted
  - Updated delete endpoint to use soft delete
  - Deleted messages show "This message was deleted" instead of original content
  - Deleted messages are still visible in conversation history but marked as deleted
  - Deleted messages cannot be starred

**API Usage**:
```http
DELETE /api/Chatting/messages/{messageId}
```
Returns: `"Message deleted successfully"`

## Database Changes

### New Properties Added to Message Entity:
- `ReplyToMessageId` (Guid?) - Reference to the message being replied to
- `ReplyToMessage` (virtual Message) - Navigation property
- `StarredBy` (List<string>) - JSON column storing user IDs who starred the message
- `IsDeleted` (bool) - Flag indicating if message is deleted
- `DeletedAt` (DateTime?) - Timestamp when message was deleted

### Database Configuration:
- Reply relationship configured with `DeleteBehavior.Restrict`
- StarredBy property configured as JSON column using `nvarchar(max)`
- Soft delete properties configured as nullable

## API Response Changes

### MessageSummaryDto Updates:
- Added `ReplyToMessageId` and `ReplyToMessage` properties
- Added `StarredBy` list and `IsStarred` boolean
- Added `IsDeleted` boolean
- Deleted messages show "This message was deleted" as text content
- All media content (voice, image) is nullified for deleted messages

### Example Response for Deleted Message:
```json
{
  "id": "message-guid",
  "type": "Text",
  "senderId": "user-id",
  "senderDisplayName": "John Doe",
  "text": "This message was deleted",
  "sentAt": "2024-01-01T12:00:00Z",
  "isDeleted": true,
  "isStarred": false,
  "starredBy": []
}
```

## Security & Permissions

### Delete Message:
- Only message sender can delete their own messages
- User must be part of the conversation
- Returns 403 Forbidden if user doesn't have permission

### Star Message:
- Any user in the conversation can star/unstar any message
- Deleted messages cannot be starred
- Starred messages are filtered by current user

## Migration Required

A new migration needs to be created and applied to add the new database columns:
```bash
dotnet ef migrations add AddMessageReplyStarAndSoftDelete --project ../Infrastructure --startup-project .
dotnet ef database update --project ../Infrastructure --startup-project .
```

## Frontend Integration Notes

1. **Reply UI**: Show reply indicator with original message preview
2. **Star UI**: Toggle star button with visual feedback
3. **Delete UI**: Show "deleted" indicator instead of message content
4. **Starred Messages**: Add filter/view for starred messages in conversation
5. **Real-time Updates**: Consider implementing SignalR notifications for star/unstar actions 