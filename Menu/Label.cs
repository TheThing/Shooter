using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class Label : IOption
	{
		string _text;

		public Label()
			: this("")
		{
		}

		public Label(string text)
		{
			_text = text;
		}

		public void Draw()
		{
			Console.SetCursorPosition(this.Menu.X, this.Menu.Y + 1 + this.Menu.Options.IndexOf(this));
			Console.Write("║ " + _text);
			Menu.DrawLine(" ", this.Menu.MaxLength - this._text.Length);
			Console.Write(" ║");
		}

		public bool Update(ref ConsoleKeyInfo key)
		{
			return false;
		}

		public bool IsEmpty()
		{
			return true;
		}

		public int MaxLengthNeeded
		{
			get { return _text.Length; }
		}
		public int Value
		{
			get { return 0; }
		}
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}
		public Menu Menu { get; set; }

		public static IOption[] CreateLabelArrayFromText(string text)
		{
			string[] splitted = text.Split('\n');
			List<Label> options = new List<Label>();
			for (int i = 0; i < splitted.Length; i++)
			{
				while (splitted[i].Length > 70)
				{
					string temp = splitted[i].Remove(70);
					if (temp.Contains(' '))
					{
						splitted[i] = splitted[i].Remove(0, temp.LastIndexOf(' '));
						temp = temp.Remove(temp.LastIndexOf(' '));
					}
					else
						splitted[i] = splitted[i].Remove(0, 70);
					options.Add(new Label(temp));
					
				}
				options.Add(new Label(splitted[i]));
			}
			return options.ToArray();
		}
	}
}
