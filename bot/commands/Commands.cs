using Discord.Commands;
using Discord;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Bot_For_Shoes {
	public class Roll : ModuleBase<SocketCommandContext> {

		
		private DBConnectionService _DBConnection;
		private Roller _roller;

		public Roll (DBConnectionService dbCon, Roller roller) {
			_DBConnection = dbCon;
			_roller = roller;
		}
		
		[Command("roll")]
		[Priority(2)]
		[Summary("Rolls the number of d6 stated.")]
		[Alias("r")]
		public async Task RollAsync (int param = 1) {
			EmbedBuilder eb = new EmbedBuilder();
			
			if (param > 20 || param < 1) {
				eb.WithTitle("Warning!");
				eb.WithDescription("Invalid amount of dice.");
			}
			else {
				string name = _DBConnection.getActiveChar(Context.User.Id);
				if (name == null)
					name = Context.User.Username;

				eb.WithTitle(name + " does something!");
				eb.WithDescription(_roller.roll(param));
			}	

			await ReplyAsync("", false, eb);
		}

		[Command("roll")]
		[Priority(1)]
		[Summary("Rolls the number of d6 of the indicated skill.")]
		[Alias("r")]
		public async Task RollAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();

			

			await ReplyAsync("", false, eb);
		}
	}



	[Group("character")]
	[Alias("char")]
	public class Chara : ModuleBase<SocketCommandContext> {
		private DBConnectionService _DBConnection;

		public Chara(DBConnectionService dbCon) {
			_DBConnection = dbCon;
		}

		[Command("create")]
		[Summary("Creates a new character with the specified name.")]
		[Alias("c", "new")]
		public async Task CreateCharAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();

			if (_DBConnection.createChar(Context.User.Id, param)) {
				eb.WithTitle("Character " + param + " created!");
				eb.WithDescription(Context.User.Mention);
			}
			else {
				eb.WithTitle("Warning!");
				eb.WithDescription("Character already exists or there has been a problem.");
			}

			await ReplyAsync("", false, eb);
		}

		[Command("delete")]
		[Summary("Deletes an existing character with the specified name.")]
		[Alias("d", "del")]
		public async Task DeleteCharAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();

			if (_DBConnection.deleteChar(Context.User.Id, param)) {
				eb.WithTitle("Character " + param + " deleted!");
				eb.WithDescription(Context.User.Mention);
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("Character does NOT exists or there has been a problem.");
			}

			await ReplyAsync("", false, eb);
		}

		[Command("list")]
		[Summary("Lists all characters of a user.")]
		[Alias("l", "ls")]
		public async Task ListCharsAsync() {
			EmbedBuilder eb = new EmbedBuilder();
			List<string> chars = _DBConnection.getCharList(Context.User.Id);
			if (chars.Count > 0) {
				eb.WithTitle("List of " + Context.User.Username + "'s characters.");
				string list = "";
				foreach (string chara in chars) {
					list += chara + Environment.NewLine;
				}
				eb.WithDescription(list);
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("You have no characters or there has been a problem.");
			}

			await ReplyAsync("", false, eb);
		}

		[Command("switch")]
		[Summary("Switches the active char of a user.")]
		[Alias("s", "sw", "select")]
		public async Task SwitchCharAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();
			if (_DBConnection.setActiveChar(Context.User.Id, param)) {
				eb.WithTitle("Switched character to "+ param +".");
				eb.WithDescription(Context.User.Mention);
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("Character not found or there has been a problem.");
			}

			await ReplyAsync("", false, eb);
		}



	}
}
