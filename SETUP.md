# Setup Guide - Entra External ID Configuration

This guide provides step-by-step instructions for configuring Microsoft Entra External ID for this demo application.

## Prerequisites

- An **Entra External ID tenant** (jaimlab1eeid.onmicrosoft.com)
- Admin access to the Entra admin center
- The application already registered with Client ID: 23424267-d4c8-413b-98dc-ff96ba21cbcc

## 🔧 Step 1: Configure App Registration

### Update Redirect URIs

1. Go to [Entra admin center](https://entra.microsoft.com)
2. Navigate to **Identity** > **Applications** > **App registrations**
3. Find and click on **browser-delegated-auth-demo**
4. Go to **Authentication** in the left menu
5. Under **Platform configurations** > **Web**, add:
   - **Local Development**: `https://localhost:7234/signin-oidc`
   - **Production** (when ready): `https://your-app.azurewebsites.net/signin-oidc`

6. Under **Front-channel logout URL**, add:
   - **Local Development**: `https://localhost:7234/signout-oidc`
   - **Production**: `https://your-app.azurewebsites.net/signout-oidc`

7. Under **Implicit grant and hybrid flows**, ensure:
   - ✅ **ID tokens** is checked (for OpenID Connect)

8. Click **Save**

### Configure API Permissions

1. In the same app registration, go to **API permissions**
2. Click **Add a permission**
3. Select **Microsoft Graph**
4. Choose **Delegated permissions**
5. Search for and add:
   - ✅ `User.Read` - Read user profile
   - ✅ `openid` - OpenID Connect authentication
   - ✅ `profile` - Access user's profile information
   - ✅ `email` - Access user's email address
   - ✅ `offline_access` - Maintain access to data

6. Click **Add permissions**
7. Click **Grant admin consent for [Your Tenant]**
8. Confirm by clicking **Yes**

## 🎭 Step 2: Configure App Roles

### Add Roles to App Manifest

1. In the app registration, go to **App roles**
2. Click **Create app role** for each of the following:

#### Admin Role
- **Display name**: Admin
- **Allowed member types**: Users/Groups
- **Value**: `Admin`
- **Description**: Administrators can access all features
- **Do you want to enable this app role?**: ✅ Yes

#### Manager Role
- **Display name**: Manager
- **Allowed member types**: Users/Groups
- **Value**: `Manager`
- **Description**: Managers can supervise teams
- **Do you want to enable this app role?**: ✅ Yes

#### User Role
- **Display name**: User
- **Allowed member types**: Users/Groups
- **Value**: `User`
- **Description**: Standard users
- **Do you want to enable this app role?**: ✅ Yes

3. Click **Apply** after creating each role

## 👥 Step 3: Assign Roles to Users

### Via Enterprise Applications

1. Go to **Identity** > **Applications** > **Enterprise applications**
2. Find and click **browser-delegated-auth-demo**
3. Go to **Users and groups** in the left menu
4. Click **Add user/group**
5. Under **Users**, click **None Selected**
6. Search for and select a user
7. Under **Select a role**, choose a role (Admin, Manager, or User)
8. Click **Assign**

### Repeat for Multiple Users

To test all role scenarios:
- Assign at least one user to **Admin** role
- Assign at least one user to **Manager** role
- Assign at least one user to **User** role

## 🔄 Step 4: Configure User Flows

Your user flow is already created: **browser-delegated-auth-demo-UF**

### Verify User Flow Configuration

1. Go to **Identity** > **External Identities** > **User flows**
2. Click on **browser-delegated-auth-demo-UF**
3. Verify the following settings:

#### Identity Providers
- ✅ Email signup (for new user registration)
- ✅ Email signin (for existing users)
- 🔄 Google (to be configured in Phase 2)

#### User Attributes and Claims
Ensure these attributes are collected during sign-up:
- ✅ Display Name
- ✅ Email Address
- ✅ Given Name
- ✅ Surname

And these claims are returned in the token:
- ✅ Display Name
- ✅ Email Addresses
- ✅ Given Name
- ✅ Surname
- ✅ User's Object ID

#### Customization (Optional)
- Configure page layouts and branding
- Add custom CSS for a branded experience
- Set up email templates for password reset

## 🔑 Step 5: Manage Client Secret

### View Existing Secret

1. In the app registration, go to **Certificates & secrets**
2. Under **Client secrets**, you should see an existing secret
3. Copy the **Secret Value** (shown only once when created)
4. Note the **Secret ID** for reference

### Create New Secret (If Needed)

If the secret expires or you need a new one:

1. Go to **Certificates & secrets**
2. Click **New client secret**
3. Add a description: "Browser Auth Demo - Dev"
4. Choose expiration: 
   - 90 days (recommended for dev)
   - 6 months
   - 12 months
   - 24 months
5. Click **Add**
6. **IMPORTANT**: Copy the secret value immediately (it won't be shown again)
7. Update your user secrets:
   ```powershell
   dotnet user-secrets set "AzureAd:ClientSecret" "your-new-secret-value"
   ```

## 🌐 Step 6: Adding Google as Identity Provider (Phase 2)

### Prerequisites for Google Integration

1. **Google Cloud Project** with OAuth 2.0 credentials
2. **Authorized redirect URIs** configured in Google

### Configure in Entra External ID

1. Go to **Identity** > **External Identities** > **Identity providers**
2. Click **New Google provider**
3. Enter:
   - **Name**: Google
   - **Client ID**: [Your Google OAuth Client ID]
   - **Client Secret**: [Your Google OAuth Client Secret]
4. Click **Save**

### Update User Flow

1. Go to your user flow: **browser-delegated-auth-demo-UF**
2. Under **Identity providers**, check:
   - ✅ Email signup
   - ✅ Email signin
   - ✅ Google
3. Click **Save**

## 🧪 Step 7: Testing the Configuration

### Test 1: Basic Sign-in

1. Run the application locally
2. Click **Sign In**
3. You should be redirected to:
   ```
   https://jaimlab1eeid.ciamlogin.com/jaimlab1eeid.onmicrosoft.com/oauth2/v2.0/authorize...
   ```
4. Sign in with your credentials
5. After successful authentication, you should be redirected back to the app

### Test 2: New User Sign-up

1. On the sign-in page, click **Sign up now**
2. Enter email and verify
3. Complete the profile (name, etc.)
4. Create password
5. Complete sign-up process

### Test 3: Password Reset

1. On the sign-in page, click **Forgot your password?**
2. Enter email address
3. Verify your identity
4. Set new password

### Test 4: Role-Based Access

1. Sign in as a user with **Admin** role
2. Navigate to **Admin Panel** - should succeed
3. Sign out
4. Sign in as a user with **User** role
5. Navigate to **Admin Panel** - should show Access Denied

### Test 5: Token Claims

1. Sign in with any user
2. Navigate to **Dashboard**
3. Verify you see:
   - ID Token claims (name, email, roles, etc.)
   - Access Token claims
   - Token expiration times

## 🔍 Troubleshooting

### Issue: "AADSTS50011: The redirect URI does not match"

**Solution**: 
- Ensure redirect URI in Entra matches exactly: `https://localhost:7234/signin-oidc`
- No trailing slashes
- Correct protocol (https)
- Correct port number

### Issue: "AADSTS650052: The app needs access to a service"

**Solution**:
- Ensure API permissions are granted
- Admin consent is provided
- Wait a few minutes for permission propagation

### Issue: Roles not appearing in token

**Solution**:
1. Verify roles are assigned to the user in Enterprise Applications
2. User needs to sign out and sign in again
3. Check token in Dashboard to see if role claims are present

### Issue: Can't acquire access token

**Solution**:
1. Ensure `User.Read` permission is granted and consented
2. Check scope configuration in appsettings.json
3. Verify token cache is working properly

## 📊 Monitoring and Logs

### View Sign-in Logs

1. Go to **Identity** > **Monitoring** > **Sign-in logs**
2. Filter by application: browser-delegated-auth-demo
3. Review successful and failed sign-ins
4. Check error details if sign-ins are failing

### View Audit Logs

1. Go to **Identity** > **Monitoring** > **Audit logs**
2. Review changes to app configuration
3. Track role assignments
4. Monitor user flow modifications

## 🚀 Production Checklist

Before deploying to production:

- [ ] Update redirect URIs with production URL
- [ ] Configure custom domain (optional)
- [ ] Set up Key Vault for secret management
- [ ] Enable application insights for monitoring
- [ ] Configure IP restrictions on Azure App Service
- [ ] Set up backup authentication methods
- [ ] Configure MFA (Multi-Factor Authentication) for sensitive roles
- [ ] Test all user flows in production environment
- [ ] Set up alerts for failed authentications
- [ ] Document emergency access procedures

## 📚 Additional Resources

- [Microsoft Entra External ID Documentation](https://learn.microsoft.com/entra/external-id/)
- [Microsoft Identity Web Documentation](https://learn.microsoft.com/entra/msal/dotnet/microsoft-identity-web/)
- [App Roles Documentation](https://learn.microsoft.com/entra/identity-platform/howto-add-app-roles-in-apps)
- [User Flows Documentation](https://learn.microsoft.com/entra/external-id/customers/how-to-user-flow-sign-up-sign-in-customers)

---

**Need Help?** Check the main [README.md](README.md) for troubleshooting tips or review Microsoft's documentation.
