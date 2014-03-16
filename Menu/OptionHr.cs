using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class OptionHr : IOption
	{
		public OptionHr()
		{
		}

		public void Draw()
		{
			int i = this.Menu.Options.IndexOf(this);
			Console.SetCursorPosition(this.Menu.X, this.Menu.Y + 1 + i);

			Console.Write("╟─");
			Menu.DrawLine("─", this.Menu.MaxLength);
			Console.Write("─╢");
		}
		public bool Update(ref ConsoleKeyInfo key)
		{
			return false;
		}
		public bool IsEmpty()
		{
			return true;
		}
		public int MaxLengthNeeded { get { return 1; } }
		public int Value { get { return 0; } }
		public Menu Menu { get; set; }
	}
}
