using Workout.Web.Pages.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Workout.Infrastructure.Data.User;

namespace Workout.Web.Pages
{
    public class IndexModel : PageModel, IRequireLogin
    {
        private readonly UserManager<ApplicationUserEO> _userManager;

        public IndexModel(UserManager<ApplicationUserEO> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            IActionResult? redirectResult = await ((IRequireLogin)this).TryRedirectToLogin(_userManager, User);
            return redirectResult ?? RedirectToPage("/gym");
        }
    }
}
