# Text Translation Setup and Usage Guide

This document explains how to set up and use the new text translation feature in MorphingTalk API.

## 🚀 Features Added

### 1. **TextMessage Entity Enhancement**
- Added `IsTranslated` field to track if a message has been translated
- Added `TranslatedContent` field for backward compatibility (single translation)
- Added `TranslatedContents` dictionary for multiple language translations
- Added helper methods: `AreAllTranslationsComplete()` and `GetContentForLanguage()`

### 2. **Free Translation Service Integration**
- Created `ITextTranslationService` interface for extensibility
- **Currently using `MyMemoryTranslationService`** - **No API key required!**
- Also available: `LibreTranslateService` and `GoogleTextTranslationService`
- Supports multiple target languages in a single request
- Handles error cases gracefully (returns original text if translation fails)

### 3. **Enhanced TextMessageHandler**
- Now performs actual translations during `HandleTranslationAsync`
- Determines source language from user's `NativeLanguage` property
- Saves messages with all translations included
- Filters out source language from target languages to avoid unnecessary translations

### 4. **Updated DTOs**
- `MessageSummaryDto` now includes `TranslatedTexts` field
- `IsTranslated` field applies to both text and voice messages
- Proper mapping from entities to DTOs

## 📋 Setup Requirements

### 1. **No API Key Required! 🎉**
The current implementation uses **MyMemory API** which is completely free and doesn't require any API keys or registration.

### 2. **Ready to Use**
No additional configuration needed! The translation service works out of the box.

### 3. **Database Migration**
The migration has been applied, but if you need to apply it manually:

```bash
dotnet ef database update --project Infrastructure --startup-project MorphingTalk-API
```

## 💡 How It Works

### 1. **Message Flow**
1. User sends a text message with `NeedTranslation = true`
2. System identifies all target languages from conversation participants
3. Determines source language from sender's `NativeLanguage`
4. Calls Google Translate API for each target language
5. Saves message with all translations included
6. Sends notification to all participants

### 2. **API Request Example**
```json
{
  "Type": "Text",
  "SenderConversationUserId": "guid-here",
  "Text": "Hello, how are you?",
  "NeedTranslation": true
}
```

### 3. **Response Example**
```json
{
  "Id": "message-guid",
  "Type": "Text",
  "Text": "Hello, how are you?",
  "TranslatedTexts": {
    "es": "Hola, ¿cómo estás?",
    "fr": "Bonjour, comment allez-vous?",
    "ar": "مرحبا كيف حالك؟"
  },
  "IsTranslated": true
}
```

## 🔧 Architecture

### Services
- **`ITextTranslationService`**: Interface for text translation providers
- **`MyMemoryTranslationService`**: MyMemory API implementation (currently active)
- **`LibreTranslateService`**: LibreTranslate API implementation (alternative)
- **`GoogleTextTranslationService`**: Google Translate API implementation (alternative)
- **`TextMessageHandler`**: Handles text message processing and translation

### Database Schema
```sql
-- Added columns to Messages table
ALTER TABLE Messages ADD TranslatedContent NVARCHAR(MAX) NULL;
ALTER TABLE Messages ADD TranslatedContents NVARCHAR(MAX) NULL; -- JSON serialized dictionary
ALTER TABLE Messages ADD IsTranslated BIT NULL;
```

## 📊 Language Support

The system supports all languages supported by Google Translate API:
- Uses ISO 639-1 language codes (e.g., "en", "es", "fr", "ar")
- Automatically filters out source language from target languages
- Handles right-to-left languages properly

## 🛠️ Error Handling

### Translation Failures
- If translation fails for a specific language, it continues with other languages
- Original text is returned if all translations fail
- Comprehensive logging for debugging

### Configuration Issues
- Application will fail to start if Google Translate API key is missing
- Clear error messages for misconfiguration

## 🔄 Backward Compatibility

- `TranslatedContent` field maintains compatibility with older code
- Existing voice message translation functionality remains unchanged
- All existing APIs continue to work

## 📈 Performance Considerations

- Translations are performed in parallel for better performance
- HTTP client is properly configured with dependency injection
- Caching can be added later if needed

## 🔍 Testing

To test the translation feature:

1. Ensure you have a valid Google Translate API key
2. Create a conversation with users having different `NativeLanguage` values
3. Send a text message with `NeedTranslation = true`
4. Verify that the response includes translations for all target languages

## 🔄 Alternative Translation Services

You can easily switch between different translation providers by changing the service registration in `ServiceExtensions.cs`:

### 1. **MyMemory API (Current - FREE)**
```csharp
services.AddScoped<ITextTranslationService, MyMemoryTranslationService>();
```
- ✅ **Completely free**
- ✅ **No API key required**
- ✅ **No registration needed**
- ⚠️ Rate limits for heavy usage
- ⚠️ Translation quality may vary

### 2. **LibreTranslate (FREE Alternative)**
```csharp
services.AddScoped<ITextTranslationService, LibreTranslateService>();
```
- ✅ **Free with public instances**
- ✅ **Open source**
- ✅ **Self-hostable**
- ⚠️ May require API key for some instances
- Configure in `appsettings.json`:
```json
{
  "LibreTranslate": {
    "BaseUrl": "https://libretranslate.com",
    "ApiKey": "optional_api_key"
  }
}
```

### 3. **Google Translate (Paid)**
```csharp
services.AddScoped<ITextTranslationService, GoogleTextTranslationService>();
```
- ✅ **High quality translations**
- ✅ **Supports many languages**
- ❌ **Requires API key and billing setup**
- Configure in `appsettings.json`:
```json
{
  "GoogleTranslate": {
    "ApiKey": "YOUR_GOOGLE_TRANSLATE_API_KEY"
  }
}
```

## 🚨 Important Notes

- **Current Setup**: Using MyMemory API - completely free, no setup required
- **Rate Limits**: MyMemory has generous rate limits for free usage
- **Language Detection**: The system uses the sender's `NativeLanguage` as source language
- **Fallback**: If translation fails, the original text is returned

## 🔮 Future Enhancements

- Support for other translation providers (Azure Translator, AWS Translate)
- Caching frequently translated phrases
- Language detection for automatic source language identification
- Translation quality scoring
- Batch translation for better performance 