using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NetworkLibrary.Core;
using NetworkLibrary.Utilities;

namespace Shooter
{
	abstract class UnitHelper : IUnit, INetworkData, INotifyPropertyChanged
	{
		protected int _health;
		protected int _x;
		protected int _y;
		protected int _lastX;
		protected int _lastY;
		protected int _delay;
		protected int _selectedGun;
		protected bool _dead;
		protected Direction _direction;
		protected Player _player;
		protected NetworkObservableCollection<Gun> _guns;

		public UnitHelper(Player player, int health)
		{
			_health = health;
			_player = player;
			_lastX = -1;
			_lastY = -1;
			_delay = 0;
			_guns = new NetworkObservableCollection<Gun>();
			_dead = false;
			_selectedGun = 0;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected int GetNextTile()
		{
			switch (this.Direction)
			{
				case Shooter.Direction.Left:
					return Game.MainGame.Board[_lastX - 1, _lastY];
				case Shooter.Direction.Right:
					return Game.MainGame.Board[_lastX + 1, _lastY];
				case Shooter.Direction.Up:
					return Game.MainGame.Board[_lastX, _lastY - 1];
				default:
					return Game.MainGame.Board[_lastX, _lastY + 1];
			}
		}

		protected void MoveForward()
		{
			switch (this.Direction)
			{
				case Shooter.Direction.Left:
					this.X--;
					break;
				case Shooter.Direction.Right:
					this.X++;
					break;
				case Shooter.Direction.Up:
					this.Y--;
					break;
				default:
					this.Y++;
					break;
			}
		}

		public void Clear(int x, int y)
		{
			if (_lastX > -1 && _lastY > -1)
			{
				if (x != _lastX || y != _lastY || (_health <= 0 && !_dead))
				{
					Console.SetCursorPosition(_lastX, _lastY);
					Console.Write(" ");
					if (_health <= 0)
						_dead = true;
				}
			}
		}

		public abstract void Update();

		public void Draw(int x, int y)
		{
			if (x != _lastX || y != _lastY)
			{
				ForceDraw(x, y);
				_lastX = x;
				_lastY = y;
			}
		}

		public void ForceDraw(int x, int y)
		{
			Console.SetCursorPosition(x, y);
			Console.ForegroundColor = this.UnitColor;
			if (_health > 0)
				if (_player != null)
					Console.Write(_player.PlayerIndex + 1);
				else
					Console.Write("e");
			else
				Console.Write(" ");
			Console.ForegroundColor = ConsoleColor.White;
		}

		protected void ThrowPropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		public abstract ConsoleColor UnitColor { get; }
		public Direction Direction
		{
			get { return _direction; }
			set
			{
				if (_direction != value)
				{
					_direction = value;
					ThrowPropertyChanged("Direction");
				}
			}
		}
		public int SelectedGun
		{
			get { return _selectedGun; }
			set
			{
				_selectedGun = value;
				ThrowPropertyChanged("SelectedGun");
			}
		}

		public int Health
		{
			get { return _health; }
			set
			{
				_health = value;
				ThrowPropertyChanged("Health");
			}
		}

		public NetworkObservableCollection<Gun> Guns
		{
			get { return _guns; }
		}

		public int X
		{
			get { return _x; }
			set
			{
				if (_x != value)
				{
					_x = value;
					ThrowPropertyChanged("X");
				}
			}
		}

		public int Y
		{
			get { return _y; }
			set
			{
				if (_y != value)
				{
					_y = value;
					ThrowPropertyChanged("Y");
				}
			}
		}

		public Player Player
		{
			get { return _player; }
			set
			{
				_player = value;
				ThrowPropertyChanged("Player");
			}
		}

		/// <summary>
		/// Get or set the id of the object.
		/// </summary>
		public string NetworkId { get; set; }
	}
}
