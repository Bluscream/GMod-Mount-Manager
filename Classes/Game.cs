using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System;
using Newtonsoft.Json;

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
                var text = File.ReadAllText(GameInfoPath.FullName);
                var gameInfo = VdfConvert.Deserialize(text);
                var json = gameInfo.ToJson(new VdfJsonConversionSettings() { ObjectDuplicateKeyHandling = DuplicateKeyHandling.Ignore, ValueDuplicateKeyHandling = DuplicateKeyHandling.Ignore });
                Logger.Error(json.ToString());
                var gi = json.ToObject<GameInfos>().gameInfo;
                Name = gi.Game;
                /*if (Name.IsNullOrWhiteSpace()) Name = gi.Name;
                if (Name.IsNullOrWhiteSpace()) Name = gi.Title;
                if (Name.IsNullOrWhiteSpace()) Name = gi.Title2;
                Icon = gi.Icon;
                Developer = gi.Developer;
                Homepage = gi.Homepage.AbsoluteUri;
                if (Homepage.IsNullOrWhiteSpace()) Homepage = gi.DeveloperUrl.AbsoluteUri;
                if (gi.Type == "singleplayer_only") Type = GameType.SINGLEPLAYER_ONLY;
                else if (gi.Type == "multiplayer_only") Type = GameType.MULTIPLAYER_ONLY;
                SupportsVR = gi.SupportsVR == 1;*/
                foreach (var map in Maps)
                {
                    // map.Hidden = gi.Hidden_maps.ContainsKey(map.Name);
                }
            }
            catch (UnauthorizedAccessException ex)
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

    public class GameInfos
    {
        public GameInfo gameInfo { get; set; }
    }

    public class GameInfo
    {
        [Newtonsoft.Json.JsonProperty("game", NullValueHandling = NullValueHandling.Ignore)]
        public string Game { get; set; }
    }
}