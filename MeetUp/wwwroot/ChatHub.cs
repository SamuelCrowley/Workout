using MeetUp.Areas.Identity.User;
using MeetUp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace MeetUp.wwwroot
{
    internal class ChatHub : Hub
    {
        private readonly ChatHubStateService _stateService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(UserManager<ApplicationUser> userManager, ChatHubStateService stateService)
        {
            _userManager = userManager;
            _stateService = stateService;
        }

        public async Task SendMessage(string message)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(Context.User);
            string senderName = GetUserName(user);
            string colour = _stateService.GetUserColour(Context.ConnectionId);

            await Clients.All.SendAsync("ReceiveMessage", senderName, message, colour);
        }

        public override async Task OnConnectedAsync()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(Context.User);
            string joinerName = GetUserName(user);

            _stateService.AddUser(Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{joinerName} joined", "#888");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(Context.User);
            string leaverName = GetUserName(user);

            _stateService.RemoveUser(Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{leaverName} left", "#888");

            await base.OnDisconnectedAsync(exception);
        }

        private string GetUserName(ApplicationUser user)
        {
            string userDisplayName = "Anonymous";

            if (user != null)
            {
                userDisplayName = !string.IsNullOrWhiteSpace(user.NickName) ? user.NickName : user.UserName;
            }

            return userDisplayName;
        }
    }
}