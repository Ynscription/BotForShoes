using System;

namespace Bot_For_Shoes.bot.services {
	public class Roller {

		private Random _random;

		public Roller () {
			_random = new Random();
		}

		public Roller(int seed) {
			_random = new Random(seed);
		}

		public Roller (Random random) {
			_random = random;
		}

		public string roll (int param) {
			string res = "";
			res = "Result " + param + "d6: ";
			int total = 0;
			int crits = 0;
			for (int i = 0; i < param; i++) {
				int roll = _random.Next(1, 7);
				total += roll;
				if (roll == 6) {
					crits++;
					res += "**" + roll + "**";
				}
				else
					res += roll;

				if (i < param - 1)
					res += ", ";
				else if (crits == param)
					res += "\t**LVL UP!**";
			}
			res += "\n**Total**: " + total;
			return res;
		}

		public string roll(int param, int xp) {
			string res = "";
			res = "Result " + param + "d6: ";
			int total = 0;
			int crits = 0;
			for (int i = 0; i < param; i++) {
				int roll = _random.Next(1, 7);
				total += roll;
				if (roll == 6) {
					crits++;
					res += "**" + roll + "**";
				}
				else
					res += roll;

				if (i < param - 1)
					res += ", ";
				else if (crits == param)
					res += "\t**LVL UP!**";
				else if (param - crits <= xp) {
					res += "\t(Spend " + (param - crits) + " XP to lvl up)";
				}
			}
			res += "\n**Total**: " + total;
			return res;
		}
	}
}
