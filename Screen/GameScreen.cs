using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkLibrary.Core;

namespace Shooter
{
	class GameScreen : IScreen
	{
		Game _game;

		public GameScreen(Game game)
		{
			_game = game;
		}

		public IScreen Start()
		{
			if (Game.Network != null)
				Game.Network.RegisterEvent((int)PacketCode.ClearBullet, ClearBullet);

			Console.Clear();

			while (Console.KeyAvailable)
				Console.ReadKey(true);
			_game.InitialiseGame();
			int numPlayers = _game.NumberOfPlayers;
			if (_game.InStoryMode)
			{
				Menu m;
				if (_game.StoryLevel == 0)
				{
					m = new Menu("Tutorial", Label.CreateLabelArrayFromText(
						"\nFirst time player? Here is some info you might\nwant to know:\n\n" +
						"Weapons:\n  p = Pistol (Ammo: 7, Speed: slow, Damage: 15hp)\n" +
						"  m = Machine gun (Ammo: 50, Speed: fast, Damage: 5hp)\n" +
						"Other:\n  D = Door\n  X = Locked door, need a key to open\n  k = Key, need to open locked doors\n\n  e = Enemies, shoot them\n"));
					m.Start(-1, 5);
					m.Clear();
				}
				_game.Board.ParseMapText();
				m = new Menu(Label.CreateLabelArrayFromText("\n" + _game.Board.Map.MapText));
				m.Start(-1, 3);
				m.Clear();
			}
			Console.CursorVisible = false;
			_game.Board.DrawBase();
			int alive = 0;
			do
			{
				if (Game.Network != null && !Game.Network.NetworkConnection.Connected)
					return new Main();
				if (!_game.InCutSchene)
					_game.Board.Update();
				_game.Board.Draw();
				System.Threading.Thread.Sleep(10);
				alive = 0;
				for (int i = 0; i < _game.Players.Length; i++)
					if (_game.Players[i] != null)
						if (_game.Players[i].Unit.Health > 0)
							alive++;
				if (alive == 1 && numPlayers > 1 && !_game.Board.Coop)
					_game.Won = true;
			} while (!Game.KeyIsDown((int)System.Windows.Forms.Keys.Escape) && !_game.Won && alive > 0);
			while (Console.KeyAvailable)
				Console.ReadKey(true);
			if (alive == 0)
			{
				Menu m = new Menu("Game Over", Label.CreateLabelArrayFromText(
						"You lost the game. "));
				m.Start(-1, 5);
				System.Threading.Thread.Sleep(1000);
				m.Clear();
			}
			if (_game.Won)
			{
				_game.Won = false;
				if (_game.InStoryMode)
				{
					if (_game.LoadStoryMode())
					{
						Menu m = new Menu("You won", Label.CreateLabelArrayFromText(
							"Congratulations, you just won the game!"));
						m.Start(-1, 5);
						System.Threading.Thread.Sleep(2000);
						m.Clear();
					}
					else
					{
						_game.Board = null;
						return new Shop(_game);
					}
				}
				else
				{
					if (_game.Board.Coop)
					{
						Menu m = new Menu("You won", Label.CreateLabelArrayFromText(
							"Congratulations, you people won the level!"));
						m.Start(-1, 5);
						System.Threading.Thread.Sleep(2000);
						m.Clear();
					}
					else
					{
						string name = "";
						for (int i = 0; i < _game.Players.Length; i++)
							if (_game.Players[i] != null)
								if (_game.Players[i].Unit.Health > 0)
								{
									name = _game.Players[i].Name;
									break;
								}
						Menu m = new Menu(name + " won", Label.CreateLabelArrayFromText(
							"Congratulations " + name + ", you are the winner!"));
						m.Start(-1, 5);
						System.Threading.Thread.Sleep(2000);
						m.Clear();
					}
				}
			}
			return new Main();
		}

		private void ClearBullet(object sender, NetworkEventArgs args)
		{
			(args.Data as Shot).Clear();
		}
	}
}
