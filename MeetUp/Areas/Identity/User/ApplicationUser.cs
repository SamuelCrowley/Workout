using Microsoft.AspNetCore.Identity;

namespace MeetUp.Areas.Identity.User
{
    public class ApplicationUser : IdentityUser
    {
        public required string NickName { get; set; }

        public ApplicationUser()
        {
            NickName = string.Empty;
        }
    }
}
