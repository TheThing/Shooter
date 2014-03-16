using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class Options : IScreen
	{
		public Options()
		{
		}

		public IScreen Start()
		{
			Menu m = new Menu("Options", "Config", "-", "Back to Main Menu");
			int selected;
			do
			{
				switch (selected = m.Start(3, 9))
				{
					case 0:
						Config();
						break;
				}

			} while (selected != 2);
			m.Clear();
			return new Main();
		}

		protected void Config()
		{
			Menu m = new Menu("", "Player 1", "Player 2", "Player 3", "Player 4", "-", "Back");
			int selected;
			do
			{
				selected = m.Start(26, 7);
				if (selected < 5)
					ConfigPlayer(selected);

			} while (selected != 5);
			m.Clear();
		}

		protected void ConfigPlayer(int index)
		{
			OptionKey[] keys = new OptionKey[] { new OptionKey("Up"),
												 new OptionKey("Down"),
												 new OptionKey("Left"),
												 new OptionKey("Right"),
												 new OptionKey("Shoot"),
												 new OptionKey("Change")};
			switch (index)
			{
				case 0:
					keys[0].Key = Properties.Settings.Default.player1_up;
					keys[1].Key = Properties.Settings.Default.player1_down;
					keys[2].Key = Properties.Settings.Default.player1_left;
					keys[3].Key = Properties.Settings.Default.player1_right;
					keys[4].Key = Properties.Settings.Default.player1_action;
					keys[5].Key = Properties.Settings.Default.player1_interact;
					break;
				case 1:
					keys[0].Key = Properties.Settings.Default.player2_up;
					keys[1].Key = Properties.Settings.Default.player2_down;
					keys[2].Key = Properties.Settings.Default.player2_left;
					keys[3].Key = Properties.Settings.Default.player2_right;
					keys[4].Key = Properties.Settings.Default.player2_action;
					keys[5].Key = Properties.Settings.Default.player2_interact;
					break;
				case 2:
					keys[0].Key = Properties.Settings.Default.player3_up;
					keys[1].Key = Properties.Settings.Default.player3_down;
					keys[2].Key = Properties.Settings.Default.player3_left;
					keys[3].Key = Properties.Settings.Default.player3_right;
					keys[4].Key = Properties.Settings.Default.player3_action;
					keys[5].Key = Properties.Settings.Default.player3_interact;
					break;
				default:
					keys[0].Key = Properties.Settings.Default.player4_up;
					keys[1].Key = Properties.Settings.Default.player4_down;
					keys[2].Key = Properties.Settings.Default.player4_left;
					keys[3].Key = Properties.Settings.Default.player4_right;
					keys[4].Key = Properties.Settings.Default.player4_action;
					keys[5].Key = Properties.Settings.Default.player4_interact;
					break;
			}
			for (int i = 0; i < 6; i++)
				keys[i].UpdateKeyName();
			Menu m = new Menu(keys[0], keys[1], keys[2], keys[3], keys[4], keys[5], new Option("-"), new Option("Back", 0));
			m.Start(40, 6);
			switch (index)
			{
				case 0:
					Properties.Settings.Default.player1_up = keys[0].Key;
					Properties.Settings.Default.player1_down = keys[1].Key;
					Properties.Settings.Default.player1_left = keys[2].Key;
					Properties.Settings.Default.player1_right = keys[3].Key;
					Properties.Settings.Default.player1_action = keys[4].Key;
					Properties.Settings.Default.player1_interact = keys[5].Key;
					break;
				case 1:
					Properties.Settings.Default.player2_up = keys[0].Key;
					Properties.Settings.Default.player2_down = keys[1].Key;
					Properties.Settings.Default.player2_left = keys[2].Key;
					Properties.Settings.Default.player2_right = keys[3].Key;
					Properties.Settings.Default.player2_action = keys[4].Key;
					Properties.Settings.Default.player2_interact = keys[5].Key;
					break;
				case 2:
					Properties.Settings.Default.player3_up = keys[0].Key;
					Properties.Settings.Default.player3_down = keys[1].Key;
					Properties.Settings.Default.player3_left = keys[2].Key;
					Properties.Settings.Default.player3_right = keys[3].Key;
					Properties.Settings.Default.player3_action = keys[4].Key;
					Properties.Settings.Default.player3_interact = keys[5].Key;
					break;
				default:
					Properties.Settings.Default.player4_up = keys[0].Key;
					Properties.Settings.Default.player4_down = keys[1].Key;
					Properties.Settings.Default.player4_left = keys[2].Key;
					Properties.Settings.Default.player4_right = keys[3].Key;
					Properties.Settings.Default.player4_action = keys[4].Key;
					Properties.Settings.Default.player4_interact = keys[5].Key;
					break;
			}
			Properties.Settings.Default.Save();
			m.Clear();
		}
	}
}
