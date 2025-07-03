# Fix Firebase "Unauthorized Domain" Error

## 🚨 Error: `auth/unauthorized-domain`

This error occurs because the domain you're using to test Firebase authentication is not authorized in your Firebase Console.

## 🔧 Quick Fix

### Step 1: Identify Your Domain
Depending on how you're running the HTML file, your domain could be:

- **File Protocol**: `file://` (if opening HTML file directly)
- **Local Server**: `localhost` or `127.0.0.1` (if using local server)
- **Live Server**: `localhost:5500` (if using VS Code Live Server)
- **Custom Local**: Any custom local development setup

### Step 2: Add Domain to Firebase Console

1. **Open Firebase Console**:
   - Go to [https://console.firebase.google.com/](https://console.firebase.google.com/)
   - Select your project: `morphing-talk-de40e`

2. **Navigate to Authentication Settings**:
   - Click "Authentication" in the left sidebar
   - Click "Settings" tab
   - Click "Authorized domains" section

3. **Add Your Domain**:
   - Click "Add domain"
   - Add the following domains one by one:

   ```
   localhost
   127.0.0.1
   file://
   ```

   If you're using a specific port (like Live Server), also add:
   ```
   localhost:5500
   localhost:3000
   localhost:8080
   ```

### Step 3: Common Domains to Add

Add these common development domains to be safe:

| Domain | Use Case |
|--------|----------|
| `localhost` | General local development |
| `127.0.0.1` | IP-based local access |
| `localhost:5500` | VS Code Live Server default |
| `localhost:3000` | React/Node.js default |
| `localhost:8080` | Vue.js default |
| `localhost:4200` | Angular default |
| `file://` | Direct HTML file opening |

## 🔄 Alternative Solutions

### Option 1: Use a Local Server
Instead of opening the HTML file directly, serve it from a local server:

**Using Python**:
```bash
# Python 3
python -m http.server 8000

# Python 2
python -m SimpleHTTPServer 8000
```

**Using Node.js**:
```bash
npx http-server
```

**Using VS Code Live Server**:
1. Install Live Server extension
2. Right-click on HTML file
3. Select "Open with Live Server"

### Option 2: Use Specific Local Domain
If you want to use a specific local domain:

1. **Set up local domain** (optional):
   - Edit your hosts file to add: `127.0.0.1 morphingtalk.local`
   - Add `morphingtalk.local` to Firebase authorized domains

## ✅ Verification Steps

1. **Add domains to Firebase Console**
2. **Wait 1-2 minutes** for changes to propagate
3. **Refresh your browser** or clear cache
4. **Test authentication again**

## 🚀 Test Your Setup

After adding domains, test with this simple check:

1. Open your HTML file in browser
2. Open browser developer tools (F12)
3. Try Google sign-in
4. Check console for any remaining errors

## 🛠️ Production Setup

For production deployment, add your actual domain:

```
yourdomain.com
www.yourdomain.com
yourapp.vercel.app
yourapp.netlify.app
```

## 📝 Screenshot Guide

1. **Firebase Console → Authentication → Settings → Authorized Domains**:
   
   [Add domains like this]:
   ```
   ✅ localhost
   ✅ 127.0.0.1
   ✅ localhost:5500
   ✅ file://
   ✅ your-production-domain.com
   ```

## 🆘 Still Having Issues?

If the error persists:

1. **Clear browser cache** completely
2. **Try incognito/private mode**
3. **Wait 5-10 minutes** after adding domains
4. **Check Firebase Console logs** for additional errors
5. **Verify project ID** matches in both console and code

## 🔧 Debug Commands

Test your setup with these URLs:

```bash
# Test Firebase config endpoint
curl https://localhost:7242/api/testfirebase/verify-firebase

# Check if API is running
curl https://localhost:7242/api/auth/login/firebase -X POST \
  -H "Content-Type: application/json" \
  -d '{"IdToken":"test"}'
```

After adding the authorized domains, your Firebase authentication should work properly! 🎉 