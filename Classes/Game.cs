﻿using Gameloop.Vdf;
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
        public FileInfo MapListPath { get; set; }
        public List<string> MapList { get; set; } = new List<string>();
        public FileInfo MapCyclePath { get; set; }
        public List<string> MapCycle { get; set; } = new List<string>();

        public Game(DirectoryInfo gameDir)
        {
            MountPath = gameDir;
            MapListPath = gameDir.CombineFile("maplist.txt");
            if (MapListPath.Exists)
            {
                MapList = MapListPath.ReadAllLines().Select(a => a.ToLower()).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
            }
            MapCyclePath = gameDir.CombineFile("mapcycle.txt");
            if (MapCyclePath.Exists)
            {
                MapCycle = MapCyclePath.ReadAllLines().Select(a => a.ToLower()).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
            }
            foreach (var map in gameDir.Combine("maps").GetFiles("*.*").Where(s => s.Extension == ".bsp" || s.Extension == ".vmf"))
            {
                var _map = new Map(map);
                if (!_map.isBackground) _map.Order = MapList.FindIndex(a => a == _map.Name) + 1;
                else _map.Hidden = true;
                Maps.Add(_map);
            }
            // var _mapnames = Maps.Select(m => m.Name);
            foreach (var (map, i) in MapList.WithIndex())
            {
                var _map = Maps.Where(m => m.Name == map).FirstOrDefault();
                if (_map != null) Maps.Add(new Map(map) { Order = i });
            }
            foreach (var map in MapCycle)
            {
                var _map = Maps.Where(m => m.Name == map).FirstOrDefault();
                if (_map != null) Maps.Add(new Map(map));
            }
            Maps.Sort((x, y) => x.Name.CompareTo(y.Name));
            // Maps.Reverse();
            if (MapList.Count > 0) Maps.Sort((x, y) => x.Order.CompareTo(y.Order));
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
                        if (map.Hidden) map.Order = -1;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error(ex.ToString());
                Name = gameDir.Name;
            }
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

        public bool isBackground {get; }

        public string Name { get; set; }

        [Browsable(false)]
        public FileInfo File { get; set; }

        public Map(FileInfo file)
        {
            Name = file.FileNameWithoutExtension().ToLower();
            isBackground = Name.Contains("background") || Name.StartsWith("bg");
            File = file;
        }

        public Map(string name)
        {
            Name = name;
        }
    }
}