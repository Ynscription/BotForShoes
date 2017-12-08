using Bot_For_Shoes.bot;
using System.Threading.Tasks;

namespace Bot_For_Shoes {

	class Program {

		static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync() {
			BotForShoes bot = new BotForShoes();
			await bot.launch();
		}
	}		
}
