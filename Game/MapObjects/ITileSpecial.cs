using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkLibrary.Core;

namespace Shooter
{
	interface ITileSpecial : IDrawable, INetworkData
	{
		 int Index { get; set; }
	}
}
