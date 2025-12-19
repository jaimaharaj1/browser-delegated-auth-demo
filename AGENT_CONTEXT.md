# Agent Context Document
## Project Session History & Technical Knowledge Base

**Last Updated:** December 19, 2025  
**Repository:** https://github.com/jaimaharaj1/browser-delegated-auth-demo

---

## 🎯 Project Purpose & Goals

This project was created to demonstrate **browser-delegated authentication with Entra External ID (CIAM)** with a specific focus on **authentication context step-up for MFA**. The primary goal was to prepare a customer demo showing how to trigger MFA challenges for specific pages using authentication contexts.

### Key Success Criteria
- ✅ Implement working authentication with Entra External ID
- ✅ Demonstrate authentication context step-up (MFA challenge on Settings page)
- ✅ Show token information (ID Token, Access Token, Refresh Token)
- ✅ Create comprehensive documentation for setup and usage
- ✅ Clean, maintainable code ready for customer demos

---

## 🏗️ Technical Architecture

### Core Technology Stack
- **Framework:** ASP.NET Core 10.0 (Razor Pages)
- **Authentication Library:** Microsoft.Identity.Web 3.3.0
- **Graph SDK:** Microsoft Graph SDK 5.56.0
- **Target Framework:** .NET 10.0
- **IDE:** Visual Studio Code with C# extensions

### Identity Provider Configuration
- **Tenant:** jaimlab1eeid.onmicrosoft.com (Entra External ID - CIAM)
- **Client ID:** 23424267-d4c8-413b-98dc-ff96ba21cbcc
- **Tenant ID:** 88340fb9-7e02-489a-9b00-e50f3c5d13b6
- **Authentication Context:** c1 (BrowserDelegatedAuth-SettingsPageMFA)
- **Redirect URI:** https://localhost:7234/signin-oidc
- **Post Logout URI:** https://localhost:7234/signout-callback-oidc

### Authentication Context Implementation
**CRITICAL:** The authentication context (c1) is configured to trigger MFA when accessing the Settings page.

**Implementation Details:**
```csharp
// In Settings.cshtml.cs
[Authorize(Policy = "ConditionalAccessPolicy")]
public class SettingsModel : PageModel
{
    // Policy requires authentication context claim: acrs=c1
}
```

**Configuration in Program.cs:**
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ConditionalAccessPolicy", policyBuilder =>
        policyBuilder.RequireClaim("acrs", "c1"));
});
```

**How It Works:**
1. User navigates to Settings page
2. Application checks for "acrs" claim with value "c1"
3. If claim is missing, authentication context step-up is triggered
4. User is redirected to Entra ID for MFA challenge
5. After MFA completion, user returns with updated token containing the "acrs" claim
6. Settings page becomes accessible

**Testing Status:** ✅ WORKING - User confirmed: "And now its working properly"

---

## 📄 Application Pages & Features

### Public Pages (No Authentication Required)
1. **Index (/)** - Landing page with app overview
2. **Error** - Generic error page
3. **AccessDenied** - Shown when authorization fails

### Authenticated Pages
4. **Dashboard (/Dashboard)** - Token viewer showing ID Token, Access Token, and Refresh Token details
5. **Profile (/Profile)** - User profile from Microsoft Graph
6. **Settings (/Settings)** - Triggers MFA via authentication context step-up ⚠️
7. **BuildInfo (/BuildInfo)** - Comprehensive project documentation
8. **Logout (/Logout)** - Custom logout handler with proper cleanup

### Role-Based Pages
9. **AdminPanel (/AdminPanel)** - Requires "Admin" role
10. **ManagerPanel (/ManagerPanel)** - Requires "Manager" role
11. **UserPanel (/UserPanel)** - Requires "User" role

### Internal Components
12. **_Layout.cshtml** - Main layout template
13. **_LoginPartial.cshtml** - Login/logout navigation component

---

## 🎫 Token Management & Dashboard Implementation

### Token Display Architecture
The Dashboard page shows three types of tokens with different levels of access:

#### 1. ID Token
- **Source:** Available in `HttpContext.User.Claims`
- **Display:** Shows all claims, raw JWT, and decoded payload
- **Expiration:** Extracted from "exp" claim
- **Decoding:** JWT structure visible (header.payload.signature)

#### 2. Access Token
- **Source:** Retrieved via `HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken)`
- **Requires:** `SaveTokens = true` in OpenIdConnect configuration
- **Display:** Shows all claims, raw JWT, and decoded payload
- **Expiration:** Extracted from "exp" claim
- **Purpose:** Used for Microsoft Graph API calls

#### 3. Refresh Token
- **Source:** ❌ NOT directly accessible
- **Management:** Handled internally by MSAL token cache
- **Display:** Shows "Managed by MSAL" badge with explanation
- **Critical Decision:** Despite setting `SaveTokens = true`, refresh tokens remain MSAL-managed by design

**Why Refresh Tokens Aren't Accessible:**
```csharp
// Attempted in Dashboard.cshtml.cs:
var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
// Result: null - refresh tokens are not exposed in authentication properties
```

**Diagnostic Findings:**
```csharp
// Available tokens in authentication properties:
// - .Token.access_token ✅
// - .Token.id_token ✅
// - .Token.refresh_token ❌ (managed by MSAL)
```

### SaveTokens Configuration
```csharp
// In Program.cs
services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        options.SaveTokens = true;  // Saves access and ID tokens
        // Note: Refresh tokens remain MSAL-managed
    });
