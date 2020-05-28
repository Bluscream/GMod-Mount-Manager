using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Library.Forms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace GModMountManager.Classes
{
    public class Game
    {
        public DirectoryInfo MountPath { get; set; }
        public FileInfo GameInfoPath { get; set; }
        public GameInfo GameInfo { get; set; }
        public string Name { get; set; }
        public string LongName { get { return $"{Name} {TypeStr}"; } }
        public string Icon { get; set; }
        public string Developer { get; set; }
        public string Homepage { get; set; }
        public GameType Type { get; set; } = GameType.UNKNOWN;
        public string TypeStr { get { return Type == GameType.SINGLEPLAYER_ONLY ? "Campaign" : "Map Pool"; } }
        public bool SupportsVR { get; set; }
        public SortableBindingList<Map> Maps { get; set; } = new SortableBindingList<Map>();
        public FileInfo MapListPath { get; set; }
        public List<string> MapList { get; set; } = new List<string>();
        public FileInfo MapCyclePath { get; set; }
        public List<string> MapCycle { get; set; } = new List<string>();

        internal void LoadMapList(FileInfo path = null)
        {
            MapListPath = path ?? MountPath.CombineFile("maplist.txt");
            if (MapListPath.Exists)
            {
                MapList = MapListPath.ReadAllLines().Select(a => a.ToLower()).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
            }
        }

        internal void LoadMapCycle(FileInfo path = null)
        {
            MapCyclePath = path ?? MountPath.CombineFile("mapcycle.txt");
            if (MapCyclePath.Exists)
            {
                MapCycle = MapCyclePath.ReadAllLines().Select(a => a.ToLower()).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
            }
        }

        public Game(DirectoryInfo gameDir)
        {
            MountPath = gameDir;
            LoadMapList();
            LoadMapCycle();
            foreach (var map in gameDir.Combine("maps").GetFiles("*.*").Where(s => s.Extension == ".bsp" || s.Extension == ".vmf"))
            {
                var _map = new Map(map);
                // if (!_map.Background) _map.Order = MapList.FindIndex(a => a == _map.Name) + 1;
                // else _map.Hidden = true;
                Maps.Add(_map);
            }
            foreach (var map in MapList.Where(m => !Map.isBackground(m)).ToList())
            {
                var _map = Maps.Where(m => m.Name == map).FirstOrDefault();
                if (_map is null)
                {
                    Maps.Add(new Map(map));
                }
            }
            foreach (var map in MapCycle)
            {
                var _map = Maps.Where(m => m.Name == map).FirstOrDefault();
                if (_map is null) Maps.Add(new Map(map));
            }
            GameInfoPath = gameDir.CombineFile("gameinfo.txt");
            if (!GameInfoPath.Exists) throw new System.Exception("Could not find gameinfo.txt");
            try
            {
                var text = File.ReadAllText(GameInfoPath.FullName);
                var gameInfo = VdfConvert.Deserialize(text);
                var json = gameInfo.ToJson(new VdfJsonConversionSettings() { ObjectDuplicateKeyHandling = DuplicateKeyHandling.Ignore, ValueDuplicateKeyHandling = DuplicateKeyHandling.Ignore });
                var hi = new JObject(json);
                Logger.Warn(hi.ToString());
                GameInfo = hi.ToObject<GameInfoWrapper>().GameInfo;
                Logger.Warn(GameInfo.ToString());
                Name = GameInfo.Game ?? Name;
                if (Name.IsNullOrWhiteSpace()) Name = GameInfo.Name ?? Name;
                if (Name.IsNullOrWhiteSpace()) Name = GameInfo.Title ?? Name;
                if (Name.IsNullOrWhiteSpace()) Name = GameInfo.Title2 ?? Name;
                Icon = GameInfo.Icon ?? Icon;
                Developer = GameInfo.Developer ?? null;
                Homepage = GameInfo.Homepage ?? null;
                if (Homepage.IsNullOrWhiteSpace()) Homepage = GameInfo.DeveloperUrl ?? null;
                if (GameInfo.Type.ToLower() == "singleplayer_only") Type = GameType.SINGLEPLAYER_ONLY;
                else if (GameInfo.Type.ToLower() == "multiplayer_only") Type = GameType.MULTIPLAYER_ONLY;
                SupportsVR = GameInfo.SupportsVR == 1;
                if (GameInfo.Hidden_maps != null)
                {
                    foreach (var map in Maps)
                    {
                        map.Hidden = GameInfo.Hidden_maps.ContainsKey(map.Name);
                        // if (map.Hidden) map.Order = -1;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error(ex.ToString());
                Name = gameDir.Name;
            }
            if (MapList.Count > 0)
            {
                var maplist_nobg = MapList.Where(m => !Map.isBackground(m)).ToList();
                foreach (var map in Maps)
                {
                    if (!map.Hidden && maplist_nobg.Contains(map.Name)) map.Order = maplist_nobg.IndexOf(map.Name) + 1;
                }
            }
            else
            {
                foreach (var (chapter, i) in gameDir.Combine("cfg").GetFiles("chapter*.cfg").WithIndex())
                {
                    // ?? int.Parse(chapter.Name.GetDigits());
                    var map = chapter.ReadAllLines()[0].Split(" ").Last();
                    var hasMap = Maps.Where(m => m.Name == map).FirstOrDefault();
                    if (hasMap is null) Maps.Add(new Map(map) { Order = i + 1 });
                    else hasMap.Order = i + 1;
                }
            }
            // Maps.Sort((x, y) => x.Name.CompareTo(y.Name));
            // Maps.Reverse();
            /*if (MapList.Count > 0)*/
            // Maps.Sort((x, y) => x.Order.CompareTo(y.Order));
        }

        public List<string> GenerateMapList()
        {
            return Maps.Where(m => m.Order > 0 && !m.Hidden).ToList().OrderBy(m => m.Order).Select(m => m.Name).ToList();
        }

        public List<string> GenerateMapCycle()
        {
            return Maps.Where(m => m.Order < 1 && !m.Hidden).ToList().OrderBy(m => m.Order).Select(m => m.Name).ToList();
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
        // [Browsable(false)]
        public int Order { get; set; } = -1;

        public bool Hidden { get; set; }

        // public bool Background { get; }

        public string Name { get; set; }

        [Browsable(false)]
        public FileInfo File { get; set; }

        public Map(FileInfo file)
        {
            Name = file.FileNameWithoutExtension().ToLower();
            Hidden = isBackground(Name);
            File = file;
        }

        public Map(string name)
        {
            Name = name;
            Hidden = isBackground(Name);
        }

        public static bool isBackground(string name)
        {
            return name.Contains("background") || name.StartsWith("bg");
        }
    }
}