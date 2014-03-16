using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Shooter
{
	class CommonOptions
	{
		static List<FileInfo> _maps;

		public static string AskInput(string name, int length, int x, int y)
		{
			OptionTextbox t = new OptionTextbox(name, length);
			Menu m = new Menu("", t, new Option("-"), new Option("Create game", 0), new Option("Back", 1));
			switch (m.Start(x, y))
			{
				case 0:
					m.Clear();
					return t.Text;
				default:
					m.Clear();
					return null;
			}
		}

		public static IScreen StartNewGame(bool story, int players, Map map)
		{
			OptionTextbox[] textboxes = new OptionTextbox[players];
			for (int i = 0; i < players; i++)
				textboxes[i] = new OptionTextbox("Player " + (i + 1), 12);

			Menu m = new Menu(new Option("-"), new Option("Start Game", 0), new Option("Back", 1));
			m.Options.InsertRange(0, textboxes);
			switch (m.Start(35, 7))
			{
				case 0:
					for (int i = 0; i < players; i++)
						if (string.IsNullOrEmpty(textboxes[i].Text))
						{
							m.Clear();
							Menu msub = new Menu(Label.CreateLabelArrayFromText(
	"\nYou have to specify a name in\norder to start a new game."));
							msub.Start(-1, 20);
							msub.Clear();
							return StartNewGame(story, players, map);
						}
					Game g = new Game();
					for (int i = 0; i < players; i++)
					{
						Player p = new Player(i);
						p.Name = textboxes[i].Text;
						g.Players[i] = p;
					}
					for (int i = players; i < 4; i++)
						g.Players[i] = null;

					if (story)
						g.LoadStoryMode();
					else
						g.Map = map;
					g.CustomGame = !story;

					if (Game.Network != null)
					{
						Game.Network.Dispose();
						Game.Network = null;
					}

					return new Shop(g);

			}
			m.Clear();
			return null;
		}
		public static Map SelectMap(int x, int players)
		{
			int offset = 0, option = 0;
			string savePath = "maps";
			if (!Directory.Exists(savePath))
				return null;

			IOption[] options = new IOption[14];
			if (_maps == null)
				_maps = new List<FileInfo>();
			else
				_maps.Clear();
			_maps.AddRange(new DirectoryInfo(savePath).GetFiles("*.map"));
			for (int i = 0; i < _maps.Count; i++)
				using (StreamReader r = new StreamReader(_maps[i].OpenRead()))
				{
					string raw = r.ReadToEnd();
					if (players == 1 && raw.IndexOf('E') < 0)
					{
						_maps.RemoveAt(i);
						i--;
					}
					else if (raw.IndexOf(players.ToString()) < 0)
					{
						_maps.RemoveAt(i);
						i--;
					}
				}
			OptionDual d = new OptionDual();
			d.FirstValue = -2;
			d.SecondValue = -1;
			Menu m;
			options[0] = d;
			options[1] = new Option("-");
			options[12] = new Option("-");
			options[13] = new Option("  Back  ", _maps.Count);
			do
			{
				if (option == -2)
					offset--;
				else if (option == -1)
					offset++;

				if (offset > 0)
					d.FirstButton = "Previous";
				else
					d.FirstButton = "-";
				if (offset < _maps.Count / 10)
					d.SecondButton = "Next";
				else
					d.SecondButton = "-";

				for (int i = 0; i < 10 && (i + offset * 10) < _maps.Count; i++)
				{
					string name = _maps[i + offset * 10].Name.Replace(".map", "");
					if (players > 1)
					{
						using (StreamReader r = new StreamReader(_maps[i + offset * 10].OpenRead()))
						{
							string raw = r.ReadToEnd();
							if (raw.IndexOf('E') >= 0)
								name += " (co-op)";
							else
								name += " (vs)";
						}
					}
					options[i + 2] = new Option(name, i + offset * 10);
				}

				if (offset == _maps.Count / 10)
					for (int i = (_maps.Count % 10); i < 10; i++)
						options[i + 2] = new Option("-", 0);

				m = new Menu(options);
				m.Clear();
			} while ((option = m.Start(x, 3)) < 0);
			m.Clear();
			if (option == _maps.Count)
				return null;
			else
				return new Map(_maps[option].FullName);
		}

		public static void DisplayConnectionError(object reason)
		{
			string errReason = "";
			if (reason is Exception)
				errReason = (reason as Exception).Message;
			else
				errReason = (string)reason;
			Menu m = new Menu("Connection lost", Label.CreateLabelArrayFromText("Connection to host has ended. The error message was:\n   " + errReason));
			m.Start(-1, 5, false);
		}
	}
}
