# Firebase Authentication Setup Guide

## Overview
This guide explains how to set up and use Firebase Authentication in the MorphingTalk API. Firebase Authentication provides a complete identity solution supporting multiple authentication providers including Google, Facebook, Twitter, email/password, and more.

## Prerequisites
1. Firebase account
2. Firebase project
3. Firebase service account key

## Setup Steps

### 1. Firebase Project Setup
1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Create a new project or select an existing one
3. Navigate to "Authentication" in the left sidebar
4. Click "Get started" if authentication is not yet enabled
5. Go to "Sign-in method" tab
6. Enable the authentication providers you want to use:
   - **Google**: Click Google → Enable → Configure
   - **Email/Password**: Click Email/Password → Enable
   - **Facebook**: Click Facebook → Enable → Configure with App ID and Secret
   - **Other providers**: Configure as needed

### 2. Service Account Setup
1. Go to Firebase Console → Project Settings (gear icon)
2. Navigate to "Service accounts" tab
3. Click "Generate new private key"
4. Download the JSON file (this is your service account key)
5. Store the file securely in your project (e.g., `firebase-service-account-key.json`)
6. **Important**: Add this file to your `.gitignore` to avoid committing it to version control

### 3. API Configuration
1. Update `appsettings.json` with your Firebase credentials:
```json
{
  "Firebase": {
    "ProjectId": "your-firebase-project-id",
    "ServiceAccountKeyPath": "path/to/your/firebase-service-account-key.json"
  }
}
```

### 4. Web App Configuration (Frontend)
1. In Firebase Console, go to Project Settings → General tab
2. Scroll down to "Your apps" section
3. Click "Add app" → Web app
4. Register your app and note the Firebase configuration object
5. Add Firebase to your web app:

```javascript
// Firebase configuration (from Firebase Console)
const firebaseConfig = {
  apiKey: "AIzaSyCkChbfhrIHCHBT2iGhQj9h19LKwZ-nfyE",
  authDomain: "morphing-talk-de40e.firebaseapp.com",
  projectId: "morphing-talk-de40e",
  storageBucket: "morphing-talk-de40e.firebasestorage.app",
  messagingSenderId: "987379563072",
  appId: "1:987379563072:web:f7db54d4e406741367fda5",
  measurementId: "G-37FV868XLS"
};
// Initialize Firebase
import { initializeApp } from 'firebase/app';
import { getAuth } from 'firebase/auth';

const app = initializeApp(firebaseConfig);
export const auth = getAuth(app);
```

### 5. Using Firebase Authentication

#### Frontend Integration Examples

**Google Sign-In:**
```javascript
import { signInWithPopup, GoogleAuthProvider } from 'firebase/auth';

const signInWithGoogle = async () => {
  const provider = new GoogleAuthProvider();
  try {
    const result = await signInWithPopup(auth, provider);
    const user = result.user;
    const idToken = await user.getIdToken();
    
    // Send ID token to your backend
    await sendTokenToBackend(idToken);
  } catch (error) {
    console.error('Google sign-in failed:', error);
  }
};
```

**Email/Password Sign-In:**
```javascript
import { signInWithEmailAndPassword, createUserWithEmailAndPassword } from 'firebase/auth';

// Sign in
const signIn = async (email, password) => {
  try {
    const userCredential = await signInWithEmailAndPassword(auth, email, password);
    const user = userCredential.user;
    const idToken = await user.getIdToken();
    
    // Send ID token to your backend
    await sendTokenToBackend(idToken);
  } catch (error) {
    console.error('Sign-in failed:', error);
  }
};

// Sign up
const signUp = async (email, password) => {
  try {
    const userCredential = await createUserWithEmailAndPassword(auth, email, password);
    const user = userCredential.user;
    const idToken = await user.getIdToken();
    
    // Send ID token to your backend
    await sendTokenToBackend(idToken);
  } catch (error) {
    console.error('Sign-up failed:', error);
  }
};
```

**Send Token to Backend:**
```javascript
const sendTokenToBackend = async (idToken) => {
  const response = await fetch('/api/auth/login/firebase', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      IdToken: idToken
    })
  });

  const result = await response.json();
  if (result.success) {
    // Store JWT token for future API requests
    localStorage.setItem('authToken', result.token);
    console.log('Login successful:', result.message);
  } else {
    console.error('Login failed:', result.message);
  }
};
```

