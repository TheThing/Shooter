using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetworkLibrary.Core;
using NetworkLibrary.Utilities;

namespace Shooter
{
	class Board : IUpdateable, INetworkData
	{
		Map _map;
		string _mapRaw;
		Game _game;
		NetworkObservableCollection<int> _tiles;
		NetworkObservableCollection<ITileSpecial> _specialTiles;
		NetworkObservableCollection<IUnit> _units;
		int _maxHeight;
		int _maxWidth;
		bool _coop;

		public Board()
		{
			_maxHeight = -1;
			_maxWidth = -1;
			_tiles = new NetworkObservableCollection<int>();
			_specialTiles = new NetworkObservableCollection<ITileSpecial>();
			_units = new NetworkObservableCollection<IUnit>();
		}

		public Board(Game game)
			: this()
		{
			_game = game;
		}

		public Board(Game game, Map map)
			: this(game)
		{
			_map = map;
		}

		public int this[int x, int y]
		{
			get
			{
				if (x < _maxWidth && y < _maxHeight)
					return _tiles[x + y * _maxWidth];
				return -2;
			}
		}

		public void Load()
		{
			int width = 0, height = 0;
			string map = _map.ReadMap(out width, out height);
			map = map.Replace("\n", "");
			Load(map, width, height);
		}

		public void Load(string mapRaw, int width, int height)
		{
			_coop = false;
			_maxHeight = height;
			_maxWidth = width;
			_mapRaw = mapRaw;

			for (int i = 0; i < _mapRaw.Length; i++)
			{
				CheckForSpecialTile(_mapRaw[i], i);
			}
		}

		public void ParseMapText()
		{
			_map.MapText = _map.MapText.Replace("%player_name%", _game.Players[0].Name);
		}

		private void CheckForSpecialTile(char c, int index)
		{
			int x = index % _maxWidth;
			int y = index / _maxWidth;
			switch (c)
			{
				case '1':
				case '2':
				case '3':
				case '4':
					if (_game.Players[(int)c - 49] != null)
					{
						_game.Players[(int)c - 49].Unit.X = x;
						_game.Players[(int)c - 49].Unit.Y = y;
						_units.Add(_game.Players[(int)c - 49].Unit);
					}
					_tiles.Add(-1);
					break;
				case 'p':
					_specialTiles.Add(new Pistol());
					_specialTiles[_specialTiles.Count - 1].Index = index;
					_tiles.Add(_specialTiles.Count - 1);
					break;
				case 'D':
				case 'X':
					_specialTiles.Add(new Door(c == 'X'));
					_specialTiles[_specialTiles.Count - 1].Index = index;
					_tiles.Add(_specialTiles.Count - 1);
					break;
				case 'k':
					_specialTiles.Add(new Key());
					_specialTiles[_specialTiles.Count - 1].Index = index;
					_tiles.Add(_specialTiles.Count - 1);
					break;
				case 'E':
					_coop = true;
					_specialTiles.Add(new Exit());
					_specialTiles[_specialTiles.Count - 1].Index = index;
					_tiles.Add(_specialTiles.Count - 1);
					break;
				case 'm':
					_specialTiles.Add(new MachineGun());
					_specialTiles[_specialTiles.Count - 1].Index = index;
					_tiles.Add(_specialTiles.Count - 1);
					break;
				case 'e':
					UnitEnemy unit = new UnitEnemy();
					unit.X = x;
					unit.Y = y;
					//Computer always has infinitive ammo
					unit.Guns.Add(new Pistol(1000000));
					unit.SelectedGun = 0;
					unit.Guns[unit.SelectedGun].Owner = unit;
					_units.Add(unit);
					_tiles.Add(-1);
					break;
				case 'C':
					_specialTiles.Add(new Computer());
					_specialTiles[_specialTiles.Count - 1].Index = index;
					_tiles.Add(_specialTiles.Count - 1);
					break;
				default:
					if (c == ' ')
						_tiles.Add(-1);
					else
						_tiles.Add(-2);
					break;
			}
		}

		public void DrawBase()
		{
			for (int i = 0; i < _tiles.Count; i++)
			{
				if (_tiles[i] > -1)
					if (_specialTiles[_tiles[i]] is IDrawable)
					{
						(_specialTiles[_tiles[i]] as IDrawable).Draw(i % _maxWidth, i / _maxWidth);
						continue;
					}

				Console.SetCursorPosition(i % _maxWidth, i / _maxWidth);
				if (_mapRaw[i] != '1' && _mapRaw[i] != '2' && _mapRaw[i] != '3' && _mapRaw[i] != '4')
					Console.Write(_mapRaw[i]);
				else
					Console.Write(" ");
			}

			for (int i = 0; i < _units.Count; i++)
				if (_units[i] is UnitPlayer)
					(_units[i] as UnitPlayer).DrawHeader();
		}

		public void Draw()
		{
			for (int i = 0; i < _specialTiles.Count; i++)
				if (_specialTiles[i] is IDrawable)
					(_specialTiles[i] as IDrawable).Draw(_specialTiles[i].Index % _maxWidth, _specialTiles[i].Index / _maxWidth);

			for (int i = 0; i < _units.Count; i++)
			{
				int x = _units[i].X, y = _units[i].Y;
				switch (_units[i].Direction)
				{
					case Shooter.Direction.Left:
						x--;
						break;
					case Shooter.Direction.Right:
						x++;
						break;
					case Shooter.Direction.Up:
						y--;
						break;
					case Shooter.Direction.Down:
						y++;
						break;
				}

				bool found = false;
				for (int a = 0; a < _units[i].Guns.Count; a++)
				{
					_units[i].Guns[a].Draw(x, y);
					if (_units[i].Guns[a].Shots.Count > 0)
						found = true;
				}

				if (_units[i].Health <= 0 && !found)
				{
				    _units.Remove(_units[i]);
				    i--;
				}
			}
			for (int i = 0; i < _units.Count; i++)
				_units[i].Clear(_units[i].X, _units[i].Y);
			for (int i = 0; i < _units.Count; i++)
				_units[i].Draw(_units[i].X, _units[i].Y);
		}

		public void Update()
		{
			for (int i = 0; i < _specialTiles.Count; i++)
				if (_specialTiles[i] is IUpdateable)
					(_specialTiles[i] as IUpdateable).Update();
			for (int i = 0; i < _units.Count; i++)
				_units[i].Update();
		}

		public Map Map
		{
			get { return _map; }
			set { _map = value; }
		}

		public NetworkObservableCollection<int> Tiles
		{
			get { return _tiles; }
		}

		public NetworkObservableCollection<ITileSpecial> SpecialTiles
		{
			get { return _specialTiles; }
		}

		public NetworkObservableCollection<IUnit> Units
		{
			get { return _units; }
		}

		public int MaxHeight
		{
			get { return _maxHeight; }
			set { _maxHeight = value; }
		}

		public int MaxWidth
		{
			get { return _maxWidth; }
			set { _maxWidth = value; }
		}

		public bool Coop
		{
			get { return _coop; }
			set { _coop = value; }
		}

		public string MapRaw
		{
			get { return _mapRaw; }
			set { _mapRaw = value; }
		}

		/// <summary>
		/// Get or set the id of the object.
		/// </summary>
		public string NetworkId { get; set; }
	}
}