```

### Educational Content on Dashboard
The Dashboard includes comprehensive explanation of why refresh tokens can't be decoded:
- **Opaque Format:** Unlike JWTs, refresh tokens are opaque strings
- **Server-Side Validation:** Only the authorization server can validate them
- **No Readable Claims:** They don't contain human-readable information
- **Security by Design:** Opacity prevents token inspection attacks
- **Token Rotation:** May be single-use and rotated on each refresh

---

## 🔐 Security Configuration

### Client Secret Management
- **Storage:** User Secrets (NOT in source control)
- **Location:** `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json`
- **Setup Command:**
  ```powershell
  dotnet user-secrets set "AzureAd:ClientSecret" "YOUR_CLIENT_SECRET_HERE"
  ```
- **GitHub Protection:** Secret removed from documentation files before pushing

### Known Security Advisories
⚠️ **Microsoft.Identity.Web 3.3.0** has moderate severity vulnerabilities:
- CVE-2024-35264 (Moderate)
- Recommendation: Update to latest stable version for production

### HTTPS Configuration
- **Development URL:** https://localhost:7234
- **HTTP Redirect:** Enabled
- **Certificate:** Self-signed development certificate

---

## 🛠️ Development Setup & Commands

### Initial Setup
```powershell
# Navigate to project directory
cd c:\GitHub\browser-delegated-auth-demo

# Restore dependencies
dotnet restore

# Configure client secret
dotnet user-secrets init
dotnet user-secrets set "AzureAd:ClientSecret" "YOUR_SECRET_HERE"

# Build project
dotnet build

# Run application
dotnet run
```

### Running the Application
```powershell
# Standard run (terminal blocks until stopped)
dotnet run

# Access at: https://localhost:7234

# Stop: Ctrl+C in terminal
```

### Git Operations
```powershell
# Check status
git status

# Add files
git add .

# Commit
git commit -m "Your message"

# Push to GitHub
git push origin main
```

---

## 🔧 Configuration Files

### appsettings.json
Contains non-sensitive configuration:
- Tenant ID
- Client ID
- Domain
- Instance (login.microsoftonline.com)
- CallbackPath (/signin-oidc)
- SignedOutCallbackPath (/signout-callback-oidc)
- Graph API scopes (User.Read)

### User Secrets (secrets.json)
Contains sensitive configuration:
- ClientSecret (never commit to source control)

### launchSettings.json
Development environment settings:
- Application URL (https://localhost:7234)
- Environment variables
- Browser launch settings

---

## 📚 Documentation Structure

### README.md
Quick overview with:
- Project description
- Key features
- Prerequisites
- Quick start link
- Links to detailed documentation

### QUICKSTART.md
5-minute setup guide with:
- Dependency installation
- Secret configuration
- Verification steps
- Testing instructions

### SETUP.md
Comprehensive guide covering:
- Detailed Entra ID configuration
- App registration steps
- API permissions setup
- Authentication context configuration
- Redirect URI configuration
- Client secret management

### BuildInfo Page
In-app documentation showing:
- Project overview
- Technology stack
- Feature list
- Page descriptions
- Architecture diagram
- Development notes

### AGENT_CONTEXT.md (This File)
Complete session history and technical knowledge base for AI agent continuity.

---

## 🐛 Troubleshooting & Known Issues

### Authentication Context Issues (RESOLVED ✅)
**Initial Problem:** Authentication context not triggering MFA  
**Root Cause:** Policy name mismatch and claim value  
**Solution:** 
- Changed policy from "RequireBrowserDelegatedAuth" to "ConditionalAccessPolicy"
- Verified claim name is "acrs" (not "acr")
- Confirmed claim value matches context ID "c1"

**Testing:** User confirmed working with message: "And now its working properly"

### Logout Behavior (RESOLVED ✅)
**Initial Problem:** Logout not properly ending session  
**Solution:** Created custom Logout.cshtml.cs handler:
```csharp
public async Task<IActionResult> OnGetAsync()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    return Redirect("/");
}
```

### Refresh Token Access (BY DESIGN ❌)
**Issue:** Cannot access refresh token directly from authentication properties  
**Explanation:** MSAL manages refresh tokens internally in its token cache  
**Decision:** Display "Managed by MSAL" message with educational content  
**Status:** Working as designed, not a bug

### Package Vulnerabilities (KNOWN ⚠️)
**Issue:** Microsoft.Identity.Web 3.3.0 has moderate vulnerabilities  
**Impact:** Development/demo environment - acceptable  
**Action Required:** Update to latest version for production use

---

## 🎨 UI/UX Design Decisions

### Bootstrap Theme
- Using Bootstrap 5.1.0 for consistent styling
- Custom CSS in wwwroot/css/site.css
- Responsive design for mobile compatibility

### Token Display Format
- **Cards:** Each token type in separate card
- **Tabs:** Raw JWT vs. Decoded Claims
- **Badges:** Status indicators (Valid, Expired, Managed by MSAL)
- **Alerts:** Informational boxes for explanations
- **Syntax Highlighting:** Code blocks for JWT structure

### Navigation Structure
- Top navbar with login/logout
- User display name when authenticated
- Role-based menu items (Admin, Manager, User panels)
- Authentication context indicator on Settings link

---

## 📝 Code Patterns & Conventions

### Page Model Pattern
```csharp
[Authorize]  // or [Authorize(Roles = "Admin")]
public class PageNameModel : PageModel
{
    private readonly ILogger<PageNameModel> _logger;
    
