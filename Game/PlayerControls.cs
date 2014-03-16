using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class PlayerControls
	{
		int _up;
		int _down;
		int _left;
		int _right;
		int _action;
		int _interact;

		public PlayerControls()
		{
		}

		public PlayerControls(int index)
		{
			switch (index)
			{
				case 0:
					_up = Properties.Settings.Default.player1_up;
					_down = Properties.Settings.Default.player1_down;
					_left = Properties.Settings.Default.player1_left;
					_right = Properties.Settings.Default.player1_right;
					_action = Properties.Settings.Default.player1_action;
					_interact = Properties.Settings.Default.player1_interact;
					break;
				case 1:
					_up = Properties.Settings.Default.player2_up;
					_down = Properties.Settings.Default.player2_down;
					_left = Properties.Settings.Default.player2_left;
					_right = Properties.Settings.Default.player2_right;
					_action = Properties.Settings.Default.player2_action;
					_interact = Properties.Settings.Default.player2_interact;
					break;
				case 2:
					_up = Properties.Settings.Default.player3_up;
					_down = Properties.Settings.Default.player3_down;
					_left = Properties.Settings.Default.player3_left;
					_right = Properties.Settings.Default.player3_right;
					_action = Properties.Settings.Default.player3_action;
					_interact = Properties.Settings.Default.player3_interact;
					break;
				case 3:
					_up = Properties.Settings.Default.player4_up;
					_down = Properties.Settings.Default.player4_down;
					_left = Properties.Settings.Default.player4_left;
					_right = Properties.Settings.Default.player4_right;
					_action = Properties.Settings.Default.player4_action;
					_interact = Properties.Settings.Default.player4_interact;
					break;
			}
		}

		public int Right
		{
			get { return _right; }
			set { _right = value; }
		}

		public int Down
		{
			get { return _down; }
			set { _down = value; }
		}

		public int Left
		{
			get { return _left; }
			set { _left = value; }
		}

		public int Up
		{
			get { return _up; }
			set { _up = value; }
		}

		public int Action
		{
			get { return _action; }
			set { _action = value; }
		}

		public int Interact
		{
			get { return _interact; }
			set { _interact = value; }
		}
	}
}
