using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrowserDelegatedAuthDemo.Pages;

[Authorize(Policy = "UserOnly")]
public class UserPanelModel : PageModel
{
    private readonly ILogger<UserPanelModel> _logger;

    public UserPanelModel(ILogger<UserPanelModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("User panel accessed by {User}", User.Identity?.Name);
    }
}
