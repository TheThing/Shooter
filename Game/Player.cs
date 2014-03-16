using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NetworkLibrary.Core;

namespace Shooter
{
	class Player : INetworkData, INotifyPropertyChanged
	{
		string _name;
		int _playerIndex;
		int _money;
		object _connection;
		UnitPlayer _unit;
		PlayerControls _controls;

		public Player()
			: this(-1)
		{
		}

		public Player(int playerIndex)
		{
			_playerIndex = playerIndex;
			_unit = new UnitPlayer(this);
			if (playerIndex != -1)
				_controls = new PlayerControls(_playerIndex);
			else
				_controls = new PlayerControls();
			_money = 1000;
		}

		public Player(int playerIndex, object connection)
			: this(playerIndex)
		{
			_connection = connection;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[System.Xml.Serialization.XmlIgnore]
		public PlayerControls Controls
		{
			get { return _controls; }
			set { _controls = value; }
		}

		[System.Xml.Serialization.XmlIgnore]
		public object Connection
		{
			get { return _connection; }
			set { _connection = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public int PlayerIndex
		{
			get { return _playerIndex; }
			set { _playerIndex = value; }
		}

		public UnitPlayer Unit
		{
			get { return _unit; }
			set { _unit = value; }
		}

		public int Money
		{
			get { return _money; }
			set
			{
				_money = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("Money"));
			}
		}

		/// <summary>
		/// Get or set the id of the object.
		/// </summary>
		public string NetworkId { get; set; }
	}
}
