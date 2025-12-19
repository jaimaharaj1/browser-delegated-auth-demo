using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace BrowserDelegatedAuthDemo.Pages;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly ILogger<ProfileModel> _logger;
    private readonly GraphServiceClient _graphServiceClient;

    public ProfileModel(ILogger<ProfileModel> logger, GraphServiceClient graphServiceClient)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
    }

    public User? UserProfile { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public bool HasError { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            // Get user profile from Microsoft Graph
            UserProfile = await _graphServiceClient.Me.GetAsync();

            // Try to get profile photo
            try
            {
                var photoStream = await _graphServiceClient.Me.Photo.Content.GetAsync();
                if (photoStream != null)
                {
                    using var memoryStream = new MemoryStream();
                    await photoStream.CopyToAsync(memoryStream);
                    var photoBytes = memoryStream.ToArray();
                    ProfilePhotoUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(photoBytes)}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No profile photo available");
                // Profile photo is optional, so we don't treat this as an error
            }

            _logger.LogInformation("Profile loaded successfully for user: {Email}", UserProfile?.Mail ?? UserProfile?.UserPrincipalName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user profile from Microsoft Graph");
            HasError = true;
            ErrorMessage = $"Unable to load profile: {ex.Message}";
        }
    }
}
