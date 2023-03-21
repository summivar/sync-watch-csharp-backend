using System;
using System.Linq;
using FilmRoom.Models;
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

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", $"User {userConnection.User} has left");
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task ChangeState(bool isPlaying, double time)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                State newState = new State(isPlaying, time);

                await Clients.Group(userConnection.Room)
                    .SendAsync("ChangeState", newState);
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
