using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class Computer : IInteractable, IDrawable, ITileSpecial
	{
		int _delay;
		bool _drawn;
		int _status;
		bool _hacking;
		UnitPlayer _player;

		public Computer()
		{
			_delay = 0;
			_status = 0;
			_drawn = false;
			_hacking = false;
			_player = null;
		}

		public void Interact(UnitPlayer player)
		{
			if (_status < 20)
			{
				if (_player == null)
				{
					_player = player;
					_hacking = true;
				}
			}
		}

		public void Draw(int x, int y)
		{
			if (!_drawn)
			{
				if (_status < 20)
					Console.ForegroundColor = ConsoleColor.Green;
				else
					Console.ForegroundColor = ConsoleColor.Blue;
				Console.SetCursorPosition(x, y);
				Console.Write("C");
				Console.ForegroundColor = ConsoleColor.White;
			}
			if (_hacking)
			{
				_delay++;
				if (_delay > 20)
				{
					if ((_player.Y == y && (_player.X == x - 1 || _player.X == x + 1)) ||
						(_player.X == x && (_player.Y == y - 1 || _player.Y == y + 1)))
					{
						_status++;
						if (_status == 20)
						{
							_player.Player.Money += 2000;
							_player.UpdateHeader();
							_drawn = false;
							_hacking = false;
						}
					}
					else
					{
						_player = null;
						_hacking = false;
					}
					UpdateHackingStatus();
					_delay = 0;
				}
			}
		}

		private void UpdateHackingStatus()
		{
			Console.SetCursorPosition(0, Game.MainGame.Board.MaxHeight + 2);
			if (_hacking)
			{
				Console.WriteLine(" ┌─────────────────────┐");
				Console.Write(" │");
				Console.BackgroundColor = ConsoleColor.Blue;
				Menu.DrawLine(" ", _status);
				Console.BackgroundColor = ConsoleColor.Black;
				Menu.DrawLine(" ", 21 - _status);
				Console.WriteLine("│");
				Console.WriteLine(" └─────────────────────┘");
			}
			else
			{
				Console.WriteLine("                        ");
				Console.WriteLine("                        ");
				Console.WriteLine("                        ");
			}
		}

		public int Index { get; set; }

		/// <summary>
		/// Get or set the id of the object.
		/// </summary>
		public string NetworkId { get; set; }
	}
}
