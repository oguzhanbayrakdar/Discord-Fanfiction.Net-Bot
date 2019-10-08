using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordFFNetBot
{
    public class CommandHandler
    {
        private CommandService _cmds;

        private DiscordSocketClient _client;

        public async Task Install(DiscordSocketClient c)
        {
            _client = c;
            _cmds = new CommandService();
            await _cmds.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            _client.MessageReceived += HandleCommand;

        }

        public async Task HandleCommand(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var context = new CommandContext(_client, msg);

            int argPos = 2;
            string prefix = "ff!";
            if (msg.HasStringPrefix(prefix, ref argPos))
            {
                var result = await _cmds.ExecuteAsync(context, argPos, null);

                if (!result.IsSuccess)
                {
                    switch (result.ToString())
                    {
                        default:

                            await s.Channel.SendMessageAsync(
                                $"An error occurred! Details: ```" + result.ToString() + "```");
                            break;
                        case "UnknownCommand: Unknown command.":

                            await msg.DeleteAsync();

                            await s.Channel.SendMessageAsync(
                                $"Command not found! Use the command {prefix}help for a list of commands.");
                            break;

                    }
                }
            }
        }


    }
}