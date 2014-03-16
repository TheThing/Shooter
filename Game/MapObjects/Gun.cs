using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NetworkLibrary.Utilities;

namespace Shooter
{
	abstract class Gun : IDrawable, IUpdateable, IInteractable, ITileSpecial, INotifyPropertyChanged
	{
		protected NetworkObservableCollection<IShot> _shots;
		protected int _bullets;
		protected IUnit _owner;
		private int _lastX;
		protected int _lastY;

		public Gun()
		{
			_shots = new NetworkObservableCollection<IShot>();
			_lastX = -1;
			_lastY = -1;
		}

		public abstract void Shoot();
		protected abstract string GetBulletIcon(Direction direction);
		public abstract void Draw(int x, int y);
		public abstract Gun Clone();
		public abstract string Name { get; }
		public abstract string Icon { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RunPropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		protected void Shoot(IShot shot)
		{
			if (_bullets > 0)
			{
				Ammo--;
				IUnit unit = null;
				if (shot.Step(out unit))
				{
					if (unit != null)
						HitPlayer(shot, unit);
					return;
				}
				if (Game.Network != null)
					Game.Network.NetworkDataHandler.Register(shot);
				_shots.Add(shot);
			}
		}

		public void Update()
		{
			if (_owner is UnitPlayer)
			{
				if (Game.KeyIsDown((_owner as UnitPlayer).Player.Controls.Action))
					Shoot();
			}

			for (int i = 0; i < _shots.Count; i++)
				_shots[i].Update();
		}

		protected void DoDraw(int x, int y)
		{
			if (_owner != null)
				Draw(x, y, GetBulletIcon(_owner.Direction));
			else
				Draw(x, y, this.Icon);
		}

		protected void SetValues(Gun g)
		{
			g._bullets = this._bullets;
			g._lastX = this._lastX;
			g._lastY = this._lastY;
			g.Cost = this.Cost;
			g.Image = this.Image;
		}

		public void Interact(UnitPlayer player)
		{
			this.Clear();

			Console.SetCursorPosition(_lastX, _lastY);
			Console.Write(" ");
			_lastX = _lastY = -1;

			if (player.Guns.Count > 0)
				for (int i = 0; i < player.Guns.Count; i++)
					if (player.Guns[i].Name == this.Name)
					{
						player.Guns[i].Ammo += this.Ammo;
						player.UpdateHeader();
						return;
					}
			player.Guns.Add(this);
			this.Owner = player;
			player.UpdateHeader();
		}

		protected void Draw(int x, int y, string icon)
		{
			if (_lastX != x || _lastY != y)
			{
				if (_owner != null)
					if (_owner.SelectedGun < _owner.Guns.Count)
						if (_owner.Guns[_owner.SelectedGun] != this)
						{
							DrawShots();
							return;
						}

				if (_lastX > -1 && _lastY > -1)
				{
					if (Game.MainGame.Board[_lastX, _lastY] == -1)
					{
						Console.SetCursorPosition(_lastX, _lastY);
						Console.Write(" ");
					}
				}

				if (x >= 0 && y >= 0)
					if (Game.MainGame.Board[x, y] == -1 || this.Index > 0)
					{
						if (_owner != null)
							Console.ForegroundColor = _owner.UnitColor;
						else
							Console.ForegroundColor = ConsoleColor.Green;
						Console.SetCursorPosition(x, y);
						if (_owner != null)
						{
							if (_owner.Health > 0)
								Console.Write(icon);
							else
								Console.Write(" ");
						}
						else
							Console.Write(icon);
						Console.ForegroundColor = ConsoleColor.White;
					}
				
				_lastX = x;
				_lastY = y;
			}
			DrawShots();
		}

		protected void DrawShots()
		{
			for (int i = 0; i < _shots.Count; i++)
			{
				if (Game.Network != null)
					if (Game.Network.NetworkType == NetworkLibrary.Core.NetworkType.Client)
					{
						_shots[i].Draw(_shots[i].X, _shots[i].Y);
						continue;
					}
				IUnit unit;
				if (_shots[i].Step(out unit))
				{
					if (unit != null)
						HitPlayer(_shots[i], unit);
					_shots[i].Clear();
					_shots.RemoveAt(i);
					i--;
				}
				else
					_shots[i].Draw(_shots[i].X, _shots[i].Y);
			}
		}

		protected void Clear()
		{
			Console.SetCursorPosition(this.Index % Game.MainGame.Board.MaxWidth, this.Index / Game.MainGame.Board.MaxWidth);
			Console.Write(" ");

			Game.MainGame.Board.Tiles[this.Index] = -1;
			if (Game.MainGame.Board.SpecialTiles.IndexOf(this) > 0)
				Game.MainGame.Board.SpecialTiles[Game.MainGame.Board.SpecialTiles.IndexOf(this)] = null;
			this.Index = -1;
		}

		protected void HitPlayer(IShot s, IUnit unit)
		{
			unit.Health -= s.Damage;
			if (unit is UnitPlayer)
				(unit as UnitPlayer).UpdateHeader();
		}

		public int Ammo
		{
			get { return _bullets; }
			set
			{
				_bullets = value;
				RunPropertyChanged("Ammo");
			}
		}

		public IUnit Owner
		{
			get { return _owner; }
			set { _owner = value; }
		}

		public NetworkObservableCollection<IShot> Shots
		{
			get { return _shots; }
		}

		public int LastX
		{
			get { return _lastX; }
			set { _lastY = value; }
		}

		public int LastY
		{
			get { return _lastY; }
			set { _lastY = value; }
		}

		public int Index { get; set; }

		public string[] Image { get; set; }

		public int Cost { get; set; }

		/// <summary>
		/// Get or set the id of the object.
		/// </summary>
		public string NetworkId { get; set; }
	}
}
