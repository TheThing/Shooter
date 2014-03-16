using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class OptionNumeric : IOption
	{
		public OptionNumeric()
			: this("")
		{
		}

		public OptionNumeric(string name)
			: this(name, 0, 0, 100)
		{
		}

		public OptionNumeric(string name, int value, int min, int max)
			: this(name, value, min, max, 1)
		{
		}

		public OptionNumeric(string name, int value, int min, int max, int step)
		{
			this.Name = name;
			this.Value = value;
			this.MinValue = min;
			this.MaxValue = max;
			this.Step = step;
		}

		public bool Update(ref ConsoleKeyInfo key)
		{
			if (key.Key == ConsoleKey.RightArrow && this.Value < this.MaxValue)
			{
				this.Value += Step;
				if (this.Value > this.MaxValue)
					this.Value = this.MaxValue;
				Draw();
			}
			else if (key.Key == ConsoleKey.LeftArrow && this.Value > this.MinValue)
			{
				this.Value -= Step;
				if (this.Value < this.MinValue)
					this.Value = this.MinValue;
				Draw();
			}
			if (key.Key == ConsoleKey.Enter)
				return true;
			return false;
		}

		public void Draw()
		{
			int i = this.Menu.Options.IndexOf(this);
			Console.SetCursorPosition(this.Menu.X, this.Menu.Y + 1 + i);

			Console.Write("║ ");

			Console.ForegroundColor = ConsoleColor.Green;
			if (i == this.Menu.CurrentOption)
				Console.Write("<");
			else
				Console.Write(" ");

			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(" " + this.Name + ": ");
			//Console.ForegroundColor = ConsoleColor.Blue;
			string value = this.Value.ToString();
			Console.Write(value);
			Menu.DrawLine(" ", this.Menu.MaxLength - this.Name.Length - 4 - value.Length - 1);
			Console.ForegroundColor = ConsoleColor.Green;
			if (i == this.Menu.CurrentOption)
				Console.Write(">");
			else
				Console.Write(" ");
			Console.ForegroundColor = ConsoleColor.White;

			Console.Write(" ║");
		}

		public bool IsEmpty()
		{
			return false;
		}

		public int MaxLengthNeeded
		{
			get
			{
				return this.Name.Length + 6 + this.MaxValue.ToString().Length;
			}
		}

		public int Step { get; set; }
		public int MinValue { get; set; }
		public int MaxValue { get; set; }
		public Menu Menu { get; set; }
		public string Name { get; set; }
		public int Value { get; set; }
	}
}