#### API Endpoint
**Endpoint:** `POST /api/auth/login/firebase`

**Request Body:**
```json
{
  "IdToken": "firebase_id_token_here"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Firebase login successful",
  "token": "jwt_token_here"
}
```

### 6. How It Works

1. **User Authentication**: User authenticates with Firebase on the frontend using any supported provider
2. **ID Token**: Firebase returns an ID token (JWT) containing user information
3. **Token Verification**: Backend verifies the ID token with Firebase Admin SDK
4. **User Management**: 
   - If user exists: Returns JWT token for API access
   - If user doesn't exist: Creates new user account and returns JWT token
5. **API Authorization**: Use the returned JWT token for subsequent API calls

### 7. Security Considerations

- **Token Verification**: ID tokens are verified using Firebase Admin SDK
- **Provider Agnostic**: Works with any authentication provider enabled in Firebase
- **Token Expiry**: ID tokens expire after 1 hour by default
- **Secure Storage**: Service account keys must be stored securely
- **CORS Configuration**: Ensure your frontend domain is properly configured

### 8. Authentication Providers

Firebase supports multiple authentication providers:

- **Google**: No additional setup required
- **Facebook**: Requires Facebook App ID and Secret
- **Twitter**: Requires Twitter API credentials
- **GitHub**: Requires GitHub OAuth app
- **Email/Password**: Built-in, no additional setup
- **Phone**: SMS-based authentication
- **Anonymous**: Temporary accounts
- **Custom**: Your own authentication system

### 9. Error Handling

Common error scenarios:
- Invalid or expired Firebase ID token
- Network connectivity issues
- Firebase service account misconfiguration
- User account creation failures
- Authentication provider misconfiguration

### 10. Testing

Use the provided `FirebaseAuth.http` file to test the implementation:

```http
POST https://localhost:7128/api/auth/login/firebase
Content-Type: application/json

{
  "IdToken": "YOUR_FIREBASE_ID_TOKEN_HERE"
}
```

### 11. Frontend Libraries

**React:**
```bash
npm install firebase
```

**Vue.js:**
```bash
npm install firebase
```

**Angular:**
```bash
npm install firebase @angular/fire
```

### 12. Development vs Production

**Development:**
- Use Firebase emulator suite for local testing
- Test authentication flows without real credentials

**Production:**
- Use environment variables for sensitive configuration
- Implement proper error handling and logging
- Monitor authentication metrics in Firebase Console

## Additional Features

### Real-time User State
```javascript
import { onAuthStateChanged } from 'firebase/auth';

onAuthStateChanged(auth, (user) => {
  if (user) {
    // User is signed in
    console.log('User signed in:', user);
  } else {
    // User is signed out
    console.log('User signed out');
  }
});
```

### Sign Out
```javascript
import { signOut } from 'firebase/auth';

const handleSignOut = async () => {
  try {
    await signOut(auth);
    console.log('User signed out successfully');
  } catch (error) {
    console.error('Sign out failed:', error);
  }
};
```

## Troubleshooting

### Common Issues
1. **Invalid Service Account**: Verify the service account key file path and permissions
2. **Project ID Mismatch**: Ensure the project ID matches your Firebase project
3. **Authentication Provider Not Enabled**: Check Firebase Console authentication settings
4. **CORS Issues**: Configure allowed domains in Firebase Console
5. **Token Expiry**: Implement token refresh logic on the frontend

### Debug Steps
1. Check Firebase Console for authentication events
2. Verify service account key file is accessible
3. Test with Firebase emulator suite
4. Check server logs for detailed error messages
5. Validate Firebase project configuration

## Best Practices

1. **Security Rules**: Configure Firebase security rules appropriately
2. **Token Refresh**: Implement automatic token refresh
3. **Error Handling**: Provide meaningful error messages to users
4. **Logging**: Log authentication events for monitoring
5. **Testing**: Test all authentication flows thoroughly
6. **Backup**: Keep backup of service account keys securely 