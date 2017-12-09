
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

		public static string getHelpText (string textWallsPath) {
			try {
				TextWallPaths tw = JsonConvert.DeserializeObject<TextWallPaths>(File.ReadAllText(textWallsPath));
				return File.ReadAllText(tw.HelpPath);
			} catch (Exception) { }
			return "";
		}


	}
}
