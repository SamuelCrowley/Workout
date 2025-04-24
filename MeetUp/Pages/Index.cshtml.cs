using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeetUp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect authenticated users to Chat page
                return Redirect("/Chat");
            }
            else
            {
                // Redirect unauthenticated users to the Register page
                return Redirect("/Identity/Account/Register");
            }
        }
    }
}
