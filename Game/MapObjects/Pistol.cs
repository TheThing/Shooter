using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class Pistol : Gun
	{
		int _delay;

		public Pistol()
			: base()
		{
			_bullets = 7;
			_delay = 40;
			this.Cost = 1000;
			this.Image = new string[] {
@"  _____________",
@" \-------------'",
@"  /  /( /",
@" /  /---",
@"/__/" };
		}

		public Pistol(int ammo)
			: this()
		{
			_bullets = ammo;
		}

		public override void Shoot()
		{
			if (_delay >= 40)
			{
				_delay = 0;
				base.Shoot(new SimpleShot(this, GetBulletIcon(_owner.Direction), 15, _owner.X, _owner.Y, _owner.Direction));
			}
		}

		public override Gun Clone()
		{
			Pistol p = new Pistol();
			SetValues(p);
			return p;
		}

		protected override string GetBulletIcon(Direction direction)
		{
			switch (direction)
			{
				case Direction.Left:
				case Direction.Right:
					return "-";
				case Direction.Up:
				case Direction.Down:
					return "|";
			}
			return this.Icon;
		}

		public override void Draw(int x, int y)
		{
			if (_delay < 40)
				_delay++;
			base.DoDraw(x, y);
		}

		public override string Name
		{
			get { return "Pistol"; }
		}

		public override string Icon
		{
			get { return "p"; }
		}
	}
}
