using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class Main : IScreen
	{
		public Main()
		{
			Console.Clear();
		}

		public IScreen Start()
		{
			Menu m = new Menu("Main Menu", "Single Player", "Multiplayer", "-", "Options", "-", "Exit");
			int selected = m.Start(-1, 5);
			m.Clear();
			switch (selected)
			{
				case 0:
					return new SinglePlayer();
				case 1:
					return new MultiPlayer();
				case 3:
					return new Options();
				default:
					return null;
			}
		}
	}
}
