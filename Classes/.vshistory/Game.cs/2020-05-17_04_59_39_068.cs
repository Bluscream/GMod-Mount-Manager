using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace GModMountManager.Classes
{
    public class Game
    {
        public DirectoryInfo MountPath { get; set; }
        public FileInfo GameInfoPath { get; set; }
        public string Name { get; set; }
        public string LongName { get { return $"{Name} {TypeStr}"; } }
        public string Icon { get; set; }
        public string Developer { get; set; }
        public string Homepage { get; set; }
        public GameType Type { get; set; } = GameType.UNKNOWN;
        public string TypeStr { get { return Type == GameType.SINGLEPLAYER_ONLY ? "Campaign" : "Map Pool"; } }
        public bool SupportsVR { get; set; }
        public List<Map> Maps { get; set; } = new List<Map>();

        public Game(DirectoryInfo gameDir)
        {
            MountPath = gameDir;
            var maplistfile = gameDir.CombineFile("maplist.txt");
            var maplist = new List<string>();
            if (maplistfile.Exists)
            {
                maplist = maplistfile.ReadAllLines().Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
                maplist.ForEach(a => a.ToLower());
            }
            foreach (var map in gameDir.Combine("maps").GetFiles("*.bsp"))
            {
                var _map = new Map(map);
                _map.Order = maplist.FindIndex(a => a == _map.Name);
                Maps.Add(_map);
            }
            Maps.Sort((x, y) => x.Name.CompareTo(y.Name));
            // Maps.Reverse();
            if (maplist.Count > 0) Maps.Sort((x, y) => x.Order.CompareTo(y.Order));
            GameInfoPath = gameDir.CombineFile("gameinfo.txt");
            if (!GameInfoPath.Exists) throw new System.Exception("Could not find gameinfo.txt");
            try
            {
                var text = File.ReadAllText(GameInfoPath.FullName);
                var gameInfo = VdfConvert.Deserialize(text);
                var json = gameInfo.ToJson(new VdfJsonConversionSettings() { ObjectDuplicateKeyHandling = DuplicateKeyHandling.Ignore, ValueDuplicateKeyHandling = DuplicateKeyHandling.Ignore });
                var hi = new JObject(json);
                Logger.Warn(hi.ToString());
                var gi = hi.ToObject<GameInfoWrapper>().GameInfo;
                Logger.Warn(gi.ToString());
                Name = gi.Game ?? Name;
                if (Name.IsNullOrWhiteSpace()) Name = gi.Name ?? Name;
                if (Name.IsNullOrWhiteSpace()) Name = gi.Title ?? Name;
                if (Name.IsNullOrWhiteSpace()) Name = gi.Title2 ?? Name;
                Icon = gi.Icon ?? Icon;
                Developer = gi.Developer ?? null;
                Homepage = gi.Homepage ?? null;
                if (Homepage.IsNullOrWhiteSpace()) Homepage = gi.DeveloperUrl ?? null;
                if (gi.Type == "singleplayer_only") Type = GameType.SINGLEPLAYER_ONLY;
                else if (gi.Type == "multiplayer_only") Type = GameType.MULTIPLAYER_ONLY;
                if (gi.SupportsVR != null) SupportsVR = gi.SupportsVR == 1;
                if (gi.Hidden_maps != null)
                {
                    foreach (var map in Maps)
                    {
                        map.Hidden = gi.Hidden_maps.ContainsKey(map.Name);
                    }
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
        [Browsable(false)]
        public int Order { get; set; } = -1;

        public bool Hidden { get; set; }

        public string Name { get; set; }

        [Browsable(false)]
        public FileInfo File { get; set; }

        public Map(FileInfo file)
        {
            Name = file.FileNameWithoutExtension().ToLower();
            File = file;
        }
    }
}