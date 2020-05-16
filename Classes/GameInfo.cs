using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NLog;
using System;

namespace GModMountManager.Classes
{
    public class Game
    {
        public FileInfo GameInfoPath { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Developer { get; set; }
        public string Homepage { get; set; }
        public GameType Type { get; set; } = GameType.UNKNOWN;
        public bool SupportsVR { get; set; }
        public List<Map> Maps { get; set; } = new List<Map>();

        public Game(DirectoryInfo gameDir)
        {
            var maplistfile = gameDir.CombineFile("maplist.txt");
            var maplist = new List<string>();
            if (maplistfile.Exists)
            {
                maplist = maplistfile.ReadAllLines();
                maplist.ForEach(a => a.ToLower());
            }
            foreach (var map in gameDir.Combine("maps").GetFiles("*.bsp"))
            {
                var _map = new Map(map);
                _map.Order = maplist.FindIndex(a => a == _map.Name);
                Maps.Add(_map);
            }
            Maps.Sort((x, y) => x.Order.CompareTo(y.Order));
            GameInfoPath = gameDir.CombineFile("gameinfo.txt");
            if (!GameInfoPath.Exists) throw new System.Exception("Could not find gameinfo.txt");
            try
            {
                VProperty _gameInfo = VdfConvert.Deserialize(GameInfoPath.FullName);
                GameInfo gi = VTokenExtensions.ToJson(_gameInfo).ToObject<GameInfo>();
                Name = gi.Game;
                if (Name.IsNullOrWhiteSpace()) Name = gi.Name;
                if (Name.IsNullOrWhiteSpace()) Name = gi.Title;
                if (Name.IsNullOrWhiteSpace()) Name = gi.Title2;
                Icon = gi.Icon;
                Developer = gi.Developer;
                Homepage = gi.Homepage;
                if (Homepage.IsNullOrWhiteSpace()) Name = gi.Developer_url;
                if (gi.Type == "singleplayer_only") Type = GameType.SINGLEPLAYER_ONLY;
                else if (gi.Type == "multiplayer_only") Type = GameType.MULTIPLAYER_ONLY;
                SupportsVR = gi.SupportsVR;
                foreach (var map in Maps)
                {
                    map.Hidden = gi.Hidden_maps.ContainsKey(map.Name);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                Name = gameDir.Name;
            }
        }
    }

    public enum GameType
    {
        UNKNOWN,
        SINGLEPLAYER_ONLY,
        MULTIPLAYER_ONLY
    }

    public class Map
    {
        public int Order { get; set; } = -1;

        public string Name { get; set; }

        [Browsable(false)]
        public bool Hidden { get; set; }

        [Browsable(false)]
        public FileInfo File { get; set; }

        public Map(FileInfo file)
        {
            Name = file.FileNameWithoutExtension().ToLower();
            File = file;
        }
    }

    public class GameInfo
    {
        public string Name { get; set; }
        public string Game { get; set; }
        public string Title { get; set; }
        public string Title2 { get; set; }
        public string Icon { get; set; }
        public string Developer { get; set; }
        public string Developer_url { get; set; }
        public string Homepage { get; set; }
        public string Type { get; set; }
        public bool SupportsVR { get; set; }
        public Dictionary<string, bool> Hidden_maps { get; set; }
    }
}