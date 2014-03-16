using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class OptionValue : IOption
	{
		public OptionValue()
		{
		}

		public OptionValue(string name)
		{
			Name = name;
		}

		public OptionValue(string name, string content)
			: this(name)
		{
			Content = content;
		}

		public OptionValue(string name, string content, int value)
			: this(name, content)
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

			Console.Write(this.Name + ": ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(this.Content);
			Console.ForegroundColor = ConsoleColor.White;

			Menu.DrawLine(" ", this.Menu.MaxLength - this.Name.Length - 2 - this.Content.Length);

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
		public string Content { get; set; }
	}
}
