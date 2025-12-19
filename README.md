# Browser Delegated Authentication Demo

A comprehensive ASP.NET Core 10.0 Razor Pages application demonstrating browser-delegated authentication with **Microsoft Entra External ID** (CIAM).

## 🎯 Key Features

- ✅ **Browser-Delegated Authentication** with Entra External ID
- ✅ **Step-Up Authentication** using Conditional Access Authentication Context (MFA for sensitive pages)
- ✅ **Microsoft Graph Integration** for user profile data
- ✅ **Role-Based Access Control** (Admin, User, Manager panels)
- ✅ **Token Viewer** (ID & Access tokens with all claims)
- ✅ **Comprehensive Build Documentation** (see Build Info page in the app)

### Microsoft Graph Integration
- ✅ **User Profile Display** - Shows user information from Microsoft Graph
- ✅ **Profile Photo** - Displays user's profile picture
- ✅ **User.Read Scope** demonstration

### Role-Based Pages
- ✅ **Admin Panel** - Requires Admin role
- ✅ **User Panel** - Requires User role
- ✅ **Manager Panel** - Requires Manager role

## 🏗️ Architecture

```
┌─────────────────┐
│   Web Browser   │
└────────┬────────┘
         │ HTTPS
         ▼
┌─────────────────────────────┐
│  ASP.NET Core Web App       │
│  (Razor Pages)              │
│                             │
│  ┌───────────────────────┐  │
│  │ Microsoft.Identity.Web│  │
│  └───────────────────────┘  │
└──────────┬──────────────────┘
           │
           ├──────────────────┐
           │                  │
           ▼                  ▼
┌──────────────────┐  ┌─────────────────┐
│ Entra External ID│  │ Microsoft Graph │
│  (CIAM Tenant)   │  │      API        │
└──────────────────┘  └─────────────────┘
```

## 📋 Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- An **Entra External ID tenant** (configured: jaimlab1eeid.onmicrosoft.com)
- **App Registration** with Client ID: 23424267-d4c8-413b-98dc-ff96ba21cbcc

## ⚙️ Quick Setup

### 1. Set Client Secret

```powershell
cd c:\GitHub\browser-delegated-auth-demo
dotnet user-secrets set "AzureAd:ClientSecret" "your-client-secret-here"
```

### 2. Configuration Details

For complete setup instructions including:
- App Registration configuration
- Authentication Context setup for step-up auth
- Conditional Access policies
- Role assignments
- MFA configuration

**See the QUICKSTART.md and SETUP.md files, or run the app and visit the Build Info page for comprehensive documentation.**

## 🚀 Running the Application

```powershell
dotnet run
```

Application URLs:
- HTTPS: https://localhost:7234
- HTTP: http://localhost:5234

## 📱 Exploring Features

Once running, explore:
- **Dashboard** - View all token claims (ID & Access tokens)
- **Profile** - Microsoft Graph API integration
- **Settings** - Step-up authentication with MFA (requires authentication context 'c1')
- **Role Demos** - Admin/Manager/User role-based access panels
- **Build Info** - Complete implementation documentation

## 📁 Project Structure

```
browser-delegated-auth-demo/
├── Pages/
│   ├── Index.cshtml                 # Landing page
│   ├── Dashboard.cshtml             # Token claims viewer
│   ├── Profile.cshtml               # Microsoft Graph profile
│   ├── Settings.cshtml              # MFA-protected (auth context)
│   ├── BuildInfo.cshtml             # Complete documentation
│   ├── AdminPanel.cshtml            # Admin role required
│   ├── UserPanel.cshtml             # User role required
│   ├── ManagerPanel.cshtml          # Manager role required
│   ├── Logout.cshtml                # Custom logout handler
│   └── Shared/
│       ├── _Layout.cshtml           # Master layout
│       └── _LoginPartial.cshtml     # Auth UI component
├── Program.cs                       # App configuration & middleware
├── appsettings.json                 # Entra External ID config
└── QUICKSTART.md / SETUP.md         # Setup documentation
```

## 🔒 Security Features

✅ **Implemented:**
- Client secrets in User Secrets (local) / Key Vault (production)
- HTTPS enforcement
- Step-up authentication with authentication context
- Role-based authorization
- Session management for auth flows
- Secure cookie configuration

## 📚 Key Technologies

- **ASP.NET Core 10.0** - Web framework
- **Razor Pages** - UI framework
- **Microsoft.Identity.Web 3.2.1** - Authentication library
- **Microsoft Graph SDK 5.56.0** - Microsoft Graph integration
- **Bootstrap 5.3.2** - UI framework
- **Entra External ID** - Identity provider

## 🐛 Troubleshooting

### Issue: "Unable to get ID token"

**Solution**: Ensure the redirect URI in Entra External ID matches exactly:
```
https://localhost:7234/signin-oidc
```

### Issue: "Access token could not be acquired"

**Solution**: 
1. Check that `User.Read` permission is granted
2. Ensure the user has consented to the app
3. Check that the scope is correctly configured in appsettings.json

### Issue: "Access Denied" on role-based pages

**Solution**: 
1. Verify the user has been assigned the required role in Entra admin center
2. Sign out and sign in again to refresh the token with new role claims

### Issue: Profile photo not loading

**Solution**: This is normal - not all users have profile photos. The app shows a placeholder icon instead.

## 📞 Support

For issues or questions:
- Check the [Microsoft Identity Web documentation](https://learn.microsoft.com/entra/identity-platform/)
- Review [Entra External ID documentation](https://learn.microsoft.com/entra/external-id/)
- Examine application logs in the console output

## 📝 License

This is a demonstration application for educational purposes.

---

**Built with ❤️ using ASP.NET Core and Microsoft Entra External ID**
