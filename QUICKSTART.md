# Quick Start Guide

Get your browser-delegated authentication demo up and running in 5 minutes!

## ⚡ Quick Setup

### 1. Install Dependencies (30 seconds)

```powershell
cd c:\GitHub\browser-delegated-auth-demo
dotnet restore
```

### 2. Configure Client Secret (1 minute)

```powershell
# Initialize user secrets
dotnet user-secrets init

# Set your client secret (IMPORTANT: Keep this secure!)
dotnet user-secrets set "AzureAd:ClientSecret" "YOUR_CLIENT_SECRET_HERE"
```

### 3. Verify Entra External ID Configuration (2 minutes)

✅ Check that your app registration has these redirect URIs:
- `https://localhost:7234/signin-oidc`

✅ Check that these API permissions are granted:
- User.Read (Microsoft Graph)
- openid, profile, email, offline_access

### 4. Run the Application (30 seconds)

```powershell
dotnet run
```

Or press `F5` in Visual Studio.

### 5. Test It Out! (2 minutes)

1. Open browser to: **https://localhost:7234**
2. Click **Sign In with Entra External ID**
3. Sign in with your credentials
4. Explore the features:
   - **Dashboard** - See your tokens
   - **Profile** - View your Graph profile
   - **Role Demos** - Test role-based access

## 🎯 What to Expect

### Home Page
- Welcome screen with feature overview
- Sign-in button if not authenticated
- Navigation to all demo features

### After Sign-In
- **Dashboard**: View ID and Access token claims
- **Profile**: See your Microsoft Graph profile
- **Role Panels**: Test Admin, User, and Manager access

## 🐛 Common Issues

### "ClientSecret is required"
**Fix**: Run the user secrets command from step 2

### "Redirect URI mismatch"
**Fix**: Ensure `https://localhost:7234/signin-oidc` is in your app registration

### "Access Denied" on role pages
**Fix**: You need roles assigned. See [SETUP.md](SETUP.md) Step 3

## 📚 Next Steps

1. ✅ Assign roles to users (see [SETUP.md](SETUP.md))
2. ✅ Test password reset flow
3. ✅ Test profile edit flow
4. ✅ Configure Google social login (Phase 2)
5. ✅ Deploy to Azure (see [README.md](README.md))

## 🆘 Need Help?

- **Detailed Setup**: See [SETUP.md](SETUP.md)
- **Full Documentation**: See [README.md](README.md)
- **Troubleshooting**: Check the Troubleshooting section in README.md

---

**You're all set! 🎉** Start exploring the features and testing the authentication flows.
