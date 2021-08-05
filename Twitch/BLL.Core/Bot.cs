using BLL.Interface;
using BLL.Interface.Events;
using System;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace BLL.Core
{
    public class Bot : IBot
    {
        public event Func<ClientLogArgs, Task> OnLog;
        
        private readonly TwitchClient _client;

        public Bot()
        {
            ConnectionCredentials credentials = new ConnectionCredentials("twitch_username", "access_token");
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            _client = new TwitchClient(customClient);
            _client.Initialize(credentials, "channel");
            
            _client.OnLog += Log;
        }

        public Task Start()
        {
            _client.Connect();
            return Task.CompletedTask;
        }

        private void Log(object? sender, OnLogArgs e)
        {
            throw new NotImplementedException();
        }
    }
}