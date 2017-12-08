using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Bot_For_Shoes.bot.services {
	public class DBConnectionService {

		private SQLiteConnection _DBConnection;
		
		public DBConnectionService () {
			_DBConnection = new SQLiteConnection("Data Source=e:\\BotForShoesDB\\users.sqlite; Version=3; New = True;");
			_DBConnection.Open();
			new SQLiteCommand("CREATE TABLE if not exists Users (id INTEGER PRIMARY KEY, active TEXT);", _DBConnection).ExecuteNonQuery();
			new SQLiteCommand("CREATE TABLE if not exists Chars (id INTEGER, char TEXT, xp INTEGER, PRIMARY KEY (id, char));", _DBConnection).ExecuteNonQuery();
			new SQLiteCommand("CREATE TABLE if not exists Skills (id INTEGER, char TEXT, skill TEXT, lvl INTEGER, PRIMARY KEY (id, char, skill));", _DBConnection).ExecuteNonQuery();
		}

		public string getActiveChar (ulong user) {
			string res = null;
			string getChar = "SELECT active FROM Users WHERE id = @user;";
			SQLiteCommand command = new SQLiteCommand(getChar, _DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			SQLiteDataReader chara = command.ExecuteReader();
			if (chara.Read()) {
				res = chara.GetString(0);
			}
			return res;
		}

		public bool createChar (ulong user, string chara) {
			bool success = false;
			string checkChar = "SELECT * FROM Chars WHERE id = @user AND char = @chara;";
			string insertChar = "INSERT INTO Chars (id, char, xp) VALUES (@user, @chara, 0);";
			string insertActive = "INSERT OR REPLACE INTO Users (id, active) VALUES (@user, @chara);";

			SQLiteCommand command = new SQLiteCommand(_DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			command.Parameters.Add(new SQLiteParameter("@chara", chara));

			SQLiteTransaction trans = _DBConnection.BeginTransaction();

			command.CommandText = checkChar;
			SQLiteDataReader check = command.ExecuteReader();
			if (!check.HasRows) {
				check.Close();
				command.CommandText = insertChar;
				command.ExecuteNonQuery();

				command.CommandText = insertActive;
				command.ExecuteNonQuery();

				success = true;				
			}
			trans.Commit();
			return success;			
		}
		
		public bool deleteChar (ulong user, string chara) {
			bool success = false;
			string deleteChar = "DELETE FROM Chars WHERE id = @user AND char = @chara;";
			string deleteSkills = "DELETE FROM Skills WHERE id = @user AND char = @chara;";
			string switchChar = "SELECT char FROM Chars WHERE id = @user LIMIT 1";
			string updateActive = "UPDATE Users SET active = @switchChara WHERE id = @user;";
			string deleteActive = "DELETE FROM Users WHERE id = @user;";

			SQLiteCommand command = new SQLiteCommand(_DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			command.Parameters.Add(new SQLiteParameter("@chara", chara));

			SQLiteTransaction trans = _DBConnection.BeginTransaction();
			command.CommandText = deleteChar;
			int deletions = command.ExecuteNonQuery();
			if (deletions == 1) {
				command.CommandText = deleteSkills;
				command.ExecuteNonQuery();

				command.CommandText = switchChar;
				SQLiteDataReader check = command.ExecuteReader();
				if (check.Read()) {
					string switchChara = check.GetString(0);
					check.Close();
					command.Parameters.Add(new SQLiteParameter("@switchChara", switchChara));
					command.CommandText = updateActive;
					command.ExecuteNonQuery();
				}
				else {
					check.Close();
					command.CommandText = deleteActive;
					command.ExecuteNonQuery();
				}			

				success = true;
				trans.Commit();
			}
			else {
				trans.Rollback();
			}
			return success;
		}
		
		public bool setActiveChar (ulong user, string chara) {
			bool success = false;
			string getChar = "SELECT char FROM Chars WHERE id = @user AND char = @chara;";
			string updateActive = "REPLACE INTO Users (id, active) VALUES (@user, @switchar)";

			SQLiteCommand command = new SQLiteCommand(getChar, _DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			command.Parameters.Add(new SQLiteParameter("@chara", chara));
			SQLiteDataReader result = command.ExecuteReader();
			if (result.Read()) {
				string switchar = result.GetString(0);
				result.Close();
				command.CommandText = updateActive;
				command.Parameters.Add(new SQLiteParameter("@switchar", switchar));
				command.ExecuteNonQuery();	
				success = true;			
			}
			return success;
		}

		public List<string> getCharList (ulong user) {
			List<string> chars = new List<string>();
			string getChars = "SELECT char FROM Chars WHERE id = @user";
			SQLiteCommand command = new SQLiteCommand(getChars, _DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			SQLiteDataReader result = command.ExecuteReader();
			while (result.Read()) {
				chars.Add(result.GetString(0));
			}
			result.Close();
			return chars;
		}

		public List<string> getCharsLike(ulong user, string chara) {
			List<string> chars = new List<string>();
			string search = "'" + chara.ToUpper() + "%'";
			string switchChar = "SELECT char FROM Chars WHERE id = @user AND UPPER(char) LIKE @search";

			SQLiteCommand command = new SQLiteCommand(switchChar, _DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			command.Parameters.Add(new SQLiteParameter("@search", search));
			
			SQLiteDataReader result = command.ExecuteReader();
			while (result.Read()) {
				chars.Add(result.GetString(0));
			}
			return chars;
		}

		public int getCharXP (ulong user, string chara) {
			int res = -1;
			string getXP = "SELECT xp FROM Chars WHERE id = @user AND char = @chara";
			SQLiteCommand command = new SQLiteCommand(getXP, _DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			command.Parameters.Add(new SQLiteParameter("@chara", chara));

			SQLiteDataReader result = command.ExecuteReader();
			if (result.Read()) {
				res = result.GetInt32(0);
			}

			return res;
		}

		public int modifyCharXP (ulong user, string chara, int mod) {
			int xp = -1;
			string getXP = "SELECT xp FROM Chars WHERE id = @user AND char = @chara;";
			string setXP = "UPDATE Chars SET xp = @newxp WHERE id = @user AND char = @chara;";

			SQLiteCommand command = new SQLiteCommand(getXP, _DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			command.Parameters.Add(new SQLiteParameter("@chara", chara));

			SQLiteDataReader result = command.ExecuteReader();
			if (result.Read()) {
				xp = result.GetInt32(0);
				xp += mod;
				xp = Math.Max(0, xp);

				result.Close();
				command.CommandText = setXP;
				command.Parameters.Add(new SQLiteParameter("@newxp", xp));
				command.ExecuteNonQuery();
			}

			return xp;
		}

		public List<Pair<string,int>> getSkillList (ulong user, string chara) {
			List<Pair<string, int>> list = new List<Pair<string, int>>();
			string getSkills = "SELECT skill, lvl FROM Skills WHERE id = @user AND char = @chara";

			SQLiteCommand command = new SQLiteCommand(getSkills, _DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			command.Parameters.Add(new SQLiteParameter("@chara", chara));
			SQLiteDataReader result = command.ExecuteReader();

			while (result.Read()) {
				Pair<string, int> s = new Pair<string, int>(result.GetString(0), result.GetInt32(1));
				list.Add(s);
			}
			result.Close();
			return list;
		}

		public List<Pair<string, int>> getSkillsLike(ulong user, string chara, string skill) {
			List<Pair<string, int>> skills = new List<Pair<string, int>>();
			string search = skill.ToUpper() + "%";
			string getSkills = "SELECT skill, lvl FROM Skills WHERE id = @user AND char = @chara AND UPPER(skill) LIKE @search";

			SQLiteCommand command = new SQLiteCommand(getSkills, _DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			command.Parameters.Add(new SQLiteParameter("@chara", chara));
			command.Parameters.Add(new SQLiteParameter("@search", search));
			SQLiteDataReader result = command.ExecuteReader();
			while (result.Read()) {
				Pair<string, int> s = new Pair<string, int>(result.GetString(0), result.GetInt32(1));
				skills.Add(s);
			}
			return skills;
		}

		public bool addSkill (ulong user, string chara, string skill, int lvl) {
			bool res = false;

			string checkSkill = "SELECT * FROM Skills WHERE id = @user AND char = @chara AND skill = @skill;";
			string insertSkill = "INSERT INTO Skills (id, char, skill, lvl) VALUES (@user, @chara, @skill, @lvl);";

			SQLiteCommand command = new SQLiteCommand(checkSkill, _DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			command.Parameters.Add(new SQLiteParameter("@chara", chara));
			command.Parameters.Add(new SQLiteParameter("@skill", skill));
			command.Parameters.Add(new SQLiteParameter("@lvl", lvl));

			SQLiteDataReader result = command.ExecuteReader();
			if (!result.HasRows) {
				result.Close();
				command.CommandText = insertSkill;
				command.ExecuteNonQuery();
				res = true;
			}
			return res;
		}

		public bool removeSkill(ulong user, string chara, string skill) {
			string removeSkill = "DELETE FROM Skills WHERE id = @user AND char = @chara AND skill = @skill;";

			SQLiteCommand command = new SQLiteCommand(removeSkill, _DBConnection);
			command.Parameters.Add(new SQLiteParameter("@user", user));
			command.Parameters.Add(new SQLiteParameter("@chara", chara));
			command.Parameters.Add(new SQLiteParameter("@skill", skill));
			
			return command.ExecuteNonQuery() == 1;
		}

		public void close () {
			Console.WriteLine("Saving...");
			_DBConnection.Close();
			Console.WriteLine("Saved!");
		}

	}
}

