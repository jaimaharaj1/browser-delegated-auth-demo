using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? new[] { "User.Read" };

// Add session for step-up authentication tracking
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        
        // Save tokens to enable refresh token retrieval
        options.SaveTokens = true;
        
        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                // Use the custom user flow for sign-in
                context.ProtocolMessage.Scope = "openid profile email offline_access";
                
                // Support acr_values for authentication context step-up
                if (context.Properties.Items.TryGetValue("acr_values", out var acrValues))
                {
                    context.ProtocolMessage.SetParameter("acr_values", acrValues);
                }
                
                return Task.CompletedTask;
            },
            OnRemoteFailure = context =>
            {
                // Handle correlation failures from password change flows
                if (context.Failure?.Message.Contains("Correlation failed") == true)
                {
                    context.HandleResponse();
                    context.Response.Redirect("/");
                    return Task.CompletedTask;
                }
                
                context.HandleResponse();
                context.Response.Redirect($"/Error?message={Uri.EscapeDataString(context.Failure?.Message ?? "Authentication failed")}");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                context.HandleResponse();
                context.Response.Redirect($"/Error?message={Uri.EscapeDataString(context.Exception.Message)}");
                return Task.CompletedTask;
            }
        };
    })
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
    .AddInMemoryTokenCaches();

// Add authorization policies for role-based access control
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
    options.AddPolicy("RequireAuthentication", policy => policy.RequireAuthenticatedUser());
    
    // Set custom access denied path
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Dashboard");
    options.Conventions.AuthorizePage("/Profile");
})
    .AddMicrosoftIdentityUI();

// Configure cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.SlidingExpiration = true;
});

builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
