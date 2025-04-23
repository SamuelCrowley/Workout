using MeetUp.Areas.Identity.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace MeetUp.wwwroot
{
    internal class ChatHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ChatHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Client calls to send message to other clients
        public async Task SendMessage(string message)
        {
            // All other clients receive the message

            string messageSenderName = await GetUserName();
            await Clients.All.SendAsync("ReceiveMessage", messageSenderName, message);
        }

        private async Task<string> GetUserName()
        {
            ApplicationUser user = await _userManager.GetUserAsync(Context.User);
            return user?.NickName ?? user?.UserName ?? "Anonymous";
        }

        public override async Task OnConnectedAsync()
        {
            string joinerName = await GetUserName();
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{joinerName} joined");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string leaverName = await GetUserName();
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{leaverName} left");
            await base.OnDisconnectedAsync(exception);
        }
    }
}