    public PageNameModel(ILogger<PageNameModel> logger)
    {
        _logger = logger;
    }
    
    public void OnGet()
    {
        // Page logic
    }
}
```

### Token Retrieval Pattern
```csharp
// Access Token
var accessToken = await HttpContext.GetTokenAsync(
    OpenIdConnectParameterNames.AccessToken);

// ID Token Claims
var claims = User.Claims.ToList();

// Specific Claim
var userEmail = User.FindFirst("preferred_username")?.Value;
```

### Graph API Call Pattern
```csharp
private readonly GraphServiceClient _graphClient;

public async Task OnGetAsync()
{
    try
    {
        var user = await _graphClient.Me.GetAsync();
        // Use user data
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Graph API call failed");
    }
}
```

---

## 🔄 Project Evolution Timeline

1. **Initial Creation** - Basic ASP.NET Core app with Entra ID authentication
2. **Authentication Context** - Added MFA step-up for Settings page
3. **Token Dashboard** - Created comprehensive token viewer
4. **ID Token Section** - Display and decode ID token
5. **Access Token Section** - Display and decode access token with SaveTokens
6. **Refresh Token Investigation** - Attempted to display refresh token
7. **MSAL Management Discovery** - Learned refresh tokens are internally managed
8. **Refresh Token Documentation** - Added educational content explaining token opacity
9. **Logout Enhancement** - Fixed logout to properly clear sessions
10. **BuildInfo Page** - Added in-app comprehensive documentation
11. **Project Cleanup** - Removed unnecessary files and code
12. **README Streamlining** - Simplified main README with links to detailed docs
13. **GitHub Preparation** - Removed secrets from documentation
14. **GitHub Push** - Successfully published to public repository
15. **Agent Context** - Created this document for future session continuity

---

## 🎯 Customer Demo Talking Points

### Authentication Context Step-Up (Primary Demo Feature)
1. **Show Normal Login** - User logs in and accesses Dashboard, Profile
2. **Navigate to Settings** - Explain this requires higher authentication level
3. **MFA Challenge** - Demonstrate the authentication context triggering MFA
4. **Token Inspection** - Show the "acrs": "c1" claim in the updated token
5. **Settings Access** - Demonstrate successful access after MFA

### Token Management
1. **Dashboard Overview** - Show all three token types
2. **ID Token** - Explain identity claims and JWT structure
3. **Access Token** - Explain API authorization and Graph integration
4. **Refresh Token** - Explain security design and MSAL management

### Security Features
1. **Client Secret Management** - Show user-secrets approach
2. **HTTPS Enforcement** - Demonstrate secure communication
3. **Token Expiration** - Show automatic calculation and display
4. **Logout Behavior** - Demonstrate proper session termination

---

## 🚀 Future Enhancement Ideas

### Potential Improvements
- [ ] Update Microsoft.Identity.Web to latest version (address vulnerabilities)
- [ ] Add token refresh functionality demonstration
- [ ] Implement multiple authentication contexts for different pages
- [ ] Add claims transformation examples
- [ ] Create admin console for user management
- [ ] Add telemetry and monitoring (Application Insights)
- [ ] Implement token caching optimization
- [ ] Add unit and integration tests
- [ ] Create Docker containerization
- [ ] Add CI/CD pipeline (GitHub Actions)

### Graph API Extensions
- [ ] Display more user profile details
- [ ] Show user's calendar events
- [ ] Display OneDrive files
- [ ] Add group membership display
- [ ] Implement delegated permissions demonstration

### Advanced Authentication
- [ ] Multiple authentication contexts (different MFA levels)
- [ ] Continuous Access Evaluation (CAE)
- [ ] Conditional Access policy evaluation
- [ ] Certificate-based authentication
- [ ] FIDO2/Passkey authentication

---

## 🧪 Testing Checklist

### Authentication Flow
- [x] Anonymous user can access home page
- [x] Login redirects to Entra ID
- [x] Successful login returns to application
- [x] User claims are properly populated
- [x] Logout clears session and redirects

### Authentication Context
- [x] Settings page triggers authentication context
- [x] MFA challenge is presented
- [x] "acrs" claim is added to token
- [x] Settings page becomes accessible
- [x] Other pages don't trigger MFA

### Token Display
- [x] ID Token displays with claims
- [x] Access Token displays with claims
- [x] Refresh Token shows MSAL management message
- [x] Token expiration calculated correctly
- [x] JWT structure properly decoded

### Authorization
- [x] Admin panel requires Admin role
- [x] Manager panel requires Manager role
- [x] User panel requires User role
- [x] Access denied page shows for unauthorized users

---

## 📞 Support & Resources

### Entra External ID Documentation
- **Overview:** https://learn.microsoft.com/entra/external-id/
- **Authentication Contexts:** https://learn.microsoft.com/entra/identity/conditional-access/concept-conditional-access-cloud-apps#authentication-context

### Microsoft Identity Platform
- **Microsoft.Identity.Web:** https://github.com/AzureAD/microsoft-identity-web
- **Graph SDK:** https://learn.microsoft.com/graph/sdks/sdks-overview
- **Token Overview:** https://learn.microsoft.com/azure/active-directory/develop/access-tokens

### Project Repository
- **GitHub:** https://github.com/jaimaharaj1/browser-delegated-auth-demo
- **Owner:** jaimaharaj1
- **Branch:** main

---

## 🎓 Key Learnings & Insights

### Authentication Context Implementation
- Authentication contexts require exact claim matching ("acrs" = "c1")
- Policy names are arbitrary but must be referenced consistently
- Claims are case-sensitive and must match exactly
- Testing requires actual user interaction (not just code review)

### Token Management in Microsoft.Identity.Web
- Refresh tokens are intentionally hidden from application code
- MSAL manages token lifecycle automatically (refresh, cache, rotation)
- `SaveTokens = true` only exposes access and ID tokens
- This is security by design, not a limitation

### Entra External ID (CIAM) Considerations
- Configuration differs slightly from Entra ID (workforce)
- User flows are simplified for external users
- Custom policies may have different capabilities
- Authentication contexts work the same as in Entra ID

### Development Best Practices
- Always use user-secrets for sensitive configuration
- Document decisions and architecture for future reference
- Test authentication flows with real users
- Keep dependencies updated for security patches
- Use meaningful commit messages for project history

---

## 📋 Quick Reference Commands

```powershell
# Build & Run
dotnet build
dotnet run

