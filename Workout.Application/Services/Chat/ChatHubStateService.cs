using System.Collections.Concurrent;

namespace Workout.Application.Services.Chat
{
    public class ChatHubStateService
    {
        private readonly List<string> _availableColours = new();
        private readonly ConcurrentDictionary<string, ChatUser> _users = new();
        private readonly ConcurrentDictionary<string, int> _colourUsages = new();
        private readonly Random _random = new();

        public ChatHubStateService(List<string> availableColours)
        {
            _availableColours.AddRange(availableColours);

            foreach (string colour in _availableColours)
            {
                _colourUsages[colour] = 0;
            }
        }

        public void AddUser(string connectionId)
        {
            int lowestUsage = _colourUsages.Values.Min();

            List<string> leastUsedColours = _colourUsages.Where(c => c.Value == lowestUsage)
                                            .Select(c => c.Key)
                                            .ToList();

            string selectedColour = leastUsedColours[_random.Next(leastUsedColours.Count)];

            _users[connectionId] = new ChatUser(connectionId, selectedColour);
            _colourUsages[selectedColour]++;
        }

        public void RemoveUser(string connectionId)
        {
            if (_users.TryRemove(connectionId, out var user))
            {
                if (_colourUsages.ContainsKey(user.Colour))
                {
                    _colourUsages[user.Colour]--;
                }
            }
        }

        public string GetUserColour(string connectionId)
        {
            string userColour = "#000";

            if (_users.TryGetValue(connectionId, out var user))
            {
                userColour = user.Colour;
            }

            return userColour;
        }

        private class ChatUser
        {
            public string ConnectionId { get; }
            public string Colour { get; }

            public ChatUser(string connectionId, string colour)
            {
                ConnectionId = connectionId;
                Colour = colour;
            }
        }
    }
}
