using System;
using Discord;
using Microsoft.Extensions.Configuration;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Net.Http;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Android.Service;

namespace Android
{
    class Program
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            var _builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(path: "botconfig.json");
            _config = _builder.Build();
        }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;
                client.Log += LogAsync;
                client.Ready += ReadyAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;
                await client.LoginAsync(TokenType.Bot, _config["TOKEN"]);
                await client.StartAsync();
                await services.GetRequiredService<CommandHandler>().InitializeAsync();
                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage Log)
        {
            Console.WriteLine(Log.ToString());
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            Console.WriteLine("Connected");
            await _client.SetGameAsync("Spotify", null, ActivityType.Listening);
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton(_config)
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
