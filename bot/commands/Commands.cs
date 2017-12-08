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

				eb.WithTitle(name + " uses Something (" + param + ")!");
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
			
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (active != null) {
				List<Pair<string, int>> skills = _DBConnection.getSkillsLike(Context.User.Id, active, param);
				if (skills.Count > 0) {
					Pair<string, int> skill = skills[0];
					eb.WithTitle(active + " uses " + skill.First + " (" + skill.Second + ")!");
					eb.WithDescription(_roller.roll(skill.Second));
				} else {
					eb.WithTitle("Warning!");
					eb.WithDescription("Skill not found or there has been a problem.");
				}
			}

			await ReplyAsync("", false, eb);
		}
	}



	[Group("character")]
	[Alias("char")]
	public class Character : ModuleBase<SocketCommandContext> {
		private DBConnectionService _DBConnection;

		public Character(DBConnectionService dbCon) {
			_DBConnection = dbCon;
		}
		

		[Command]
		[Priority (0)]
		[Summary("Switches the active char of a user.")]
		[Alias("s", "sw", "select")]
		public async Task SwitchCharDefAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();
			if (_DBConnection.setActiveChar(Context.User.Id, param)) {
				eb.WithTitle("Switched character to " + param + ".");
				eb.WithDescription(Context.User.Mention);
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("Character not found or there has been a problem.");
			}

			await ReplyAsync("", false, eb);
		}


		[Command("create")]
		[Priority(1)]
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
		[Priority(1)]
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
		[Priority(1)]
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

	}

	[Group("skill")]
	[Alias("sk")]
	public class Skill : ModuleBase<SocketCommandContext> {
		private DBConnectionService _DBConnection;
		private Roller _roller;

		public Skill(DBConnectionService dbCon, Roller roller) {
			_DBConnection = dbCon;
			_roller = roller;
		}

		[Command]
		[Summary("Rolls using the skill.")]
		[Alias("use", "u", "roll", "r")]
		public async Task RollSkillAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (active != null) {
				List<Pair<string, int>> skills = _DBConnection.getSkillsLike(Context.User.Id, active,param);
				if (skills.Count > 0) {
					Pair<string, int> skill = skills[0];
					eb.WithTitle(active + " uses " + skill.First + " (" + skill.Second + ")!");
					eb.WithDescription(_roller.roll(skill.Second));
				} 
				else {
					eb.WithTitle("Warning!");
					eb.WithDescription("Skill not found or there has been a problem.");
				}
			} 
			else {
				eb.WithTitle("Warning!");
				eb.WithDescription("No active character or there has been a problem.");
			}

			await ReplyAsync("", false, eb);
		}

		[Command("add")]
		[Summary("Adds a new skill to the active character.")]
		[Alias("a")]
		public async Task AddSkillAsync(string skill, int lvl) {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (lvl > 1) {
				if (active != null) {
					if (_DBConnection.addSkill(Context.User.Id, active, skill, lvl)) {
						eb.WithTitle(active + " now can use " + skill + " " + lvl);
						eb.WithDescription(Context.User.Mention);
					}
					else {
						eb.WithTitle("Warning!");
						eb.WithDescription("Skill already exists or there has been a problem.");
					}
				} 
				else {
					eb.WithTitle("Warning!");
					eb.WithDescription("No active character or there has been a problem.");
				}
			}
			else {
				eb.WithTitle("Warning!");
				eb.WithDescription("Level must be higher than 1.");
			}
			await ReplyAsync("", false, eb);
		}
	}
}
