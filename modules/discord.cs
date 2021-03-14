using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Scout_game.modules
{
    //ISP hosted service for discord bot
    public class Discord_bot : IHostedService
    {
        private readonly DiscordSocketClient _client = new DiscordSocketClient();
        public  CommandService _command;
        private readonly IServiceProvider _services;
        readonly CommandService  _commands;
        public Discord_bot(IServiceProvider services, CommandService command)
        {
            _services = services;
            _commands = command;
        }


        //discord message handler
        public async Task Client_MessageReceived(SocketMessage arg)
        {
            int argPos = 0;

            var message = arg as SocketUserMessage;

            if (!(message.HasCharPrefix('!', ref argPos) || message.Author.IsBot))
            {
                    return;
            }
            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _services);

        }

        //log for discord.net client
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        //async start point for hosting service
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //credentials
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "credentials");
            //cred_end
            _client.Log += Log;
            var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
            _client.MessageReceived += Client_MessageReceived;
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await _commands.AddModulesAsync(GetType().Assembly, _services);

        }

        //async end point for hosting service
        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
