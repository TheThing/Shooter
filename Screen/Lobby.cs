using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkLibrary.Core;

namespace Shooter
{
	class Lobby : IScreen
	{
		Game _game;
		Label[] _players;
		Menu _menuPlayers;
		Menu _menuSettings;
		Menu _mainMenu;
		Label[] _chat;
		bool _active;

		public Lobby(Game game)
		{
			_game = game;
			_players = new Label[] { new Label("<empty slot> "), new Label("<empty slot>"), new Label("<empty slot>"), new Label("<empty slot>") };
			_chat = new Label[] { new Label(), new Label(), new Label(), new Label(), new Label() };
			_mainMenu = new Menu("Chat", _chat);
			_menuSettings = new Menu("Game Settings", new OptionNumeric("Health", _game.DefaultHealth, 10, 1000, 10), new OptionValue("  Map", "<none>", 0), new Option("-"), new Option("-", 1));
			_menuPlayers = new Menu("Lobby", _players);
			_menuSettings.MakeTight = _menuPlayers.MakeTight = true;
			_menuSettings.MaxLength = 51;
		}

		public IScreen Start()
		{
			_active = true;
			Game.Network.OnDisconnected += Network_OnDisconnected;
			Game.Network.RegisterEvent((int)PacketCode.NewPlayer, NewPlayer);
			Game.Network.RegisterEvent((int)PacketCode.Chat, NewChat);
			Game.Network.RegisterEvent((int)PacketCode.StartGame, StartGame);
			Game.Network.RegisterEvent((int)CorePacketCode.PropertyUpdated, UpdateGameSettings);
			Game.Network.RegisterEvent((int)PacketCode.PlayerDisconnected, PlayerDisconnected);
			_menuPlayers.Start(60, 2, false);
			_menuSettings.Start(3, 2, false);
			UpdatePlayers();
			OptionTextbox t = new OptionTextbox(_game.CurrentPlayer.Name, 70 - _game.CurrentPlayer.Name.Length - 2);


			_mainMenu.MakeTight = true;
			_mainMenu.Options.Add(new OptionHr());
			_mainMenu.Options.Add(t);
			_mainMenu.Options.Add(new Option("Senda", 0));
			if (Game.Network.NetworkType == NetworkLibrary.Core.NetworkType.Host)
			{
				_mainMenu.Options.Add(new Option("-"));
				_mainMenu.Options.Add(new Option("Configure", 1));
				_mainMenu.Options.Add(new Option("Start Game", 2));
			}
			_mainMenu.Options.Add(new Option("-"));
			_mainMenu.Options.Add(new Option("Exit Lobby", 3));
			do
			{
				int sel = _mainMenu.Start(-1, 10);

				if (!Game.Network.NetworkConnection.Connected)
					return new Main();
				switch (sel)
				{
					case 0:
						Game.Network.SendEvent((int)PacketCode.Chat, _game.CurrentPlayer.Name + ": " +  t.Text, true);
						t.Text = "";
						break;
					case 1:
						ConfigureGame();
						break;
					case 2:
						if (PrepareStartGame())
						{
							while (_game.Board == null)
								System.Threading.Thread.Sleep(100);
						}
						break;
					case 3:
						Game.Network.Disconnect();
						return new Main();
				}
			} while (_game.Board == null);
			if (Game.Network.NetworkType == NetworkType.Client)
			{
				_game.Board = Game.Network.NetworkDataHandler.RequestInstance<Board>();
				if (_game.Players[0].Unit != _game.Board.Units[0])
				{
				}
				if (_game.Board == null)
				{
					Menu m = new Menu(Label.CreateLabelArrayFromText("Invalid response received from host. Retrying in 1 sec."));
					m.Start(-1, 8);
					System.Threading.Thread.Sleep(1000);
					_game.Board = Game.Network.NetworkDataHandler.RequestInstance<Board>();

					if (_game.Board == null)
					{
						m = new Menu(Label.CreateLabelArrayFromText("Invalid response received from host, disconnecting."));
						m.Start(-1, 8);
						Game.Network.Disconnect();
						return new Main();
					}
				}
			}
			_active = false;
			return new Shop(_game);
		}

		bool PrepareStartGame()
		{
			if (_game.Map == null)
			{
				Menu m = new Menu(Label.CreateLabelArrayFromText("You have to choose a map before\nyou can start the game."));
				m.Start(-1, 8);
				return false;
			}
			int players = 0;
			for (int i = 0; i < _game.Players.Length; i++)
				if (_game.Players[i] != null)
					players++;
			int maxAllowed = _game.Map.MaxPlayersAllowed();
			if (players > maxAllowed)
			{
				Menu m = new Menu(Label.CreateLabelArrayFromText(string.Format("This map does not support so many players.\nMaximum allowed numbers of players for this map is {0}.")));
				m.Start(-1, 8);
				return false;
			}
			int width, height;
			Game.Network.SendEvent((int)PacketCode.StartGame, _game.Map.ReadMap(out width, out height), true);
			return true;
		}

