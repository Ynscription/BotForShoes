using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace Bot_For_Shoes {
	class BotForShoes {
		private DiscordSocketClient _client;
		private Commander _commander;
		private readonly IServiceCollection _map = new ServiceCollection();
		private DBConnectionService _DBConnection;
		private readonly string _token = "Mzg4NDY3NTM1NjM0NDk3NTM2.DQtcGg.5EkBJPRZE0iBibkUM40qamJ5LLg";

		public BotForShoes() {
			_client = new DiscordSocketClient();
			_client.Log += Log;
			_map.AddSingleton(_client);

			Random random = new Random();
			_map.AddSingleton(random);
			_map.AddSingleton(new Roller(random));			

			_DBConnection = new DBConnectionService();
			_map.AddSingleton(_DBConnection);
			
			_commander = new Commander(_client);
		}

		public async Task launch() {
			IServiceProvider services = _map.BuildServiceProvider();
			await _commander.InstallCommandsAsync(services);

			await _client.LoginAsync(TokenType.Bot, _token);
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
