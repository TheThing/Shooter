using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	public enum PacketCode
	{
		NewPlayer = 0,
		Chat = 1,
		PlayerDisconnected = 2,
		StartGame = 3,
		PlayerSelectionChanged = 4,
		PlayerShopBuys = 5,
		ClearBullet = 6,
		PlayerInteracts = 7
	}
}
