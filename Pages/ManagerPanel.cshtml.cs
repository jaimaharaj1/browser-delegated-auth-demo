using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrowserDelegatedAuthDemo.Pages;

[Authorize(Policy = "ManagerOnly")]
public class ManagerPanelModel : PageModel
{
    private readonly ILogger<ManagerPanelModel> _logger;

    public ManagerPanelModel(ILogger<ManagerPanelModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Manager panel accessed by {User}", User.Identity?.Name);
    }
}
