using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrowserDelegatedAuthDemo.Pages;

[Authorize(Policy = "AdminOnly")]
public class AdminPanelModel : PageModel
{
    private readonly ILogger<AdminPanelModel> _logger;

    public AdminPanelModel(ILogger<AdminPanelModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Admin panel accessed by {User}", User.Identity?.Name);
    }
}
