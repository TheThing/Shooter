using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Shooter
{
	class SinglePlayer : IScreen
	{
		

		public SinglePlayer()
		{
			Console.Clear();
		}

		public IScreen Start()
		{
			Menu m = new Menu("Single player", "Story Mode", "Custom Game", "-", "Back to Main Menu");
			int selected;
			IScreen next;
			do
			{
				switch (selected = m.Start(5, 8))
				{
					case 0:
						if ((next = CommonOptions.StartNewGame(true, 1, new Map())) != null)
							return next;
						break;
					case 1:
						if ((next = CustomGame()) != null)
							return next;
						break;
				}

			} while (selected != 3);

			return new Main();
		}

		private IScreen CustomGame()
		{
			Map map = CommonOptions.SelectMap(32, 1);
			IScreen next = null;
			if (!string.IsNullOrEmpty(map.Filename))
				next = CommonOptions.StartNewGame(false, 1, map);
			else
				return null;
			if (next == null)
				return CustomGame();
			else
				return next;
		}
	}
}
