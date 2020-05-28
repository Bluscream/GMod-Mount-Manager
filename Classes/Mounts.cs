using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace GModMountManager
{
    public class MountsConfig
    {
        public const string RelativePath = "garrysmod/cfg/mount.cfg";

        // public static DirectoryInfo SourcemodsDir = new DirectoryInfo(EpicMorg.SteamPathsLib.SteamPathsUtil.GetSteamData().SourceModInstallPath);
        public FileInfo File { get; internal set; }

        public BindingList<Mount> Mounts { get; set; }

        public MountsConfig(FileInfo file)
        {
            if (file is null) file = new FileInfo(RelativePath);
            this.File = file;
            Logger.Info("Reading mounts from {}", file.FullName);
            var text = this.File.ReadAllText();
            var vp = VdfConvert.Deserialize(text, VdfSerializerSettings.Default);
            Mounts = new BindingList<Mount>(vp.Value.Children().Select(x => new Mount(x.Value.Value<string>(), x.Key)).ToList()); // TODO: .Properties()
            Logger.Info("Read {} mounts successfully", Mounts.Count);
            // Logger.Trace(Mounts.ToJSON());
        }

        public void Save(FileInfo file = null)
        {
            file = file ?? File;
            var bakfile = file.Combine(".bak");
            var str = Extensions.toVdf(Mounts); // VdfConvert.Serialize(VToken()); // TODO: https://github.com/shravan2x/Gameloop.Vdf/issues/19
            if (str == file.ReadAllText())
            {
                Logger.Trace("File already contains proposed changes, discarding save..."); return;
            }
            if (!bakfile.Exists || bakfile.ReadAllText() != str)
            {
                Logger.Trace("Backup file does not exist or does not contain current content, copying {} to {}", file.Name, bakfile.Name);
                file.CopyTo(bakfile.FullName);
            }
            Logger.Debug("Saving to {}", File.FullName);
            file.WriteAllText(str);
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
                SourceMod = (value.Parent.Name.ToLower() == "sourcemods") ? true : false;
                _path = value;
            }
        }

        public Mount(string path, string name = null)
        {
            Path = new DirectoryInfo(path);
            Name = name ?? Path.Name;
            //if (Path.FullName.StartsWith(MountsConfig.SourcemodsDir.FullName, System.StringComparison.OrdinalIgnoreCase)) SourceMod = true;
            Logger.Trace("Created new mount: {}", this.ToJSON(false));
        }
    }
}