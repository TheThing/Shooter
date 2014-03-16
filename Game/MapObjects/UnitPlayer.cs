using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NetworkLibrary.Utilities;

namespace Shooter
{
	class UnitPlayer : UnitHelper
	{
		private NetworkObservableCollection<object> _inventory;
		private int _headerX;
		private int _headerY;

		public UnitPlayer()
			: this(null)
		{
		}

		public UnitPlayer(Player player)
			: base(player, 100)
		{
			_inventory = new NetworkObservableCollection<object>();
			_headerX = _headerY = -1;
			_guns.CollectionChanged += _guns_CollectionChanged;
		}

		void _guns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				for (int i = 0; i < e.NewItems.Count; i++)
				{
					(e.NewItems[i] as Gun).PropertyChanged += UnitPlayer_PropertyChanged;
				}
		}

		void UnitPlayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			this.UpdateHeader();
		}

		public override void Update()
		{
			if (Game.Network != null)
				if (_player != Game.MainGame.CurrentPlayer)
					return;

			if (_health <= 0)
				return;
			if (_delay < 15)
				_delay++;

			if (Game.KeyIsDown(_player.Controls.Left))
				Direction = Shooter.Direction.Left;
			if (Game.KeyIsDown(_player.Controls.Right))
				Direction = Shooter.Direction.Right;
			if (Game.KeyIsDown(_player.Controls.Up))
				Direction = Shooter.Direction.Up;
			if (Game.KeyIsDown(_player.Controls.Down))
				Direction = Shooter.Direction.Down;

			if (_delay >= 15 && (Game.KeyIsDown(_player.Controls.Left) ||
								Game.KeyIsDown(_player.Controls.Right) ||
								Game.KeyIsDown(_player.Controls.Up) ||
								Game.KeyIsDown(_player.Controls.Down)))
				CheckMoveForward();

			if (Game.KeyIsDown(_player.Controls.Action) && _guns.Count > 0)
			{
				if (SelectedGun >= _guns.Count)
					SelectedGun = 0;
				_guns[SelectedGun].Shoot();
				UpdateHeader();
			}

			if (Game.KeyIsDown(_player.Controls.Interact))
				if (GetNextTile() >= 0)
				{
					if (!(Game.MainGame.Board.SpecialTiles[GetNextTile()] is Exit) && Game.MainGame.Board.SpecialTiles[GetNextTile()] is IInteractable)
						(Game.MainGame.Board.SpecialTiles[GetNextTile()] as IInteractable).Interact(this);
				}
				else if (_guns.Count > 0 && _delay >= 10)
				{
					SelectedGun++;
					if (SelectedGun >= _guns.Count)
						SelectedGun = 0;
					_guns[SelectedGun].LastX = _guns[SelectedGun].LastY = -1;
					UpdateHeader();
					_delay = 0;
				}
		}



		protected void CheckMoveForward()
		{
			_delay = 0;
			int t = GetNextTile();

			if (t >= -1)
			{
				if (t >= 0)
					if (Game.MainGame.Board.SpecialTiles[t] is IInteractable && !(Game.MainGame.Board.SpecialTiles[GetNextTile()] is Door))
					{
						(Game.MainGame.Board.SpecialTiles[t] as IInteractable).Interact(this);
						if (Game.MainGame.Board.SpecialTiles[t] is Exit)
							this.MoveForward();
						t = GetNextTile();
					}
				if (t == -1)
				{
					this.MoveForward();
				}
			}
		}

		public void DrawHeader()
		{
			_headerX = Game.MainGame.Board.MaxWidth + 3;
			if (_headerY < 0)
				for (int i = 0; i < Game.MainGame.Players.Length; i++)
					if (this._player == Game.MainGame.Players[i])
					{
						_headerY = 3 + (i * 5);
						break;
					}
			if (_headerX > -1 && _headerY > -1)
			{
				Console.SetCursorPosition(_headerX, _headerY);
				Console.Write("┌───────────────────────┐");
				Console.SetCursorPosition(_headerX, _headerY + 1);
				Console.Write("│                       │");
				Console.SetCursorPosition(_headerX, _headerY + 2);
				Console.Write("│                       │");
				Console.SetCursorPosition(_headerX, _headerY + 3);
				Console.Write("└───────────────────────┘");

				Console.SetCursorPosition(_headerX + (25 - _player.Name.Length) / 2, _headerY);
				Console.Write(_player.Name);
				UpdateHeader();
			}
		}

		public void UpdateHeader()
		{
			if (_headerX > -1 && _headerY > -1)
			{
				Console.SetCursorPosition(_headerX + 2, _headerY + 1);
				Console.Write(this.Player.Money.ToString() + " $       ");
				Console.SetCursorPosition(_headerX + 17, _headerY + 1);
				if (this.Health < 0)
					this.Health = 0;
				Console.Write(this.Health.ToString().PadLeft(3) + " hp");
				Console.SetCursorPosition(_headerX + 2, _headerY + 2);
				if (_guns.Count > 0)
					Console.Write(string.Format("{0} ({1})", _guns[SelectedGun].Name, _guns[SelectedGun].Ammo).PadRight(21));
				else
					Console.Write("                      ");
				int keys = 0;
				for (int i = 0; i < this.Inventory.Count; i++)
					if (this.Inventory[i] is Key)
						keys++;
				Console.SetCursorPosition(_headerX + 20, _headerY + 2);
				if (keys > 0)
					Console.Write("{0} k", keys);
				else
					Console.Write("   ");
			}
		}

		public override ConsoleColor UnitColor
		{
			get { return ConsoleColor.Cyan; }
		}

		public NetworkObservableCollection<object> Inventory
		{
			get { return _inventory; }
		}
	}
}
