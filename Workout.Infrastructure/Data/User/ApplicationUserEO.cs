using Microsoft.AspNetCore.Identity;
using Workout.Domain.Data.Base.Interfaces;
using Workout.Infrastructure.Data.Gym;
using Workout.Domain.Data.Interfaces.Gym;

namespace Workout.Infrastructure.Data.User
{
    public class ApplicationUserEO : IdentityUser, IApplicationUserEO
    {
        #region EF navigation
        public GymUserEO GymUserEO { get; set; } 
        IGymUserData IApplicationUserEO.GymUserEO => GymUserEO;
        #endregion


        [ProtectedPersonalData] // SEC 25-Apr-2025 - Users frequently put protected personal data inside nicknames/aliases 
        public required string NickName { get; set; }
        
    }
}
