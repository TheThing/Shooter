using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Shooter
{
	class Map
	{
		string _filename;
		string _description;
		string _mapText;

		public Map()
		{
		}

		public Map(string filename)
		{
			_filename = filename;
			_description = new FileInfo(filename).Name.Replace(".map", "");
			using (StreamReader r = new StreamReader(_filename))
			{
				string raw = r.ReadToEnd();
				if (raw.IndexOf('E') >= 0)
					_description += " (co-op)";
				else
					_description += " (vs)";
			}
		}

		public int MaxPlayersAllowed()
		{
			int players = 1;
			using (StreamReader r = new StreamReader(_filename))
			{
				string raw = r.ReadToEnd();
				for (int i = 4; i >= 1; i--)
				{
					if (raw.IndexOf(i.ToString()) != -1)
					{
						players = i;
						break;
					}
				}
			}
			return players;
		}

		public string ReadMap(out int maxWidth, out int maxHeight)
		{
			string map = "";
			int width = 0, height = 0;
			List<string> temp = new List<string>();
			using (StreamReader read = new StreamReader(_filename))
			{
				while (read.Peek() > -1)
				{
					string buffer = read.ReadLine();
					if (buffer == "=" && map == "")
					{
						while ((buffer = read.ReadLine()) != "=")
							_mapText += buffer + "\n";
						buffer = read.ReadLine();
					}
					temp.Add(buffer);
					if (buffer.Length > width)
						width = buffer.Length;
					height++;
				}
			}
			for (int i = 0; i < temp.Count; i++)
				map += temp[i].PadRight(width) + "\n";
			maxWidth = width;
			maxHeight = height;
			return map;
		}

		public string MapText
		{
			get { return _mapText; }
			set { _mapText = value; }
		}

		public string Filename
		{
			get { return _filename; }
			set { _filename = value; }
		}
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}
	}
}
