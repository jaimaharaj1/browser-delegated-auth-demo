using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BrowserDelegatedAuthDemo.Pages;

[Authorize]
public class SettingsModel : PageModel
{
    private readonly ILogger<SettingsModel> _logger;

    public SettingsModel(ILogger<SettingsModel> logger)
    {
        _logger = logger;
    }

    public bool IsMfaCompleted { get; set; }
    public List<string> AuthenticationMethods { get; set; } = new();
    public string? Email { get; set; }
    public string? DisplayName { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Log all claims for debugging
        _logger.LogInformation("=== All Claims in Token ===");
        foreach (var claim in User.Claims)
        {
            _logger.LogInformation($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }
        _logger.LogInformation("=== End Claims ===");

        // Required authentication context ID
        const string requiredAuthContextId = "c1";

        // Step 1: Check if user ALREADY has the required authentication context in current token
        // This follows the pattern from Microsoft's sample: check acrs claim first
        var acrsClaim = User.FindFirst("acrs");
        bool hasRequiredAuthContext = false;

        if (acrsClaim != null)
        {
            _logger.LogInformation($"Found 'acrs' claim with value: {acrsClaim.Value}");
            if (acrsClaim.Value == requiredAuthContextId)
            {
                _logger.LogInformation($"✓ Authentication Context '{requiredAuthContextId}' satisfied via 'acrs' claim");
                hasRequiredAuthContext = true;
                IsMfaCompleted = true;
            }
        }

        // Also check 'acr' claim as alternative
        if (!hasRequiredAuthContext)
        {
            var acrClaim = User.FindFirst("acr");
            if (acrClaim != null)
            {
                _logger.LogInformation($"Found 'acr' claim with value: {acrClaim.Value}");
                if (acrClaim.Value == requiredAuthContextId)
                {
                    _logger.LogInformation($"✓ Authentication Context '{requiredAuthContextId}' satisfied via 'acr' claim");
                    hasRequiredAuthContext = true;
                    IsMfaCompleted = true;
                }
            }
        }

        // Step 2: If auth context is satisfied, allow access
        if (hasRequiredAuthContext)
        {
            // Get user information
            Email = User.FindFirst("email")?.Value ?? User.FindFirst("preferred_username")?.Value;
            DisplayName = User.FindFirst("name")?.Value ?? User.Identity?.Name;

            // Parse authentication methods
            var amrClaim = User.FindFirst("amr");
            if (amrClaim != null)
            {
                var amrValues = System.Text.Json.JsonSerializer.Deserialize<string[]>(amrClaim.Value);
                if (amrValues != null)
                {
                    AuthenticationMethods = amrValues.ToList();
                }
            }

            _logger.LogInformation("Settings page accessed successfully with required authentication context");
            return Page();
        }

        // Step 3: Auth context NOT satisfied - check if we already tried to challenge
        var attemptedStepUp = HttpContext.Session.GetString("StepUpAttempted");
        
        if (attemptedStepUp == "true")
        {
            // User came back from challenge but still doesn't have required auth context
            _logger.LogError($"Step-up authentication attempted but auth context '{requiredAuthContextId}' not found in token. Access denied.");
            HttpContext.Session.Remove("StepUpAttempted");
            return RedirectToPage("/AccessDenied", new { reason = $"Authentication context '{requiredAuthContextId}' requirement not satisfied. MFA may not be configured or policy not applied." });
        }

        // Step 4: Trigger step-up authentication challenge with claims payload
        _logger.LogInformation($"Authentication context '{requiredAuthContextId}' not found in token. Triggering step-up authentication...");
        HttpContext.Session.SetString("StepUpAttempted", "true");

        // Create claims challenge following Microsoft sample pattern
        var claimsChallenge = $"{{\"id_token\":{{\"acrs\":{{\"essential\":true,\"value\":\"{requiredAuthContextId}\"}}}}}}";
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/Settings",
            Items =
            {
                { "claims", claimsChallenge },
                { "acr_values", requiredAuthContextId }
            }
        };

        return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
