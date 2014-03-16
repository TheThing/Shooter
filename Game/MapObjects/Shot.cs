using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Shooter
{
	abstract class Shot : IShot
	{
		int _x;
		int _y;
		int _lastX;
		int _lastY;
		private Gun _gun;

		public Shot()
		{
			_lastX = -1;
			_lastY = -1;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected abstract string GetBulletIcon { get; }
		public abstract bool Step(out IUnit unit);
		public abstract int Damage { get; set; }

		public void Update()
		{
		}

		public void Draw(int x, int y)
		{
			if (_lastX != x || _lastY != y)
			{
				if (_lastX > -1 && _lastY > -1)
				{
					Console.SetCursorPosition(_lastX, _lastY);
					Console.Write(" ");
				}

				Console.ForegroundColor = UnitColor;
				Console.SetCursorPosition(x, y);
				Console.Write(GetBulletIcon);
				Console.ForegroundColor = ConsoleColor.White;

				_lastX = x;
				_lastY = y;
			}
			else
			{
			}
		}

		public void Clear()
		{
			Console.SetCursorPosition(_lastX, _lastY);
			Console.Write(" ");
		}

		protected bool DoStep(out IUnit unit)
		{
			for (int i = 0; i < Game.MainGame.Board.Units.Count; i++)
				if (Game.MainGame.Board.Units[i].X == _x && Game.MainGame.Board.Units[i].Y == _y && Game.MainGame.Board.Units[i] != _gun.Owner)
				{
					if (Game.MainGame.Board.Units[i] is UnitPlayer && Game.MainGame.Board.Coop && _gun.Owner is UnitPlayer)
						continue;

					unit = Game.MainGame.Board.Units[i];
					return true;

				}
			switch (this.Direction)
			{
				case Shooter.Direction.Left:
					X--;
					break;
				case Shooter.Direction.Right:
					X++;
					break;
				case Shooter.Direction.Up:
					Y--;
					break;
				case Shooter.Direction.Down:
					Y++;
					break;
			}
			unit = null;
			for (int i = 0; i < Game.MainGame.Board.Units.Count; i++)
				if (Game.MainGame.Board.Units[i].X == _x && Game.MainGame.Board.Units[i].Y == _y && Game.MainGame.Board.Units[i] != _gun.Owner)
				{
					if (Game.MainGame.Board.Units[i] is UnitPlayer && Game.MainGame.Board.Coop && _gun.Owner is UnitPlayer)
						continue;

					unit = Game.MainGame.Board.Units[i];
					return true;
				}
			if (Game.MainGame.Board[_x, _y] == -1)
				return false;
			return true;
		}

		protected void ThrowPropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		public Direction Direction { get; set; }

		public ConsoleColor UnitColor
		{
			get { return _gun.Owner.UnitColor; }
		}

		public Gun Gun
		{
			get { return _gun; }
			set { _gun = value; }
		}

		public int X
		{
			get { return _x; }
			set
			{
				_x = value;
				ThrowPropertyChanged("X");
			}
		}

		public int Y
		{
			get { return _y; }
			set
			{
				_y = value;
				ThrowPropertyChanged("Y");
			}
		}

		public string NetworkId
		{
			get;
			set;
		}
	}
}
