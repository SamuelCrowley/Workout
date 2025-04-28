using MeetUp.Data.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeetUp.Views
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUserEO> _userManager;

        public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUserEO> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            ApplicationUserEO? user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Redirect authenticated users to Gym page
                return Redirect("/gym");
            }
            else
            {
                // Redirect unauthenticated users to the Register page
                return Redirect("/Account/Login");
            }
        }
    }
}
