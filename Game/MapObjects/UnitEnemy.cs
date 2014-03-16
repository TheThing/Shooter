using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class UnitEnemy : UnitHelper
	{
		private static Random _r; //our AI intelligence
		bool _angry;

		public UnitEnemy()
			: base(null, 30)
		{
			if (_r == null)
				_r = new Random();
			_angry = false;
			this.Direction = (Shooter.Direction)_r.Next(0, 4);
		}

		public override void Update()
		{
			if (Game.Network != null)
				if (Game.Network.NetworkType != NetworkLibrary.Core.NetworkType.Host)
					return;
			if (_lastX == -1 || _lastY == -1)
				return;
			if (_health <= 0)
				return;
			_delay++;

			Shooter.Direction dir;
			if (CheckForPlayer(out dir))
			{
				this.Direction = dir;
				_angry = true;
				for (int i = 0; i < _guns.Count; i++)
					_guns[i].Shoot();
			}

				
			if (_delay >= 25)
			{
				_delay = 0;
				

				if (GetNextTile() == -1)
				{
					_delay = 0;
					this.MoveForward();

					//Something random, becuase nothing says good AI like a random AI
					if (!_angry)
						if (_r.Next(0, 10) == 1)
							this.Direction = (Shooter.Direction)_r.Next(0, 4);
				}
				else
				{
					_angry = false;
					//ooohh, another random... Shiney
					this.Direction = (Shooter.Direction)_r.Next(0, 4);
				}
			}
		}

		bool CheckForPlayer(out Shooter.Direction dir)
		{
			int x = this._lastX; int y = this._lastY;
			dir = Shooter.Direction.Up;
			for (y--; y > 1; y--)
			{
				if (Game.MainGame.Board[x, y] == -1)
				{
					if (IsPlayeronSpot(x, y))
						return true;
				}
				else
					break;
			}
			y = this._lastY;
			dir = Shooter.Direction.Down;
			for (y++; y < Game.MainGame.Board.MaxHeight; y++)
			{
				if (Game.MainGame.Board[x, y] == -1)
				{
					if (IsPlayeronSpot(x, y))
						return true;
				}
				else
					break;
			}
			y = this._lastY;
			dir = Shooter.Direction.Left;
			for (x--; x > 1; x--)
			{
				if (Game.MainGame.Board[x, y] == -1)
				{
					if (IsPlayeronSpot(x, y))
						return true;
				}
				else
					break;
			}
			x = this._lastX;
			dir = Shooter.Direction.Right;
			for (x++; x < Game.MainGame.Board.MaxWidth; x++)
			{
				if (Game.MainGame.Board[x, y] == -1)
				{
					if (IsPlayeronSpot(x, y))
						return true;
				}
				else
					break;
			}
			return false;
		}

		private bool IsPlayeronSpot(int x, int y)
		{
			for (int i = 0; i < Game.MainGame.Players.Length; i++)
				if (Game.MainGame.Players[i] != null)
						if (Game.MainGame.Players[i].Unit.X == x && Game.MainGame.Players[i].Unit.Y == y && Game.MainGame.Players[i].Unit.Health > 0)
							return true;
			return false;
		}

		public override ConsoleColor UnitColor
		{
			get { return ConsoleColor.Red; }
		}
	}
}
