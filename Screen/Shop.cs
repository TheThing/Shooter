using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkLibrary.Core;

namespace Shooter
{
	class Shop : IScreen
	{
		private Game _game;
		private int _playerIndex;
		private List<Gun> _guns;
		private int[] _playerSelection;

		public Shop(Game game)
			: this(game, 0)
		{
		}

		public Shop(Game game, int playerIndex)
		{
			_guns = new List<Gun>();
			_guns.Add(new Pistol());
			_guns.Add(new MachineGun());
			_game = game;
			_playerSelection = new int[] { -1, -1 };
			if (_playerIndex < 4)
				_playerIndex = playerIndex;
			else
				_playerIndex = 0;

			if (Game.Network != null)
			{
				Game.Network.RegisterEvent((int)PacketCode.PlayerDisconnected, PlayerDisconnected);
				Game.Network.RegisterEvent((int)PacketCode.PlayerSelectionChanged, PlayerSelectionChanged);
				Game.Network.RegisterEvent((int)PacketCode.PlayerShopBuys, PlayerNetworkBuys);
			}
		}

		public IScreen Start()
		{
			Console.Clear();
			Console.CursorVisible = false;
			for (int i = 0; i < 2; i++)
				if (_game.Players[_playerIndex + i] != null)
				{
					DrawBase(1 + 40 * i, 1);
					_playerSelection[i] = 0;
				}

			while (_playerSelection[0] > -1 || _playerSelection[1] > -1)
			{
				if (Game.Network != null)
					if (!Game.Network.NetworkConnection.Connected)
						return new Main();

				ConsoleKeyInfo key = Console.ReadKey(true);

				for (int i = 0; i < 2; i++)
				{
					if (_game.Players[_playerIndex + i] != null && _playerSelection[i] > -1)
						if ((int)key.Key == _game.Players[_playerIndex + i].Controls.Down && _playerSelection[i] < _guns.Count)
						{
							DrawOption(i, _playerSelection[i], false);
							UpdatePlayerSelection(i, _playerSelection[i] + 1);
							DrawOption(i, _playerSelection[i], true);
						}
						else if ((int)key.Key == _game.Players[_playerIndex + i].Controls.Up && _playerSelection[i] > 0)
						{
							DrawOption(i, _playerSelection[i], false);
							UpdatePlayerSelection(i, _playerSelection[i] - 1);
							DrawOption(i, _playerSelection[i], true);
						}
						else if ((int)key.Key == _game.Players[_playerIndex + i].Controls.Action)
							if (_playerSelection[i] == _guns.Count)
								UpdatePlayerSelection(i, -1);
							else if (_game.Players[_playerIndex + i].Money >= _guns[_playerSelection[i]].Cost)
								PlayerBuys(i);
				}
			}
			Console.Clear();
			Console.CursorVisible = true;
			return new GameScreen(_game);
		}

		private void DrawOption(int player, int option, bool active)
		{
			if (option < _guns.Count)
			{
				Console.SetCursorPosition(2 + 40 * player, 4 + option * 6);
				Console.Write(_guns[option].Image[0].PadRight(35));
				Console.SetCursorPosition(2 + 40 * player, 5 + option * 6);
				Console.Write(_guns[option].Image[1].PadRight(35));
				Console.SetCursorPosition(2 + 40 * player, 6 + option * 6);
				Console.Write(_guns[option].Image[2].PadRight(35));
				Console.SetCursorPosition(2 + 40 * player, 7 + option * 6);
				Console.Write(_guns[option].Image[3].PadRight(35));
				Console.SetCursorPosition(2 + 40 * player, 8 + option * 6);
				Console.Write(_guns[option].Image[4].PadRight(35));

				Console.SetCursorPosition(24 + (11 - _guns[option].Name.Length) / 2 + 1 + 40 * player, 4 + option * 6);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write(_guns[option].Name);
				Console.ForegroundColor = ConsoleColor.White;

				Console.SetCursorPosition(24 + 40 * player, 5 + option * 6);
				Console.Write("Ammo: ");
				Console.Write(_guns[option].Ammo.ToString().PadLeft(4) + " b");
				Console.SetCursorPosition(24 + 40 * player, 6 + option * 6);
				Console.Write("Cost: ");
				Console.Write(_guns[option].Cost.ToString().PadLeft(5) + "$");
				for (int i = 0; i < _game.Players[_playerIndex + player].Unit.Guns.Count; i++)
					if (_game.Players[_playerIndex + player].Unit.Guns[i].Name == _guns[option].Name)
					{
						Console.SetCursorPosition(25 + 40 * player, 7 + option * 6);
						Console.Write("Own: ");
						Console.Write(_game.Players[_playerIndex + player].Unit.Guns[i].Ammo.ToString().PadLeft(4) + " b");
					}
				if (active)
					Console.BackgroundColor = ConsoleColor.Red;
				Console.SetCursorPosition(26 + 40 * player, 8 + option * 6);
				Console.Write("   Buy   ");
			}
			else //
			{
				Console.SetCursorPosition(3 + 40 * player, 4 + option * 6);
				if (active)
					Console.BackgroundColor = ConsoleColor.Red;
				Console.Write("             Finished            ");
			}
			Console.BackgroundColor = ConsoleColor.Black;
		}

