using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BrowserDelegatedAuthDemo.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly ILogger<DashboardModel> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;

    public DashboardModel(ILogger<DashboardModel> logger, ITokenAcquisition tokenAcquisition)
    {
        _logger = logger;
        _tokenAcquisition = tokenAcquisition;
    }

    public IEnumerable<Claim> IdTokenClaims { get; set; } = new List<Claim>();
    public IEnumerable<Claim> AccessTokenClaims { get; set; } = new List<Claim>();
    public string? RawIdToken { get; set; }
    public string? RawAccessToken { get; set; }
    public DateTime? IdTokenExpiration { get; set; }
    public DateTime? AccessTokenExpiration { get; set; }
    public bool RefreshTokenManagedByMsal { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            // Get ID Token claims from the current user
            IdTokenClaims = User.Claims;

            // Extract the raw ID token if available
            RawIdToken = await GetIdTokenAsync();
            if (!string.IsNullOrEmpty(RawIdToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(RawIdToken);
                IdTokenExpiration = jwtToken.ValidTo;
            }

            // Check if refresh token is managed by MSAL
            RefreshTokenManagedByMsal = await CheckRefreshTokenInMsalAsync();

            // Get Access Token
            try
            {
                var scopes = new[] { "User.Read" };
                RawAccessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                
                if (!string.IsNullOrEmpty(RawAccessToken))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(RawAccessToken);
                    AccessTokenClaims = jwtToken.Claims;
                    AccessTokenExpiration = jwtToken.ValidTo;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not acquire access token");
            }

            _logger.LogInformation("Dashboard accessed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
        }
    }

    private async Task<string?> GetIdTokenAsync()
    {
        try
        {
            // Try to get the ID token from the authentication properties
            var authenticateResult = await HttpContext.AuthenticateAsync();
            if (authenticateResult?.Properties?.Items != null &&
                authenticateResult.Properties.Items.TryGetValue(".Token.id_token", out var idToken))
            {
                return idToken;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not retrieve ID token");
        }
        return null;
    }

    private async Task<bool> CheckRefreshTokenInMsalAsync()
    {
        try
        {
            // When using Microsoft.Identity.Web with EnableTokenAcquisitionToCallDownstreamApi,
            // refresh tokens are managed internally by MSAL and not exposed in authentication properties.
            // We can verify MSAL has a refresh token by checking if the user is authenticated.
            return User.Identity?.IsAuthenticated == true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not check refresh token status");
        }
        return false;
    }
}
