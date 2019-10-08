using System;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordFFNetBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        private async Task Start()
        {
            _client = new DiscordSocketClient();
            _handler = new CommandHandler();
            await _client.LoginAsync(Discord.TokenType.Bot,
                "Token Goes Here", true);

            await _client.StartAsync();

            await _handler.Install(_client);
            _client.Ready += Client_Ready;

            await Task.Delay(-1);
        }

        private async Task Client_Ready()
        {

            Console.WriteLine("Discord Fanfiction.net Story bot is ready!");
        }
    }
}