		void ConfigureGame()
		{
			(_menuSettings.Options[3] as Option).Name = "Back";
			(_menuSettings.Options[1] as OptionValue).Content = "<none>";
			if (_game.Map != null)
				if (!string.IsNullOrEmpty(_game.Map.Description))
					(_menuSettings.Options[1] as OptionValue).Content = _game.Map.Description;
			
			if (_menuSettings.Start(3, 2) == 0)
			{
				int players = 0;
				for (int i = 0; i < _game.Players.Length; i++)
					if (_game.Players[i] != null)
						players++;
				Map map = CommonOptions.SelectMap(-1, players);
				_mainMenu.ReDraw();
				_menuPlayers.ReDraw();
				_game.DefaultHealth = (_menuSettings.Options[0] as OptionNumeric).Value;
				if (map != null && !string.IsNullOrEmpty(map.Filename))
					_game.Map = map;
				ConfigureGame();
				return;
			}
			else
				_game.DefaultHealth = (_menuSettings.Options[0] as OptionNumeric).Value;
			_menuSettings.ReDraw();
			(_menuSettings.Options[3] as Option).Name = "-";
		}

		void Network_OnDisconnected(object source, object reason)
		{
			if (Game.Network.NetworkType == NetworkType.Client)
			{
				CommonOptions.DisplayConnectionError(reason);
			}
			else
				for (int i = 0; i < _game.Players.Length; i++)
					if (_game.Players[i] != null)
						if (_game.Players[i].Connection == source)
						{
							Game.Network.SendEvent((int)PacketCode.Chat, "-- player " + _game.Players[i].Name + " has disconnected --", true);
							Game.Network.SendEvent((int)PacketCode.PlayerDisconnected, _game.Players[i], true);
							break;
						}
		}

		protected void StartGame(object source, NetworkEventArgs args)
		{
			Game.Network.UnregisterEvent((int)PacketCode.NewPlayer, NewPlayer);
			Game.Network.UnregisterEvent((int)PacketCode.Chat, NewChat);
			Game.Network.UnregisterEvent((int)PacketCode.StartGame, StartGame);
			Game.Network.UnregisterEvent((int)CorePacketCode.PropertyUpdated, UpdateGameSettings);

			string map = args.Data as string;
			string[] splitted = map.Split('\n');

			_game.Board = new Board(_game, _game.Map);
			if (Game.Network.NetworkType == NetworkType.Host)
			{
				_game.Board.Load(map.Replace("\n", ""), splitted[0].Length, splitted.Length);
				Game.Network.NetworkDataHandler.RegisterRecursive(_game.Board);
			}

			_mainMenu.Options.Insert(8, new Option("Start", 10));
			_mainMenu.Options.Insert(8, new Option("-", 10));
			_mainMenu.ReDraw();

			string text = "-- Host has started the game, please click Start --";
			for (int i = _chat.Length - 1; i >= 0; i--)
			{
				string temp = _chat[i].Text;
				_chat[i].Text = text;
				text = temp;
				_chat[i].Draw();
			}
		}

		protected void UpdateGameSettings(object source, NetworkEventArgs args)
		{
			lock (_menuSettings)
			{
				bool changed = false;
				(_menuSettings.Options[1] as OptionValue).Content = "<none>";
				if (_game.Map != null)
					if (!string.IsNullOrEmpty(_game.Map.Description))
						if ((_menuSettings.Options[1] as OptionValue).Content != _game.Map.Description)
						{
							(_menuSettings.Options[1] as OptionValue).Content = _game.Map.Description;
							changed = true;
						}
				if ((_menuSettings.Options[0] as OptionNumeric).Value != _game.DefaultHealth)
				{
					(_menuSettings.Options[0] as OptionNumeric).Value = _game.DefaultHealth;
					changed = true;
				}
				if (changed)
					_menuSettings.ReDraw();
			}
		}

		protected void NewChat(object source, NetworkEventArgs args)
		{
			string text = args.Data as string;
			for (int i = _chat.Length - 1; i >= 0; i--)
			{
				string temp = _chat[i].Text;
				_chat[i].Text = text;
				text = temp;
				_chat[i].Draw();
			}
			args.Forward();
		}

		protected void NewPlayer(object source, NetworkEventArgs args)
		{
			if (args.BasePacket.Header.GetValue<string>("prog") != "shooter" && Game.Network.NetworkType == NetworkType.Host)
			{
				(Game.Network as ConnectionHost).Disconnect(args.SourceConnection);
				return;
			}
			Player p = args.Data as Player;
			p.Connection = args.SourceConnection;
			int index = 0;
			for (; index < _game.Players.Length; index++)
			{
				if (_game.Players[index] == null)
					break;
			}
			if (index == 4)
			{
				if (Game.Network.NetworkType == NetworkType.Host)
					(Game.Network as ConnectionHost).Disconnect(args.SourceConnection);
				return;
			}
			_game.Players[index] = p;

			args.Forward();
			if (Game.Network.NetworkType == NetworkType.Host)
			{
				Game.Network.SendEvent((int)PacketCode.Chat, "-- player " + p.Name + " has connected --", true, args.SourceConnection);
				args.SendReply((int)CorePacketCode.AssignNewHeaderValue, new NetworkLibrary.Utilities.Header("id", index));
			}
			UpdatePlayers();
		}

		protected void PlayerDisconnected(object source, NetworkEventArgs args)
		{
			Player p = args.Data as Player;
			for (int i = 0; i < _game.Players.Length; i++)
				if (_game.Players[i] == p)
				{
					_game.Players[i] = null;
					break;
				}
			if (_active)
				UpdatePlayers();
		}

		protected void UpdatePlayers()
		{
			for (int i = 0; i < _game.Players.Length; i++)
				if (_game.Players[i] != null)
					_players[i].Text = _game.Players[i].Name;
				else
					_players[i].Text = "<empty slot> ";
			_menuPlayers.ReDraw();
		}
	}
}
