using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkLibrary.Utilities;

namespace Shooter
{
	public enum Direction { Left = 0, Right = 1, Up = 2, Down = 3 };

	interface IUnit : IDrawable, IUpdateable
	{
		int X { get; set; }
		int Y { get; set; }
		Direction Direction { get; set; }
		NetworkObservableCollection<Gun> Guns { get; }
		int SelectedGun { get; set; }
		ConsoleColor UnitColor { get; }
		int Health { get; set; }
		void Clear(int x, int y);
	}
}