		private void UpdatePlayer(int index)
		{
			Console.SetCursorPosition(3 + 40 * index, 2);
			Console.Write(_game.Players[_playerIndex + index].Name);
			Console.SetCursorPosition(25 + 40 * index, 2);
			Console.Write(_game.Players[_playerIndex + index].Money.ToString().PadLeft(10) + "$");
		}

		private void DrawBase(int x, int y)
		{
			Console.SetCursorPosition(x, y);
			Console.Write("┌───────────────────────────────────┐");
			Console.SetCursorPosition(x, y + 1);
			Console.Write("│                                   │");
			Console.SetCursorPosition(x, y + 2);
			Console.Write("├───────────────────────────────────┤");
			for (int i = 0; i < _guns.Count; i++)
			{
				for (int a = 0; a < 5; a++)
				{
					Console.SetCursorPosition(x, y + 3 + a + i * 6);
					Console.Write("│                                   │");
					Console.SetCursorPosition(x, y + 3 + a + i * 6);
				}
				DrawOption(x / 30, i, false);
				Console.SetCursorPosition(x, y + 8 + i * 6);
				Console.Write("├───────────────────────────────────┤");
			}
			Console.SetCursorPosition(x, y + 3 + _guns.Count * 6);
			Console.Write("│                                   │");
			Console.SetCursorPosition(x, y + 4 + _guns.Count * 6);
			Console.Write("└───────────────────────────────────┘");
			DrawOption(x / 30, _guns.Count, false);
			UpdatePlayer(x / 40);
			DrawOption(x / 40, 0, true);
		}

		private void PlayerBuys(int i)
		{
			bool found = false;
			for (int a = 0; a < _game.Players[_playerIndex + i].Unit.Guns.Count; a++)
				if (_game.Players[_playerIndex + i].Unit.Guns[i].Name == _guns[_playerSelection[i]].Name)
				{
					found = true;
					_game.Players[_playerIndex + i].Unit.Guns[i].Ammo += _guns[_playerSelection[i]].Ammo;
				}
			if (!found)
			{
				Gun g = _guns[_playerSelection[i]].Clone();
				g.Owner = _game.Players[_playerIndex + i].Unit;
				if (Game.Network != null)
					Game.Network.NetworkDataHandler.RegisterRecursive(g);

				_game.Players[_playerIndex + i].Unit.Guns.Add(g);
			}
			_game.Players[_playerIndex + i].Money -= _guns[_playerSelection[i]].Cost;
			UpdatePlayer(i);
			DrawOption(i, _playerSelection[i], true);

			if (_game.Players[_playerIndex + i] == _game.CurrentPlayer && Game.Network != null)
				Game.Network.SendEvent((int)PacketCode.PlayerShopBuys, i);
		}

		private void UpdatePlayerSelection(int playerIndex, int newValue)
		{
			_playerSelection[playerIndex] = newValue;
			if (Game.Network != null)
			{
				Game.Network.SendEvent((int)PacketCode.PlayerSelectionChanged, new int[] { playerIndex, _playerSelection[playerIndex] });
				if (newValue == -1 && (_playerSelection[0] != -1 || _playerSelection[1] != -1))
				{
					Menu m = new Menu(Label.CreateLabelArrayFromText("Waiting for other player. "));
					m.Start(-1, 5);
				}
			}
		}

		//Network related code

		void PlayerNetworkBuys(object source, NetworkEventArgs args)
		{
			UpdatePlayer((int)args.Data);
			DrawOption((int)args.Data, _playerSelection[(int)args.Data], true);
			args.Forward();
		}

		void PlayerSelectionChanged(object source, NetworkEventArgs args)
		{
			int[] values = args.Data as int[];
			DrawOption(values[0], _playerSelection[values[0]], false);
			_playerSelection[values[0]] = values[1];
			if (_playerSelection[values[0]] != -1)
				DrawOption(values[0], _playerSelection[values[0]], true);
			else
			{
				DrawOption(values[0], _guns.Count, true);
				if (_playerSelection[0] == -1 && _playerSelection[1] == -1)
				{
					Menu m = new Menu(Label.CreateLabelArrayFromText("Press any key to continue."));
					m.Start(-1, 5);
				}
			}
			args.Forward();
		}

		protected void PlayerDisconnected(object source, NetworkEventArgs args)
		{
			Player p = args.Data as Player;
			for (int i = 0; i < _game.Players.Length; i++)
				if (_game.Players[i] == p)
				{
					if ((_playerIndex == 0 && i < 2) || (_playerIndex == 2 && i >= 2))
						Game.Network.SendEvent((int)PacketCode.PlayerSelectionChanged, new int[] { i % 2, -1 }, true);
					break;
				}
		}

		//End network related code
	}
}
