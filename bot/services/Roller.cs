using System;

namespace Bot_For_Shoes {
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
			for (int i = 0; i < param; i++) {
				int roll = _random.Next(1, 7);
				total += roll;
				if (roll == 6)
					res += "**" + roll + "**";
				else
					res += roll;

				if (i < param - 1)
					res += ", ";
				else if (total == 6 * param)
					res += "\t**LVL UP!**";
			}
			res += "\n**Total**: " + total;
			return res;
		}
	}
}
