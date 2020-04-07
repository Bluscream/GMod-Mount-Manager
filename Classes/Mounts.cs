using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GModMountManager
{
    public class MountsConfig
    {
        public const string RelativePath = "garrysmod/cfg/mount.cfg";

        // public static DirectoryInfo SourcemodsDir = new DirectoryInfo(EpicMorg.SteamPathsLib.SteamPathsUtil.GetSteamData().SourceModInstallPath);
        public FileInfo File { get; internal set; }

        public List<Mount> Mounts { get; set; }

        public MountsConfig(FileInfo file)
        {
            if (file is null) file = new FileInfo(RelativePath);
            this.File = file;
            Logger.Info("Reading mounts from {}", file.FullName.Quote());
            var text = this.File.ReadAllText();
            var vp = VdfConvert.Deserialize(text, VdfSerializerSettings.Default);
            Mounts = vp.Value.Children().Select(x => new Mount(x.Value.Value<string>(), x.Key)).ToList();
            Logger.Info("Read {} mounts successfully", Mounts.Count);
            Logger.Trace(Mounts.ToJson());
        }

        public MountsConfig(List<Mount> mounts)
        {
            this.Mounts = mounts;
        }

        public void save()
        {
            var bakfile = File.Combine(".bak");
            var str = ""; // VdfConvert.Serialize(VToken()); // TODO: https://github.com/shravan2x/Gameloop.Vdf/issues/19
            if (str == File.ReadAllText())
            {
                Logger.Trace("File already contains proposed changes, discarding save..."); return;
            }
            if (!bakfile.Exists || bakfile.ReadAllText() != str)
            {
                Logger.Trace("Backup file does not exist or does not contain current content, copying {} to {}", File.Name, bakfile.Name);
                File.CopyTo(bakfile.FullName);
            }
            Logger.Trace("Saving to {}", File.FullName.Quote());
            File.WriteAllText(str);
        }
    }

    public class Mount
    {
        // [JsonIgnore]
        public bool Enabled { get; set; } = true;

        public bool SourceMod { get; set; }

        public string Name { get; set; }
        private DirectoryInfo _path { get; set; }

        public DirectoryInfo Path
        {
            get { return _path; }
            set
            {
                SourceMod = (value.Parent.Name == "sourcemods") ? true : false;
                _path = value;
            }
        }

        public Mount(string path, string name = null)
        {
            Path = new DirectoryInfo(path);
            Name = name ?? Path.Name;
            //if (Path.FullName.StartsWith(MountsConfig.SourcemodsDir.FullName, System.StringComparison.OrdinalIgnoreCase)) SourceMod = true;
            Logger.Trace("Created new mount: {}", this.ToJson(false));
        }
    }
}