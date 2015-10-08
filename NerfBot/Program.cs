using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerfBot {
	class Program {
		static void Main(string[] args) {

			var config = new BotConfiguration() {
				Host = ConfigurationManager.AppSettings["host"],
				Nick = ConfigurationManager.AppSettings["nick"],
				Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]),
				Channel = ConfigurationManager.AppSettings["channel"]
			};

			var bot = new Bot(config);

			bot.Start();

		}
	}
}
