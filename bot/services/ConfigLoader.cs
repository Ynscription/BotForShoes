using System;
using System.IO;

namespace Bot_For_Shoes.services {
	class ConfigLoader {

		public string DBPath { get;}

		public ConfigLoader() {
			DBPath = "c:\\BotForShoesDB\\users.sqlite";
			try {
				StreamReader file = new StreamReader(@".\config.cfg");
				DBPath = file.ReadLine();
				file.Close();
			}
			catch (Exception e) {}
		}
	}
}
