using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace BrowserDelegatedAuthDemo.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    public string? ErrorMessage { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(string? message = null)
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        ErrorMessage = message;
        _logger.LogError("Error page accessed. RequestId: {RequestId}, Message: {Message}", RequestId, ErrorMessage);
    }
}
