
using Newtonsoft.Json;
using System;
using System.IO;

namespace Bot_For_Shoes.bot.services {
	public class TextWallsService {

		public static string getSystemText (string textWallsPath) {
			try {
				TextWallPaths tw = JsonConvert.DeserializeObject<TextWallPaths>(File.ReadAllText(textWallsPath));
				return File.ReadAllText(tw.SystemPath);
			}catch (Exception) {}
			return "";
		}

		public static string getHelpText (string textWallsPath, int command) {
			try {
				TextWallPaths tw = JsonConvert.DeserializeObject<TextWallPaths>(File.ReadAllText(textWallsPath));
				TextWallHelp twh = JsonConvert.DeserializeObject<TextWallHelp>(File.ReadAllText(tw.HelpPath));
				switch (command) {
					case 0:
						return twh.Help;
					case 1:
						return twh.HelpHelp;
					case 2:
						return twh.HelpRoll;
					case 3:
						return twh.HelpChar;
					case 4:
						return twh.HelpSkill;
					case 5:
						return twh.HelpXP;
					default:
						return "Command not recognized";
				}
				
			} catch (Exception) { }
			return "";
		}


	}
}
