using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Bot_For_Shoes.bot.commands;
using Bot_For_Shoes.bot.services;

namespace Bot_For_Shoes.bot {
	class BotForShoes {
		private DiscordSocketClient _client;
		private Commander _commander;
		private ConfigService _cfgService;
		private readonly IServiceCollection _map = new ServiceCollection();
		private DBConnectionService _DBConnection;

		public BotForShoes() {
			_cfgService = new ConfigService();
			_map.AddSingleton(_cfgService);

			_client = new DiscordSocketClient();
			_client.Log += Log;
			_map.AddSingleton(_client);

			Random random = new Random();
			_map.AddSingleton(random);
			_map.AddSingleton(new Roller(random));			

			_DBConnection = new DBConnectionService(_cfgService.DBPath);
			_map.AddSingleton(_DBConnection);

			_map.AddSingleton(new StringChooser());
						
			_commander = new Commander(_client);
		}

		public async Task launch() {
			IServiceProvider services = _map.BuildServiceProvider();
			await _commander.InstallCommandsAsync(services);

			await _client.LoginAsync(TokenType.Bot, _cfgService.Token);
			await _client.StartAsync();

			await Closure();
		}

		private Task Closure () {
			string input = "";
			while (input != "quit") {
				input = Console.ReadLine();
			}
			_DBConnection.close();
			return Task.CompletedTask;
		}



		private Task Log(LogMessage msg) {
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
		
	}
}
