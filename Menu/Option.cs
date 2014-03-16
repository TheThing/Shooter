using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class Option : IOption
	{
		public Option()
		{
		}

		public Option(string name)
		{
			Name = name;
		}

		public Option(string name, int value)
			: this(name)
		{
			Value = value;
		}

		public bool Update(ref ConsoleKeyInfo key)
		{
			return false;
		}

		public void Draw()
		{
			int i = this.Menu.Options.IndexOf(this);
			Console.SetCursorPosition(this.Menu.X, this.Menu.Y + 1 + i);

			Console.Write("║ ");

			if (i == this.Menu.CurrentOption)
				Console.BackgroundColor = ConsoleColor.Red;
			else
				Console.BackgroundColor = ConsoleColor.Black;

			Menu.DrawLine(" ", (this.Menu.MaxLength - this.Name.Length) / 2 + (this.Menu.MaxLength - this.Name.Length) % 2);
			if (this.Name != "-")
				Console.Write(this.Name);
			else
				Console.Write(" ");
			Menu.DrawLine(" ", (this.Menu.MaxLength - this.Name.Length) / 2);

			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write(" ║");
		}

		public bool IsEmpty()
		{
			if (this.Name == "-")
				return true;
			return false;
		}

		public int MaxLengthNeeded
		{
			get
			{
				return this.Name.Length;
			}
		}

		public Menu Menu { get; set; }
		public string Name { get; set; }
		public int Value { get; set; }
	}
}
