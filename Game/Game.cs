using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using NetworkLibrary.Core;

namespace Shooter
{
	class Game : INetworkData, INotifyPropertyChanged
	{
		public static Game MainGame;

		int _defaultHealth;
		static INetwork _network;
		Player[] _players;
		Map _map;
		string _mapDescription;
		int _mapIndex;
		Board _board;

		public Game()
		{
			this._players = new Player[4];
			this._mapIndex = -1;
			this._defaultHealth = 100;
			MainGame = this;
			this.Won = this.InCutSchene = this.CustomGame = false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public bool LoadStoryMode()
		{
			this.CustomGame = false;
			this._mapIndex++;
			if (File.Exists(string.Format("maps\\{0}.stage", _mapIndex)))
				this._map = new Map(string.Format("maps\\{0}.stage", _mapIndex));
			else
				return true;
			return false;
		}

		public void InitialiseGame()
		{
			if (this._board == null)
			{
				this._board = new Board(this, _map);
				this._board.Load();
			}
			for (int i = 0; i < _players.Length; i++)
			{
				if (_players[i] != null)
					_players[i].Unit.Health = _defaultHealth;
			}
		}

		[DllImport("User32.dll")]
		public static extern short GetKeyState(System.Int32 vKey);

		public static bool KeyIsDown(int key)
		{
			return GetKeyState(key) < 0;
		}

		protected void ThrowPropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		public Player[] Players
		{
			get { return _players; }
		}

		public Map Map
		{
			get { return _map; }
			set 
			{
				_map = value;
				ThrowPropertyChanged("Map");
			}
		}

		public string MapDescription
		{
			get { return _mapDescription; }
			set
			{
				_mapDescription = value;
				ThrowPropertyChanged("MapDescription");
			}
		}

		public int DefaultHealth
		{
			get { return _defaultHealth; }
			set
			{
				_defaultHealth = value;
				ThrowPropertyChanged("DefaultHealth");
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public static INetwork Network
		{
			get { return _network; }
			set { _network = value; }
		}

		public bool InStoryMode
		{
			get { return _mapIndex >= 0; }
		}

		public Board Board
		{
			get { return _board; }
			set { _board = value; }
		}

		public int StoryLevel
		{
			get { return _mapIndex; }
		}
		public int NumberOfPlayers
		{
			get
			{
				int num = 0;
				for (int i = 0; i < _players.Length; i++)
					if (_players[i] != null)
						num++;
				return num;
			}
		}
		public Player CurrentPlayer { get; set; }
		public bool CustomGame { get; set; }
		public bool InCutSchene { get; set; }
		public bool Won { get; set; }
		/// <summary>
		/// Get or set the id of the object.
		/// </summary>
		public string NetworkId { get; set; }
	}
}
