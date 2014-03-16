using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class Tile
	{
		string _tile;
		object _special;

		public Tile()
		{
		}

		public Tile(string tile, int x, int y)
		{
			this._tile = tile;
			this.X = x;
			this.Y = y;
		}

		public void Clear()
		{
			this.Special = null;
			Console.SetCursorPosition(X, Y);
			Console.Write(" ");
		}

		public string TileString
		{
			get { return _tile; }
			set { _tile = value; }
		}

		public object Special
		{
			get { return _special; }
			set { _special = value; }
		}

		public int X { get; set; }
		public int Y { get; set; }
	}
}
