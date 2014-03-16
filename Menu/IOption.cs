using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	interface IOption
	{
		void Draw();
		bool Update(ref ConsoleKeyInfo key);
		bool IsEmpty();
		int MaxLengthNeeded { get; }
		int Value { get; }
		Menu Menu { get; set; }
	}
}
