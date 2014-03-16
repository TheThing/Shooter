using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class MachineGun : Gun
	{
		int _delay;

		public MachineGun()
			: base()
		{
			_bullets = 50;
			_delay = 40;
			this.Cost = 5000;
			this.Image = new string[] {
@"  /-\___________",
@" |  ___________.=",
@" | / {X}( |",
@" |/  {X}--'",
@"     {X}" };
		}

		public MachineGun(int ammo)
			: this()
		{
			_bullets = ammo;
		}

		public override void Shoot()
		{
			if (_delay >= 10)
			{
				_delay = 0;
				base.Shoot(new SimpleShot(this, GetBulletIcon(_owner.Direction), 5, _owner.X, _owner.Y, _owner.Direction));
			}
		}

		public override Gun Clone()
		{
			MachineGun p = new MachineGun();
			SetValues(p);
			return p;
		}

		protected override string GetBulletIcon(Direction direction)
		{
			switch (direction)
			{
				case Direction.Left:
				case Direction.Right:
				case Direction.Up:
				case Direction.Down:
					return ".";
			}
			return this.Icon;
		}

		public override void Draw(int x, int y)
		{
			if (_delay < 10)
				_delay++;
			base.DoDraw(x, y);
		}

		public override string Name
		{
			get { return "Machine Gun"; }
		}

		public override string Icon
		{
			get { return "m"; }
		}
	}
}
