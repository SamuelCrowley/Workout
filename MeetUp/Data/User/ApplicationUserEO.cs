using MeetUp.Data.Gym;
using Microsoft.AspNetCore.Identity;

namespace MeetUp.Data.User
{
    public class ApplicationUserEO : IdentityUser
    {
        public GymUserEO GymUserEO { get; set; }
        [ProtectedPersonalData] // SEC 25-Apr-2025 - Users frequently put protected personal data inside nicknames/aliases 
        public required string NickName { get; set; }
    }
}
