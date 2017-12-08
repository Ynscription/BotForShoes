
namespace Bot_For_Shoes.bot.services {
	public class TextWalls {

		public static string getSystemText () {
			return @"· Say what you do and roll a number of d6s.

· If the sum of your roll is higher than the opposing roll (either another player or the DM), the thing you wanted to happen, happens.

· The number of the d6s you roll is determined by the level of skill you have.

· At start, you have only one skill: Do anything 1.

· If you roll all sixes on your roll, you can get new skill one level higher than the one you used for the action. The skill must be a subset of what happened to you in the action (Say, Athletics 2 if you were climbing a wall, or Teeth of Biting 2 if you were eating a cake).

· For every roll you fail, you get 1 XP.

· XP can be used to change a die into a 6 for advancement purposes but not for success purposes.";
		}

		public static string getHelpText () {
			return @"
**roll** - Rolls nd6 or a skill from your active character.
If you can level up it displays the approppiate info.
	· roll <n>	· roll <ability>

**character** - Displays info of your active character.
Manage your characters: 
	· create	· delete
	· switch	· list
	· info

**skill** - Rolls a skill of your active character.
Manages skills of your active character:
	· add		· remove

**xp** - Manages xp of your active character.
	· mod		· info

**help** - Displays this help message.";
		}


	}
}
