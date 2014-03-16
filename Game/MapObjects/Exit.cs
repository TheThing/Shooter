using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class Exit : IDrawable, IInteractable, ITileSpecial
	{
		bool _animation;
		bool _drawn;
		bool _reverse;
		int _step;
		int _delay;
		UnitPlayer _player;

		public Exit()
		{
			_reverse = _animation = _drawn = false;
			_step = _delay = 0;
			_player = null;
		}

		public void Draw(int x, int y)
		{
			if (!_drawn)
			{
				_drawn = true;
				DrawStep("░");
			}
			else if (_animation)
			{
				_delay++;
				if (_delay > 10)
				{
					if (_step % 2 == 0)
						if (_reverse)
							DrawStep(" ");
						else
						{
							Console.BackgroundColor = ConsoleColor.Yellow;
							_player.ForceDraw(x, y);
							Console.BackgroundColor = ConsoleColor.Black;
						}
					else if (_step % 8 == 1)
						DrawStep("░");
					else if (_step % 8 == 3)
						DrawStep("▒");
					else if (_step % 8 == 5)
						DrawStep("▓");
					else if (_step % 8 == 7)
					{
						DrawStep("█");
						_reverse = true;
					}
					if (_step == 0 && _reverse)
					{
						Game.MainGame.InCutSchene = false;
						Game.MainGame.Won = true;
					}
					if (_reverse)
						_step--;
					else
						_step++;
					_delay = 0;
				}
			}
		}

		protected void DrawStep(string draw)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.SetCursorPosition(this.Index % Game.MainGame.Board.MaxWidth, this.Index / Game.MainGame.Board.MaxWidth);
			Console.Write(draw);
			Console.ForegroundColor = ConsoleColor.White;
		}

		public void Interact(UnitPlayer player)
		{
			_player = player;
			Game.MainGame.InCutSchene = _animation = true;
		}

		public int Index { get; set; }

		/// <summary>
		/// Get or set the id of the object.
		/// </summary>
		public string NetworkId { get; set; }
	}
}
