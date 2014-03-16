using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetworkLibrary.Core;

namespace Shooter
{
	class NetGame : IScreen
	{
		string _name;
		bool _host;
		string _ip;

		public NetGame(string name)
		{
			_host = true;
			_name = name;
			if (!(Game.Network is ConnectionHost))
			{
				if (Game.Network != null)
					Game.Network.Dispose();
				Game.Network = new ConnectionHost(33060, 4);
			}
		}

		public NetGame(string name, string ip)
		{
			_host = false;
			_name = name;
			_ip = ip;
			if (!(Game.Network is ConnectionClient))
			{
				if (Game.Network != null)
					Game.Network.Dispose();
				Game.Network = new ConnectionClient();
			}
		}

		public IScreen Start()
		{
			Game.Network.Header.Add(new NetworkLibrary.Utilities.Header("prog", "shooter"));
			Game.Network.OnWarningOccured += new NetworkLibrary.Exceptions.delegateWarning(Network_OnWarningOccured);
			Game.Network.OnNotificationOccured += new NetworkLibrary.Exceptions.delegateNotification(Network_OnNotificationOccured);
			string message;
			if (_host)
				message = "Initialising host and starting\nto listen on port 33060.";
			else
				message = "Initialising client and connecting\nto ip " + _ip + " on port 33060.";
			Menu m = new Menu(Label.CreateLabelArrayFromText(message));
			m.Start(-1, 3, false);
			try
			{
				if (_host)
					(Game.Network as ConnectionHost).StartBroadcasting();
				else
					(Game.Network as ConnectionClient).Connect(_ip, 33060);
				m.Clear();
				return RegisterDefault(_host);
			}
			catch (Exception e)
			{
				string err;
				if (_host)
					err = "Error while starting host on port 33060:\n\n   ";
				else
					err = "Error while connecting to " + _ip + " on port 33060:\n\n   ";
				m = new Menu(Label.CreateLabelArrayFromText(err + e.Message));
				m.Start(-1, 4);
				m.Clear();
				return new MultiPlayer();
			}
		}

		void Network_OnWarningOccured(object source, NetworkLibrary.Exceptions.Warning warning)
		{
			Console.SetCursorPosition(0, 27);
			Console.Write(warning.Message + "               ");
		}

		protected IScreen RegisterDefault(bool host)
		{
			Game.Network.NetworkDataHandler.RegisterTypeFromAssembly(System.Reflection.Assembly.GetCallingAssembly());
			Menu m;
			if (host)
			{
				m = new Menu(Label.CreateLabelArrayFromText("Registering default types and initialising lobby."));
				m.Start(-1, 3, false);
				Game game = new Game();
				Player p = new Player(0);
				p.Name = _name;
				game.Players[0] = p;
				game.CurrentPlayer = p;
				Game.Network.NetworkDataHandler.RegisterRecursive(game);
				m.Clear();
				return new Lobby(game);
			}
			else
			{
				m = new Menu(Label.CreateLabelArrayFromText("Connected to host. retreaving game data and initialising lobby."));
				m.Start(-1, 3, false)
					;
				Game game = Game.Network.NetworkDataHandler.RequestInstance<Game>();
				m.Clear();
				int index = 0;
				for (; index < game.Players.Length; index++)
				{
					if (game.Players[index] == null)
						break;
				}
				
				if (index == 4)
					throw new Exception("Connection was allowed but the game was full");
				Player p = new Player(index);
				p.Controls = new PlayerControls(0);
				p.Name = _name;
				game.Players[index] = p;
				game.CurrentPlayer = p;
				Game.Network.NetworkDataHandler.RegisterRecursive(p);
				Game.Network.SendEvent((int)PacketCode.NewPlayer, p);
				return new Lobby(game);
			}
		}

		void Network_OnNotificationOccured(object source, string message)
		{
			
		}
	}
}
