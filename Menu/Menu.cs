using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
    class Menu
	{
		List<IOption> _options;
		string _header;
		int _maxLength;
		int _currentOption;
		int _x;
		int _y;

		public Menu()
		{
			MakeTight = false;
			_options = new List<IOption>();
		}

		public Menu(string header, params string[] options)
			: this()
		{
			_header = header;
			for (int i = 0; i < options.Length; i++)
			{
				_options.Add(new Option(options[i], i));
			}
		}

		public Menu(params IOption[] options)
			: this()
		{
			_options.AddRange(options);
		}

		public Menu(string header, params IOption[] options)
			: this(options)
		{
			_header = header;
		}

		public void Clear()
		{
			int add = 2;
			if (!string.IsNullOrEmpty(_header))
			{
				if (MakeTight)
					_y -= 2;
				else
					_y -= 3;
				add += 4;
			}
			for (int i = 0; i < _options.Count + add; i++)
			{
				Console.SetCursorPosition(_x, _y + i);
				Console.BackgroundColor = ConsoleColor.Black;
				DrawLine(" ", _maxLength + 4);
			}
		}

		public int Start(int x, int y)
		{
			return Start(x, y, true);
		}

		public int Start(int x, int y, bool stop)
		{
			_x = x;
			_y = y;
			_currentOption = -1;

			Console.CursorVisible = false;
			bool optionsAvailable = false;
			for (int i = 0; i < _options.Count; i++)
			{
				if (!_options[i].IsEmpty() && !optionsAvailable)
				{
					optionsAvailable = true;
					if (stop)
						_currentOption = i;
				}
				if (_options[i].MaxLengthNeeded > _maxLength)
					_maxLength = _options[i].MaxLengthNeeded;
			}

			if (!string.IsNullOrEmpty(_header))
				if (_maxLength < _header.Length + 8)
					_maxLength = _header.Length + 8;

			if (_x == -1)
				_x = (80 - _maxLength - 4) / 2;

			//╔═╗║╚╝╡╞  ╞╡┌┐└┘─
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.White;
			if (!string.IsNullOrEmpty(_header))
			{
				int offset = 3;
				if (MakeTight)
					offset = 2;
				Console.SetCursorPosition(_x, _y - offset);
				DrawLine(" ", (_maxLength - _header.Length - 6) / 2 + 2 + (_maxLength - _header.Length - 6) % 2);
				Console.Write("╔");
				DrawLine("═", _header.Length + 4);
				Console.Write("╗");
				DrawLine(" ", (_maxLength - _header.Length - 6) / 2 + 2);

				Console.SetCursorPosition(_x, _y - offset + 1);
				Console.Write("╔");
				DrawLine("═", (_maxLength - _header.Length - 6) / 2 + 1 + (_maxLength - _header.Length - 6) % 2);
				Console.Write("╣  ");
				Console.Write(_header);
				Console.Write("  ╠");
				DrawLine("═", (_maxLength - _header.Length - 6) / 2 + 1);
				Console.Write("╗");
				Console.SetCursorPosition(_x, _y - offset + 2);
				Console.Write("║");
				DrawLine(" ", (_maxLength - _header.Length - 6) / 2 + 1 + (_maxLength - _header.Length - 6) % 2);
				Console.Write("╚");
				DrawLine("═", _header.Length + 4);
				Console.Write("╝");
				DrawLine(" ", (_maxLength - _header.Length - 6) / 2 + 1);
				Console.Write("║");
				if (!MakeTight)
				{
					Console.SetCursorPosition(_x, _y);
					Console.Write("║");
					DrawLine(" ", _maxLength + 2);
					Console.Write("║");
				}
			}
			else
			{
				Console.SetCursorPosition(_x, _y);
				Console.Write("╔");
				DrawLine("═", _maxLength + 2);
				Console.Write("╗");
			}
			for (int i = 0; i < _options.Count; i++)
			{
				_options[i].Menu = this;
				_options[i].Draw();
			}
			Console.SetCursorPosition(_x, _y + _options.Count + 1);
			if (!string.IsNullOrEmpty(_header) && !MakeTight)
			{
				Console.Write("║");
				DrawLine(" ", _maxLength + 2);
				Console.Write("║");
				Console.SetCursorPosition(_x, _y + _options.Count + 2);
			}
			Console.Write("╚");
			DrawLine("═", _maxLength + 2);
			Console.Write("╝");

			if (!stop)
				return 0;
			if (!optionsAvailable)
			{
				Console.ReadKey(true);
				return 0;
			}
			if (_currentOption < 0)
				_currentOption = 0;
			_options[_currentOption].Draw();
			ConsoleKeyInfo key;
			do
			{
				key = Console.ReadKey(true);
				if (_currentOption < 0)
					_currentOption = 0;
				if (!_options[_currentOption].Update(ref key))
				{
					int index = _currentOption;
					do
					{
						if (key.Key == ConsoleKey.DownArrow && _currentOption < _options.Count - 1)
							index++;
						else if (key.Key == ConsoleKey.UpArrow && _currentOption > 0)
							index--;
					} while (_options[index].IsEmpty() && index != _options.Count - 1 && index != 0);
					if ((index == _options.Count - 1 || index == 0) && _options[index].IsEmpty())
						index = _currentOption;
					if (index != _currentOption)
					{
						int temp = _currentOption;
						_currentOption = index;
						_options[temp].Draw();
						_options[_currentOption].Draw();
					}
				}
				else
					key = new ConsoleKeyInfo();
			} while (key.Key != ConsoleKey.Enter);

			Console.CursorVisible = true;
			return _options[_currentOption].Value;
		}

		public void ReDraw()
		{
			this.Start(_x, _y, false);
		}

		public static void DrawLine(string text, int repeat)
		{
			for (int i = 0; i < repeat; i++)
				Console.Write(text);
		}

		public bool MakeTight
		{
			get;
			set;
		}

		public List<IOption> Options
		{
			get { return _options; }
		}
		public int MaxLength
		{
			get { return _maxLength; }
			set { _maxLength = value; }
		}
		public int CurrentOption
		{
			get { return _currentOption; }
		}
		public int X
		{
			get { return _x; }
		}
		public int Y
		{
			get { return _y; }
		}
	}
}
