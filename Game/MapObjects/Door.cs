using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class Door : IDrawable, IInteractable, ITileSpecial
	{
		private bool _locked;
		private bool _drawn;

		public Door()
			: this(true)
		{
		}

		public Door(bool locked)
		{
			_locked = locked;
			_drawn = false;
		}

		public void Draw(int x, int y)
		{
			if (!_drawn)
			{
				_drawn = true;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.SetCursorPosition(x, y);
				Console.Write(_locked ? "X" : "D");
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		public void Interact(UnitPlayer player)
		{
			if (_locked)
			{
				for (int i = 0; i < player.Inventory.Count; i++)
					if (player.Inventory[i] is Key)
					{
						player.Inventory.RemoveAt(i);
						player.UpdateHeader();
						this._locked = false;
						this.Interact(player);
						break;
					}
				return;
			}
			int x = this.Index % Game.MainGame.Board.MaxWidth, y = this.Index / Game.MainGame.Board.MaxWidth;
			this.Clear();

			CheckForDoor(x - 1, y, player);
			CheckForDoor(x + 1, y, player);
			CheckForDoor(x, y - 1, player);
			CheckForDoor(x, y + 1, player);
		}

		protected void CheckForDoor(int x, int y, UnitPlayer player)
		{
			if (Game.MainGame.Board[x, y] >= 0)
			{
				if (Game.MainGame.Board.SpecialTiles[Game.MainGame.Board[x, y]] is Door)
				{
					Door d = (Door)Game.MainGame.Board.SpecialTiles[Game.MainGame.Board[x, y]];
					d._locked = this._locked;
					d.Interact(player);
				}
			}
		}

		protected void Clear()
		{
			Console.SetCursorPosition(this.Index % Game.MainGame.Board.MaxWidth, this.Index / Game.MainGame.Board.MaxWidth);
			Console.Write(" ");

			Game.MainGame.Board.Tiles[this.Index] = -1;
			Game.MainGame.Board.SpecialTiles[Game.MainGame.Board.SpecialTiles.IndexOf(this)] = null;
			this.Index = -1;
		}

		public int Index { get; set; }

		public bool Locked
		{
			get { return _locked; }
			set { _locked = value; }
		}

		/// <summary>
		/// Get or set the id of the object.
		/// </summary>
		public string NetworkId { get; set; }
	}
}
