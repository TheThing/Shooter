using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shooter
{
	class OptionKey : IOption
	{
		public OptionKey()
		{
		}

		public OptionKey(string name)
		{
			this.Name = name;
			this.KeyName = "<none>";
		}

		public OptionKey(string name, int key)
			: this(name)
		{
			this.Key = key;
			this.KeyName = Enum.GetName(typeof(System.Windows.Forms.Keys), this.Key);
		}

		public bool Update(ref ConsoleKeyInfo key)
		{
			if (key.Key == ConsoleKey.Enter)
			{
				this.KeyName = "<press a key>";
				Draw(true);
				this.Key = (int)Console.ReadKey(true).Key;
				this.KeyName = Enum.GetName(typeof(System.Windows.Forms.Keys), this.Key);
				Draw();
				return true;
			}
			return false;
		}

		public void Draw()
		{
			Draw(false);
		}

		public void Draw(bool overriden)
		{
			int i = this.Menu.Options.IndexOf(this);
			Console.SetCursorPosition(this.Menu.X, this.Menu.Y + 1 + i);

			Console.Write("║ ");

			if (!overriden)
				if (i == this.Menu.CurrentOption)
					Console.BackgroundColor = ConsoleColor.Red;
				else
					Console.BackgroundColor = ConsoleColor.Black;
			
			Console.Write(this.Name + ": ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(this.KeyName);
			Console.ForegroundColor = ConsoleColor.White;
			Menu.DrawLine(" ", this.Menu.MaxLength - this.Name.Length - 2 - this.KeyName.Length);

			Console.BackgroundColor = ConsoleColor.Black;
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
				return this.Name.Length + 18;
			}
		}

		public void UpdateKeyName()
		{
			this.KeyName = Enum.GetName(typeof(System.Windows.Forms.Keys), this.Key);
		}

		public string KeyName { get; set; }
		public int Key { get; set; }
		public Menu Menu { get; set; }
		public string Name { get; set; }
		public int Value { get; set; }
	}
}
