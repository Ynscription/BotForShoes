using Newtonsoft.Json;
using System;
using System.IO;

namespace Bot_For_Shoes.bot.services {
	public class ConfigService {
		
		private Config _config;

		public string Token { get {return _config.Token;} }
		public string DBPath { get { return _config.DBPath;} }
		public string TextWallPath { get { return _config.TextWallPath;} }

		public ConfigService() {
			try {
				_config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@".\config.json"));
			}
			catch (Exception) {
				throw new Exception ("Can't locate config file!");
			}
		}

		public void reload () {
			try {
				_config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@".\config.json"));
			} catch (Exception) {	}
		}

		public void reload (string cfgPath) {
			try {
				_config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(cfgPath));
			} catch (Exception) {	}
		}



	}
}
