using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Shooter
{
	class XmlManager
	{
		Game _game;

		public XmlManager(Game game)
		{
			_game = game;
		}

		protected void CreateBase(string filename)
		{
			using (XmlTextWriter w = new XmlTextWriter(filename, Encoding.Unicode))
			{
				w.WriteStartElement("Game");
				w.WriteStartElement("Board");
				string map = "";
				for (int y = 0; y < _game.Board.MaxHeight; y++)
				{
					for (int x = 0; x < _game.Board.MaxWidth; x++)
					{
					}
					if (y < _game.Board.MaxHeight - 1)
						map += "\n";
				}
				w.WriteString(map);
			}
		}
	}
}
