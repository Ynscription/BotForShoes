using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System;

namespace Bot_For_Shoes.bot.commands {
	class Commander {

		private CommandService _commands;
		private DiscordSocketClient _client;
		private IServiceProvider _services;

		public Commander (DiscordSocketClient client) {
			_commands = new CommandService();
			_client = client;
		}

		public async Task InstallCommandsAsync (IServiceProvider services) {
			_services = services;
			_client.MessageReceived += HandleCommandAsync;
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
		}

		private async Task HandleCommandAsync (SocketMessage param) {
			var msg = param as SocketUserMessage;
			if (msg != null) {
				if (!msg.Author.IsBot) {
					int argPos = 0;
					if (msg.HasCharPrefix(';', ref argPos)) {
						SocketCommandContext context = new SocketCommandContext(_client, msg);
						var res = await _commands.ExecuteAsync(context, argPos, _services);
						if (!res.IsSuccess)
							await context.Channel.SendMessageAsync(res.ErrorReason);
					}
				}
			}
		}
	}
}
