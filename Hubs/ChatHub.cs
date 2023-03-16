using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs
{
    public class FilmHub : Hub
    {
        private readonly IDictionary<string, UserConnection> _connections;
        public FilmHub(IDictionary<string, UserConnection> connections)
        {
            _connections = connections;
        }

        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.GroupExcept(userConnection.Room, Context.ConnectionId)
                    .SendAsync("ReceiveMessage", message);
            }
        }

        public async Task PlayingVideo(bool play)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Groups(userConnection.Room)
                    .SendAsync("PauseState", play);
            }
        }
        public async Task RewindVideo(double seconds)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.GroupExcept(userConnection.Room, Context.ConnectionId)
                    .SendAsync("TimeState", seconds);
            }
        }
        public async Task ChangeVideo(string newURL)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Groups(userConnection.Room)
                    .SendAsync("NewVideo", newURL);
            }
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;

            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", $"{userConnection.User} has joined {userConnection.Room}");
        }
    }
}
