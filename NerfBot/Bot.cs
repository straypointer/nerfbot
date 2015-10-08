using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NerfBot {

	public class BotConfiguration {
		public string Host { get; set; }
		public int Port { get; set; }
		public string Nick { get; set; }
		public string Channel { get; set; }
	}
	
	public class Bot {

		BotConfiguration _config = null;
		Socket _socket = null;

		public Bot(BotConfiguration config) {
			_config = config;
		}

		public void Start() {

			var socket = this.OpenSocket();
			if (socket == null) {
				return;
			}

			this.SendIrcCommandNoWait(string.Format("NICK {0}", _config.Nick));
			this.SendIrcCommandNoWait(string.Format("USER {0} {1} bla :{2}", "ident", _config.Host, "realname"));
			this.SendIrcCommandNoWait(string.Format("JOIN {0}", _config.Channel));

			// do stuff
			string[] commands = null;
			string[] parameters = null;
			string cmd;
			bool run = true;

			while (run) {
				commands = this.ReadIrcResponse();

				foreach (string c in commands) {
					cmd = c.Trim();

					System.Console.WriteLine(cmd);

					parameters = cmd.Split(new char[] { ' ' }, 4);

					if (parameters[0] == "PING") {
						this.SendIrcCommandNoWait(string.Format("PONG {0}", parameters[1]));
					} else if (parameters.Length > 1) {
						run = this.HandleIrcCommand(parameters);
					}


				}

				// do we have tasks to take care of?  nerf war?


				System.Threading.Thread.Sleep(0);
			}

			this.CloseSocket();

		}

		protected bool HandleIrcCommand(string[] parameters) {
			string cmd = parameters[1];

			switch (cmd) {
				case "PRIVMSG":
					if (parameters[2] == _config.Channel) {
						this.RecvChannelMessage(parameters[0], parameters[3]);
					} else if (parameters[2] == _config.Nick) {
						return this.RecvPrivateMessage(parameters[0], parameters[3]);
					}
					break;
			}

			return true;
		}

		protected void RecvChannelMessage(string sender, string msg) {
			msg = msg.Substring(1);

			if (msg.Contains("nerfbot")) {
				this.SendChannelMessage("yes, i am nerfbot.");
			}
		}

		protected bool RecvPrivateMessage(string sender, string msg) {
			msg = msg.Substring(1);

			if (msg == "QUIT") {
				return false;
			}

			return true;
		}

		protected void SendChannelMessage(string msg) {
			string cmd = string.Format("PRIVMSG {0} {1} ", _config.Channel, ":" + msg);
			this.SendIrcCommandNoWait(cmd);
		}

		protected void SendPrivateMessage(string target, string msg) {
			string cmd = string.Format("PRIVMSG {0} {1} ", target, ":" + msg);
			this.SendIrcCommandNoWait(cmd);
		}

		protected Socket OpenSocket() {
			IPHostEntry ipHostInfo = Dns.GetHostEntry(_config.Host);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, _config.Port);

			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try {
				_socket.Connect(remoteEP);

				Console.WriteLine("Socket connected to {0}", _socket.RemoteEndPoint.ToString());

			} catch (Exception ex) {

				Console.WriteLine("EXCEPTION: ", ex.Message);
				return null;
			}

			return _socket;
		}

		protected void CloseSocket() {
			_socket.Shutdown(SocketShutdown.Both);
			_socket.Close();
		}

		protected byte[] GetBytes(string str) {
			return System.Text.Encoding.UTF8.GetBytes(str);
		}

		protected string GetString(byte[] buffer, int size) {
			return System.Text.Encoding.UTF8.GetString(buffer, 0, size);
		}

		protected int SendIrcCommandNoWait(string cmd) {
		//	byte[] buffer = new byte[1024];
			int bytes = _socket.Send(this.GetBytes(cmd + "\r\n"));

			// if debug
			System.Console.WriteLine("SENT: {0}", cmd);

			return bytes;
		}

		protected string[] ReadIrcResponse() {
			byte[] buffer = new byte[1024];
			int size = _socket.Receive(buffer);

			var str = this.GetString(buffer, size);

			return str.Split('\n');
		}
		

	}
}
