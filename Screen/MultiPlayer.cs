using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Shooter
{
	class MultiPlayer : IScreen
	{
		static List<FileInfo> _maps;

		public MultiPlayer()
		{
			_maps = new List<FileInfo>();
			Console.Clear();
		}

		public IScreen Start()
		{
			Menu m = new Menu("Multiplayer", "Hot Seat", "Direct IP", "Internet", "-", "Back to Main Menu");
			int selected;
			IScreen next;
			do
			{
				switch (selected = m.Start(5, 8))
				{
					case 0:
						if ((next = HotSeat()) != null)
						{
							m.Clear();
							return next;
						}
						break;
					case 1:
						if ((next = DirectIP()) != null)
						{
							m.Clear();
							return next;
						}
						break;
				}

			} while (selected != 4);

			return new Main();
		}

		private IScreen HotSeat()
		{
			OptionNumeric num = new OptionNumeric("Number of Players", 2, 2, 4);
			Menu m = new Menu("", num, new Option("-"), new Option("Create Game", 0), new Option("Back", 1));
			switch (m.Start(35, 7))
			{
				case 0:
					m.Clear();
					return SelectMap(num.Value);
				default:
					m.Clear();
					return null;
			}
		}

		private IScreen DirectIP()
		{
			Menu m = new Menu("", "Host a game", "Join a game", "-", "Back");
			int selected = m.Start(35, 7);
			if (selected == 3)
			{
				m.Clear();
				return null;
			}
			string name = CommonOptions.AskInput("Name", 12, 35, 7);
			if (string.IsNullOrEmpty(name))
				return DirectIP();
			if (selected == 0)
				return InitialiseNetGame(name, null);
			else
			{
				string ip = CommonOptions.AskInput("Ip", 15, 35, 7);
				if (string.IsNullOrEmpty(ip))
					return DirectIP();
				return InitialiseNetGame(name, ip);
			}
		}

		private IScreen InitialiseNetGame(string name, string ip)
		{
			Menu m = new Menu(Label.CreateLabelArrayFromText("Initialising the network library and\nloading network plugins."));
			m.Start(32, 7, false);
			try
			{
				IScreen s;
				if (string.IsNullOrEmpty(ip))
					s = new NetGame(name);
				else
					s = new NetGame(name, ip);
				m.Clear();
				return s;
			}
			catch (Exception e)
			{
				m = new Menu(Label.CreateLabelArrayFromText("Error while initialising network library:\n   " + e.Message));
				m.Start(32, 7);
				m.Clear();
				return null;
			}
		}

		private IScreen SelectMap(int players)
		{
			Map map = CommonOptions.SelectMap(32, players);
			IScreen next = null;
			if (!string.IsNullOrEmpty(map.Filename))
				next = CommonOptions.StartNewGame(false, players, map);
			else
				return null;
			if (next == null)
				return SelectMap(players);
			else
				return next;
		}
	}
}
