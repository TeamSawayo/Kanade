using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Android.Service
{
    public class CommandHandler
    {
        private readonly IConfiguration _config;
        private readonly CommandService _command;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _service;

        public CommandHandler(IServiceProvider service)
        {
            _config = service.GetRequiredService<IConfiguration>();
            _command = service.GetRequiredService<CommandService>();
            _client = service.GetRequiredService<DiscordSocketClient>();
            _service = service;
            _command.CommandExecuted += CommandExecutedAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _service);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if(!(rawMessage is SocketUserMessage message))
            {
                return;
            }

            if(message.Source != MessageSource.User)
            {
                return;
            }

            var argPos = 0;
            string prefix = String.Concat(_config["PREFIX"]);

            if(!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasStringPrefix(prefix, ref argPos)))
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);
            await _command.ExecuteAsync(context, argPos, _service);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command , ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
            {
                Console.WriteLine($"Command failed to execute for {context.Message} -> {result}");
                return;
            }

            if (result.IsSuccess)
            {
                Console.WriteLine($"Command {context.Message} executed -> {result}");
                return;
            }

            await context.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
        }
    }
}