# Secrets Management
dotnet user-secrets list
dotnet user-secrets set "AzureAd:ClientSecret" "value"
dotnet user-secrets remove "AzureAd:ClientSecret"
dotnet user-secrets clear

# Package Management
dotnet restore
dotnet add package PackageName
dotnet remove package PackageName
dotnet list package --vulnerable

# Git Operations
git status
git add .
git commit -m "message"
git push origin main
git pull origin main

# GitHub CLI
gh repo view --web
gh repo edit --default-branch main
gh auth status
```

---

## 💡 Notes for Future AI Agent Sessions

### When Resuming This Project
1. **Read this file first** - Contains all context and decisions
2. **Check current branch** - Should be on `main`
3. **Verify secrets** - User-secrets must be configured locally
4. **Test authentication** - Ensure Entra ID connection works
5. **Review recent commits** - See what changed since this document

### Understanding User Intent
- User is preparing customer demos for Entra External ID
- Primary focus is authentication context step-up (MFA)
- Secondary focus is token management and security
- Documentation is important for explaining concepts

### Code Modification Guidelines
- **Don't break authentication** - Test login/logout after changes
- **Preserve token display** - Dashboard is key demo feature
- **Maintain documentation** - Update this file with major changes
- **Keep secrets safe** - Never commit sensitive data
- **Test MFA flow** - Verify Settings page still triggers authentication context

### Communication Style
- User appreciates clear explanations and technical accuracy
- User confirms understanding with phrases like "brilliant" and "perfect"
- User is technical and understands identity concepts
- Be direct and efficient with responses

---

**END OF AGENT CONTEXT DOCUMENT**

*This document should be updated whenever significant changes are made to the project architecture, configuration, or functionality.*
