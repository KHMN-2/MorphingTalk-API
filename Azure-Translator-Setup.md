# Azure Translator Service Setup

This document explains how to set up and configure the Azure Translator service in the MorphingTalk API.

## Overview

The Azure Translator service has been implemented as a replacement for the existing translation services. It provides high-quality, real-time text translation using Microsoft's Azure Cognitive Services.

## Prerequisites

1. **Azure Account**: You need an active Azure subscription
2. **Azure Translator Resource**: Create a Translator resource in the Azure Portal

## Azure Portal Setup

### Step 1: Create a Translator Resource

1. Sign in to the [Azure Portal](https://portal.azure.com)
2. Click **Create a resource**
3. Search for "Translator" and select **Translator**
4. Click **Create**
5. Fill in the required information:
   - **Subscription**: Select your Azure subscription
   - **Resource Group**: Create a new resource group or select an existing one
   - **Region**: Choose a region (e.g., "East US", "West Europe")
   - **Name**: Enter a unique name for your Translator resource
   - **Pricing Tier**: Select your pricing tier (F0 for free tier, S1 for standard)
6. Click **Review + Create**, then **Create**

### Step 2: Get Your Credentials

1. After deployment, go to your Translator resource
2. In the left sidebar, click **Keys and Endpoint**
3. Copy the following information:
   - **Key 1** (or Key 2) - this is your subscription key
   - **Location/Region** - the region where your resource is deployed
   - **Endpoint** - should be `https://api.cognitive.microsofttranslator.com`

## Application Configuration

### Step 1: Update Configuration Files

Update the following configuration files with your Azure Translator credentials:

#### appsettings.json
```json
{
  "AzureTranslator": {
    "SubscriptionKey": "YOUR_ACTUAL_SUBSCRIPTION_KEY",
    "SubscriptionRegion": "YOUR_ACTUAL_REGION",
    "Endpoint": "https://api.cognitive.microsofttranslator.com"
  }
}
```

#### appsettings.Development.json
```json
{
  "AzureTranslator": {
    "SubscriptionKey": "YOUR_DEVELOPMENT_SUBSCRIPTION_KEY",
    "SubscriptionRegion": "YOUR_DEVELOPMENT_REGION",
    "Endpoint": "https://api.cognitive.microsofttranslator.com"
  }
}
```

### Step 2: Environment Variables (Optional)

For production environments, you can also set these as environment variables:

```bash
AzureTranslator__SubscriptionKey=your_subscription_key
AzureTranslator__SubscriptionRegion=your_region
AzureTranslator__Endpoint=https://api.cognitive.microsofttranslator.com
```

## Service Implementation

The Azure Translator service implements the `ITextTranslationService` interface and provides the following methods:

- `TranslateTextAsync(string text, string sourceLanguage, List<string> targetLanguages)`: Translates text to multiple target languages
- `TranslateTextToLanguageAsync(string text, string sourceLanguage, string targetLanguage)`: Translates text to a single target language

## Usage Example

The service is automatically registered and can be injected into your controllers or services:

```csharp
public class YourController : ControllerBase
{
    private readonly ITextTranslationService _translationService;

    public YourController(ITextTranslationService translationService)
    {
        _translationService = translationService;
    }

    public async Task<IActionResult> TranslateText(string text, string from, string to)
    {
        var result = await _translationService.TranslateTextToLanguageAsync(text, from, to);
        return Ok(result);
    }
}
```

## Supported Languages

Azure Translator supports over 100 languages. Some commonly used language codes include:

- `en` - English
- `es` - Spanish
- `fr` - French
- `de` - German
- `it` - Italian
- `pt` - Portuguese
- `ru` - Russian
- `ja` - Japanese
- `zh` - Chinese (Simplified)
- `ar` - Arabic

For a complete list of supported languages, visit the [Azure Translator documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/language-support).

## Pricing

Azure Translator pricing is based on the number of characters translated:

- **Free Tier (F0)**: 2 million characters per month
- **Standard Tier (S1)**: Pay-per-use pricing
- **Volume Discount Plans**: Available for high-volume usage

For current pricing details, visit the [Azure Translator pricing page](https://azure.microsoft.com/en-us/pricing/details/cognitive-services/translator/).

## Error Handling

The service includes comprehensive error handling:

- **Configuration Errors**: Throws `ArgumentException` if subscription key is missing
- **API Errors**: Logs errors and returns original text if translation fails
- **Network Errors**: Handles timeouts and connection issues gracefully

## Logging

The service uses `ILogger` for comprehensive logging:

- **Information**: Successful translations
- **Warning**: Empty text or configuration issues
- **Error**: API errors and exceptions

## Security Best Practices

1. **Never commit credentials**: Don't commit actual subscription keys to version control
2. **Use environment variables**: Store credentials in environment variables for production
3. **Rotate keys regularly**: Regenerate your subscription keys periodically
4. **Use Azure Key Vault**: For enhanced security, consider using Azure Key Vault to store credentials

## Troubleshooting

### Common Issues

1. **401 Unauthorized**: Check your subscription key
2. **403 Forbidden**: Check your subscription region
3. **429 Too Many Requests**: You've exceeded your quota
4. **500 Internal Server Error**: Check your endpoint URL

### Debug Steps

1. Verify your subscription key is correct
2. Ensure the region matches your Azure resource
3. Check that your Azure resource is active and not suspended
4. Verify you haven't exceeded your quota limits

## Migration from Previous Services

If you're migrating from `LibreTranslateService` or `MyMemoryTranslationService`, no code changes are required in your controllers or services that use `ITextTranslationService`. The interface remains the same, ensuring backward compatibility.

## Support

For issues related to:
- **Azure Translator Service**: Contact Azure Support
- **Implementation Issues**: Check the application logs or contact your development team
- **API Documentation**: Visit the [Azure Translator REST API documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-translate) 