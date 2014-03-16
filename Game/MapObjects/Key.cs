using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class Key : IDrawable, IInteractable, ITileSpecial
	{
		bool _drawn;

		public Key()
		{
			_drawn = false;
		}

		public void Draw(int x, int y)
		{
			if (!_drawn)
			{
				_drawn = true;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.SetCursorPosition(x, y);
				Console.Write("k");
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		public void Interact(UnitPlayer player)
		{
			player.Inventory.Add(this);
			player.UpdateHeader();
			Console.SetCursorPosition(this.Index % Game.MainGame.Board.MaxWidth, this.Index / Game.MainGame.Board.MaxWidth);
			Console.Write(" ");

			Game.MainGame.Board.Tiles[this.Index] = -1;
			Game.MainGame.Board.SpecialTiles[Game.MainGame.Board.SpecialTiles.IndexOf(this)] = null;
			this.Index = -1;
		}

		public int Index { get; set; }

		/// <summary>
		/// Get or set the id of the object.
		/// </summary>
		public string NetworkId { get; set; }
	}
}
