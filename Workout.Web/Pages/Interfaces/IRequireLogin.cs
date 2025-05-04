using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Workout.Infrastructure.Data.User;

namespace Workout.Web.Pages.Interfaces
{
    public interface IRequireLogin
    {
        async Task<IActionResult?> TryRedirectToLogin(UserManager<ApplicationUserEO> userManager, ClaimsPrincipal appUser)
        {
            ApplicationUserEO? user = await userManager.GetUserAsync(appUser);
            return user == null ? new RedirectToPageResult("/Account/Login") : null;
        }
    }
}
