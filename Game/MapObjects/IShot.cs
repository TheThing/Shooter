using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NetworkLibrary.Core;

namespace Shooter
{
	interface IShot : INotifyPropertyChanged, IDrawable, IUpdateable, INetworkData
	{
		int X { get; set; }
		int Y { get; set; }
		Direction Direction { get; set; }
		Gun Gun { get; set; }
		ConsoleColor UnitColor { get; }
		bool Step(out IUnit unit);
		void Clear();
		int Damage { get; }
	}
}
