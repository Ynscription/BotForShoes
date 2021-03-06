﻿using Discord.Commands;
using Discord;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bot_For_Shoes.bot.services;

namespace Bot_For_Shoes.bot.commands {

	public class Roll : ModuleBase<SocketCommandContext> {

		
		private DBConnectionService _DBConnection;
		private Roller _roller;
		private Random _random;
		private StringChooser _stringChooser;

		public Roll (DBConnectionService dbCon, Roller roller, Random random, StringChooser stringChooser) {
			_DBConnection = dbCon;
			_roller = roller;
			_random = random;
			_stringChooser = stringChooser;
		}
		
		[Command("roll")]
		[Priority(2)]
		[Summary("Rolls the number of d6 stated.")]
		[Alias("r")]
		public async Task RollAsync (int param = 1) {
			EmbedBuilder eb = new EmbedBuilder();
			
			if (param > 50 || param < 1) {
				eb.WithTitle("Warning!");
				eb.WithDescription("Invalid amount of dice.");
				eb.WithColor(Color.Red);
			}
			else {
				string active = _DBConnection.getActiveChar(Context.User.Id);
				if (active == null) {
					active = Context.User.Username;
					eb.WithDescription(_roller.roll(param));
				}
				else {
					int xp = _DBConnection.getCharXP(Context.User.Id, active);
					int step = _DBConnection.getCharXPStep (Context.User.Id, active);
					eb.WithDescription(_roller.roll(param, xp, step));
				}

				eb.WithTitle(active + " uses Something (" + param + ")!");				
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
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
					Pair<string, int> skill = _stringChooser.getClosestString(skills, param);
					eb.WithTitle(active + " uses " + skill.First + " (" + skill.Second + ")!");
					int xp = _DBConnection.getCharXP(Context.User.Id, active);
					int step = _DBConnection.getCharXPStep(Context.User.Id, active);
					eb.WithDescription(_roller.roll(skill.Second, xp, step));
					eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
				} else {
					eb.WithTitle("Warning!");
					eb.WithDescription("Skill not found or there has been a problem.");
					eb.WithColor(Color.Red);
				}
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("No active character or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}
	}

	
	public class About : ModuleBase<SocketCommandContext> {
		
		private ConfigService _configService;
		private StringChooser _stringChooser;
		private List<Pair<string, int>> commands;

		public About(ConfigService cfgService, StringChooser stringChooser) {
			_configService = cfgService;
			_stringChooser = stringChooser;
			commands = new List<Pair<string, int>>();
			commands.Add(new Pair<string, int>("Help", 1));
			commands.Add(new Pair<string, int>("Roll", 2));
			commands.Add(new Pair<string, int>("Char", 3));
			commands.Add(new Pair<string, int>("Skill", 4));
			commands.Add(new Pair<string, int>("XP", 5));
		}

		[Command("system")]
		[Summary("Displays information about the system.")]
		[Alias("sys")]
		public async Task SystemTextAsync() {
			EmbedBuilder eb = new EmbedBuilder();

			eb.WithTitle("**Roll for Shoes** System");
			eb.WithDescription(TextWallsService.getSystemText(_configService.TextWallPath));
			eb.WithColor(Color.DarkBlue);
			
			await ReplyAsync("", false, eb);
		}

		[Command("help")]
		[Priority(1)]
		[Summary("Displays information about the bot.")]
		[Alias("h")]
		public async Task HelpTextAsync() {
			EmbedBuilder eb = new EmbedBuilder();

			eb.WithTitle("Bot for Shoes Help");
			eb.WithDescription(TextWallsService.getHelpText(_configService.TextWallPath, 0));
			eb.WithColor(Color.DarkBlue);

			await ReplyAsync("", false, eb);
		}

		[Command("help")]
		[Priority(0)]
		[Summary("Displays information about the bot.")]
		[Alias("h")]
		public async Task HelpTextAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();

			Pair<string, int> command = _stringChooser.getClosestString(commands, param);
			if (command != null) {
				eb.WithTitle(command.First + " Help");
				eb.WithDescription(TextWallsService.getHelpText(_configService.TextWallPath, command.Second));
				eb.WithColor(Color.DarkBlue);
			}
			else {
				eb.WithTitle("Bot for Shoes Help");
				eb.WithDescription(TextWallsService.getHelpText(_configService.TextWallPath, -1));
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}

	}

	[Group("character")]
	[Alias("char")]
	public class Character : ModuleBase<SocketCommandContext> {
		private DBConnectionService _DBConnection;
		private Random _random;
		private StringChooser _stringChooser;

		public Character(DBConnectionService dbCon, Random random, StringChooser stringChooser) {
			_DBConnection = dbCon;
			_random = random;
			_stringChooser = stringChooser;
		}
		

		[Command]
		[Priority (0)]
		[Summary("Switches the active char of a user.")]
		[Alias("swi")]
		public async Task SwitchCharDefAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();

			List<string> chars = _DBConnection.getCharsLike(Context.User.Id, param);
			if (chars.Count > 0) {
				_DBConnection.setActiveChar(Context.User.Id, _stringChooser.getClosestString(chars, param));
				eb.WithTitle("Switched character to " + chars[0] + ".");
				eb.WithDescription(Context.User.Mention);
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("Character not found or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}

		[Command]
		[Priority(1)]
		[Summary("Displays info of the active char.")]
		public async Task CharInfoAsync() {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (active != null) {
				eb.WithTitle(active);
				List<Pair<string, int>> skills = _DBConnection.getSkillList(Context.User.Id, active);
				string list = "";
				if (skills.Count < 1)
					list = "No skills";
				foreach (Pair<string, int> skill in skills) {
					list += skill.First + " " + skill.Second + Environment.NewLine;
				}
				int xp = _DBConnection.getCharXP (Context.User.Id, active);
				eb.WithDescription("XP: " + xp);
				eb.AddField("Skills", list, true);
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
			} 
			else {
				eb.WithTitle("Warning!");
				eb.WithDescription("No active character or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}


		[Command("create")]
		[Priority(2)]
		[Summary("Creates a new character with the specified name.")]
		[Alias("new")]
		public async Task CreateCharAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();

			if (_DBConnection.createChar(Context.User.Id, param)) {
				eb.WithTitle("Character " + param + " created!");
				eb.WithDescription(Context.User.Mention);
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
			}
			else {
				eb.WithTitle("Warning!");
				eb.WithDescription("Character already exists or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}


		[Command("delete")]
		[Priority(2)]
		[Summary("Deletes an existing character with the specified name.")]
		[Alias("del")]
		public async Task DeleteCharAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();

			if (_DBConnection.deleteChar(Context.User.Id, param)) {
				eb.WithTitle("Character " + param + " deleted!");
				eb.WithDescription(Context.User.Mention);
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("Character does NOT exists or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}


		[Command("list")]
		[Priority(2)]
		[Summary("Lists all characters of a user.")]
		[Alias("ls")]
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
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("You have no characters or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}

	}


	[Group("skill")]
	[Alias("sk")]
	public class Skill : ModuleBase<SocketCommandContext> {
		private DBConnectionService _DBConnection;
		private Roller _roller;
		private Random _random;
		private StringChooser _stringChooser;

		public Skill(DBConnectionService dbCon, Roller roller, Random random, StringChooser stringChooser) {
			_DBConnection = dbCon;
			_roller = roller;
			_random = random;
			_stringChooser = stringChooser;
		}

		[Command]
		[Priority(1)]
		[Summary("Displays the active's char skill list.")]
		[Alias("ls", "list")]
		public async Task RollSkillAsync() {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (active != null) {
				eb.WithTitle(active + " Skills");
				List<Pair<string, int>> skills = _DBConnection.getSkillList(Context.User.Id, active);
				string list = "";
				if (skills.Count < 1)
					list = "No skills";
				foreach (Pair<string, int> skill in skills) {
					list += skill.First + " " + skill.Second + Environment.NewLine;
				}
				eb.WithDescription(list);
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("No active character or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}

		[Command]
		[Priority(0)]
		[Summary("Rolls using the skill.")]
		[Alias("use", "roll")]
		public async Task RollSkillAsync(string param) {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (active != null) {
				List<Pair<string, int>> skills = _DBConnection.getSkillsLike(Context.User.Id, active,param);
				if (skills.Count > 0) {
					Pair<string, int> skill = _stringChooser.getClosestString(skills, param);
					eb.WithTitle(active + " uses " + skill.First + " (" + skill.Second + ")!");
					int xp = _DBConnection.getCharXP(Context.User.Id, active);
					int step = _DBConnection.getCharXPStep(Context.User.Id, active);
					eb.WithDescription(_roller.roll(skill.Second, xp, step));
					eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
				} 
				else {
					eb.WithTitle("Warning!");
					eb.WithDescription("Skill not found or there has been a problem.");
					eb.WithColor(Color.Red);
				}
			} 
			else {
				eb.WithTitle("Warning!");
				eb.WithDescription("No active character or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}

		[Command("add")]
		[Summary("Adds a new skill to the active character.")]
		[Alias("new", "create")]
		public async Task AddSkillAsync(string skill, int lvl) {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (lvl > 1 && lvl <= 50) {
				if (active != null) {
					if (_DBConnection.addSkill(Context.User.Id, active, skill, lvl)) {
						eb.WithTitle(active + " can now use " + skill + " " + lvl);
						eb.WithDescription(Context.User.Mention);
						eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
					}
					else {
						eb.WithTitle("Warning!");
						eb.WithDescription("Skill already exists or there has been a problem.");
						eb.WithColor(Color.Red);
					}
				} 
				else {
					eb.WithTitle("Warning!");
					eb.WithDescription("No active character or there has been a problem.");
					eb.WithColor(Color.Red);
				}
			}
			else {
				eb.WithTitle("Warning!");
				eb.WithDescription("Level must be higher than 1 and lower than 50.");
				eb.WithColor(Color.Red);
			}
			await ReplyAsync("", false, eb);
		}

		[Command("remove")]
		[Summary("Removes a skill from the active character.")]
		[Alias("del", "delete")]
		public async Task RemoveSkillAsync(string skill) {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (active != null) {
				if (_DBConnection.removeSkill(Context.User.Id, active, skill)) {
					eb.WithTitle(active + " can no longer use " + skill);
					eb.WithDescription(Context.User.Mention);
					eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
				} else {
					eb.WithTitle("Warning!");
					eb.WithDescription("Skill does NOT exist or there has been a problem.");
					eb.WithColor(Color.Red);
				}
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("No active character or there has been a problem.");
				eb.WithColor(Color.Red);
			}
			
			await ReplyAsync("", false, eb);
		}
	}

	[Group("xp")]
	public class XP: ModuleBase<SocketCommandContext> {

		private DBConnectionService _DBConnection;
		private Random _random;

		public XP(DBConnectionService dbCon, Random random) {
			_DBConnection = dbCon;
			_random = random;
		}

		[Command]
		[Priority(1)]
		[Summary("Displays the XP of the current char.")]
		[Alias("info")]
		public async Task XPInfo() {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (active != null) {
				eb.WithTitle(active);
				int xp = _DBConnection.getCharXP(Context.User.Id, active);
				eb.WithDescription("XP: " + xp);
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("No active character or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}

		[Command]
		[Priority(0)]
		[Summary("Modifies the XP of the current char.")]
		[Alias("mod", "modify")]
		public async Task XPMod(int param) {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (active != null) {
				eb.WithTitle(active);
				int xp = _DBConnection.modifyCharXP(Context.User.Id, active, param);
				eb.WithDescription("XP: " + xp);
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("No active character or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}

		[Command("step")]
		[Priority(1)]
		[Summary("Displays the XPStep of the current char.")]
		public async Task XPStepInfo() {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (active != null) {
				eb.WithTitle(active);
				int step = _DBConnection.getCharXPStep(Context.User.Id, active);
				eb.WithDescription("XP step: " + step);
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("No active character or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}

		[Command("step")]
		[Priority(0)]
		[Summary("Sets the XP step of the current char.")]
		public async Task XPStepMod(int param) {
			EmbedBuilder eb = new EmbedBuilder();
			string active = _DBConnection.getActiveChar(Context.User.Id);
			if (active != null) {
				eb.WithTitle(active);
				int step = _DBConnection.setCharXPStep(Context.User.Id, active, param);
				eb.WithDescription("XP step: " + step);
				eb.WithColor((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256));
			} else {
				eb.WithTitle("Warning!");
				eb.WithDescription("No active character or there has been a problem.");
				eb.WithColor(Color.Red);
			}

			await ReplyAsync("", false, eb);
		}

	}

}
