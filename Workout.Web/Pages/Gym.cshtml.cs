using Workout.Infrastructure.Data.User;
using Workout.Domain.Extensions;
using Workout.Web.Pages.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Workout.Domain.Enums;

namespace MeetUp.Views
{
    public class GymModel : PageModel, IRequireLogin
    {
        private readonly UserManager<ApplicationUserEO> _userManager;

        public GymModel(UserManager<ApplicationUserEO> userManager)
        {
            _userManager = userManager;
        }

        public List<string> DifficultyOptions { get; } =
        Enum.GetValues(typeof(RepetitionDifficulty))
            .Cast<RepetitionDifficulty>()
            .Where(x => x != RepetitionDifficulty.Unknown)
            .Select(x => x.GetDisplayName())
            .ToList();

        public async Task<IActionResult> OnGet()
        {
            IRequireLogin iRequireLogin = this;
            IActionResult? redirect = await iRequireLogin.TryRedirectToLogin(_userManager, User);

            return redirect;
        }
    }
}
