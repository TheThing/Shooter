using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class SimpleShot : Shot
	{
		string _icon;
		int _damage;
		int _delay;

		public SimpleShot()
			: base()
		{
		}

		public SimpleShot(Gun gun, string icon, int damage, int x, int y, Direction direction)
			: base()
		{
			_delay = 100;
			this.Gun = gun;
			_icon = icon;
			_damage = damage;
			this.X = x;
			this.Y = y;
			this.Direction = direction;
		}

		public override bool Step(out IUnit unit)
		{
			_delay++;
			unit = null;
			if (_delay < 10)
				return false;
			_delay = 0;
			return base.DoStep(out unit);
		}

		public override int Damage
		{
			get { return _damage; }
			set { _damage = value; }
		}

		public string Icon
		{
			get { return _icon; }
			set { _icon = value; }
		}

		protected override string GetBulletIcon
		{
			get { return _icon; }
		}
	}
}
