using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace browser_delegated_auth_demo.Pages
{
    [Authorize]
    public class BuildInfoModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
