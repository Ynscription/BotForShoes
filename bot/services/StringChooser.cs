using F23.StringSimilarity;
using System.Collections.Generic;

namespace Bot_For_Shoes.bot.services {
	public class StringChooser {
		JaroWinkler _jw;
		public StringChooser () {
			_jw = new JaroWinkler();

		}

		public string getClosestString(List<string> list, string match) {
			string bestS = "";
			double bestV = 0;
			
			foreach (string s in list) {
				double aux = _jw.Similarity(match, s);
				if (aux > bestV) {
					bestV = aux;
					bestS = s;
				}
			}

			return bestS;
		}

		public Pair<string,int> getClosestString(List<Pair<string,int>> list, string match) {
			Pair<string, int> bestS = null;
			double bestV = 0;

			foreach (Pair<string,int> s in list) {
				double aux = _jw.Similarity(match, s.First);
				if (aux > bestV) {
					bestV = aux;
					bestS = s;
				}
			}
			return bestS;
		}
	}
}
