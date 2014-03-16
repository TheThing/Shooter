using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Shooter
{
	class OptionTextbox : IOption
	{
		int _length;
		string _title;
		string _value;
		int _index;

		public OptionTextbox()
		{
			_index = 0;
			_value = "";
		}

		public OptionTextbox(string title, int length)
			: this()
		{
			_title = title;
			_length = length;
		}

		public void Draw()
		{
			Console.CursorVisible = false;

			int i = this.Menu.Options.IndexOf(this);
			Console.SetCursorPosition(this.Menu.X, this.Menu.Y + 1 + i);

			Console.Write("║ ");

			Console.Write(_title + ": ");

			if (i == this.Menu.CurrentOption)
				Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(_value);
			Console.ForegroundColor = ConsoleColor.White;
			Menu.DrawLine(" ", Menu.MaxLength - _title.Length - 2 - _value.Length);
			Console.Write(" ║");

			if (i == this.Menu.CurrentOption)
			{
				Console.SetCursorPosition(this.Menu.X + _title.Length + 4 + _value.Length, this.Menu.Y + 1 + i);
				Console.CursorVisible = true;
			}
		}

		public bool Update(ref ConsoleKeyInfo key)
		{
			switch (key.Key)
			{
				case ConsoleKey.LeftArrow:
					if (_index > 0)
					{
						_index--;
						Console.SetCursorPosition(this.Menu.X + _title.Length + 4 + _value.Length, this.Menu.Y + 1 + this.Menu.CurrentOption);
					}
					break;
				case ConsoleKey.RightArrow:
					if (_index < _value.Length)
					{
						_index++;
						Console.SetCursorPosition(this.Menu.X + _title.Length + 4 + _value.Length, this.Menu.Y + 1 + this.Menu.CurrentOption);
					}
					break;
				case ConsoleKey.Backspace:
					if (_index > 0)
					{
						_value = _value.Remove(_index - 1, 1);
						_index--;
						Draw();
					}
					break;
				case ConsoleKey.Enter:
					key = new ConsoleKeyInfo('\r', ConsoleKey.DownArrow, false, false, false);
					return false;
				default:
					if (_index > _value.Length)
						_index = 0;
					if (!char.IsControl(key.KeyChar) && _value.Length < _length)
					{
						_value = _value.Insert(_index, key.KeyChar.ToString());
						_index++;
						Draw();
					}
					break;
			}
			return false;
		}

		public bool IsEmpty()
		{
			return false;
		}

		public string Text
		{
			get { return _value; }
			set { _value = value; }
		}

		public int MaxLengthNeeded
		{
			get
			{
				return _length + _title.Length + 2;
			}
		}
		public int Value
		{
			get
			{
				return 0;
			}
		}
		public Menu Menu { get; set; }
	}
}
