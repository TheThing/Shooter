using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class DualOption : IOption
	{
		bool _buttonRightSelected = false;

		public DualOption()
			: base()
		{
		}

		public DualOption(string first, string second)
			: base()
		{
			FirstButton = first;
			SecondButton = second;
		}

		public void Draw()
		{
			int i = this.Menu.Options.IndexOf(this);
			Console.SetCursorPosition(this.Menu.X, this.Menu.Y + 1 + i);

			Console.Write("║ ");

			if (i == this.Menu.CurrentOption && !_buttonRightSelected && FirstButton != "-")
				Console.BackgroundColor = ConsoleColor.Red;
			else
				Console.BackgroundColor = ConsoleColor.Black;

			Menu.DrawLine(" ", (this.Menu.MaxLength / 2 - FirstButton.Length) / 2);
			if (this.FirstButton != "-")
				Console.Write(this.FirstButton);
			else
				Console.Write(" ");
			Menu.DrawLine(" ", this.Menu.MaxLength / 2 - ((this.Menu.MaxLength / 2 - FirstButton.Length) / 2 + this.FirstButton.Length) + (this.Menu.MaxLength - this.FirstButton.Length) % 2);

			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write(" ");

			if (i == this.Menu.CurrentOption && _buttonRightSelected && SecondButton != "-")
				Console.BackgroundColor = ConsoleColor.Red;
			else
				Console.BackgroundColor = ConsoleColor.Black;

			Menu.DrawLine(" ", (this.Menu.MaxLength / 2 - SecondButton.Length) / 2);
			if (this.SecondButton != "-")
				Console.Write(this.SecondButton);
			else
				Console.Write(" ");
			Menu.DrawLine(" ", this.Menu.MaxLength / 2 - ((this.Menu.MaxLength / 2 - SecondButton.Length) / 2 + this.SecondButton.Length) + (this.Menu.MaxLength - this.SecondButton.Length) % 2);

			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write(" ║");
		}

		public bool Update(ref ConsoleKeyInfo key)
		{
			if (!_buttonRightSelected && FirstButton == "-")
				_buttonRightSelected = true;
			else if (_buttonRightSelected && SecondButton == "-")
				_buttonRightSelected = false;

			if (key.Key == ConsoleKey.RightArrow && !_buttonRightSelected && SecondButton != "-")
			{
				_buttonRightSelected = true;
				Draw();
			}
			else if (key.Key == ConsoleKey.LeftArrow && _buttonRightSelected && FirstButton != "-")
			{
				_buttonRightSelected = false;
				Draw();
			}
			return false;
		}

		public bool IsEmpty()
		{
			if (FirstButton == "-" && SecondButton == "-")
				return true;
			return false;
		}

		public int Value
		{
			get { return FirstValue; }
		}

		public int MaxLengthNeeded
		{
			get
			{
				if (FirstButton.Length > SecondButton.Length)
					return FirstButton.Length * 2 + 3;
				else
					return SecondButton.Length * 2 + 3;
			}
		}
		public string FirstButton {	get; set; }
		public string SecondButton { get; set; }
		public int FirstValue { get; set; }
		public int SecondValue { get; set; }
		public Menu Menu { get; set; }
	}
}
