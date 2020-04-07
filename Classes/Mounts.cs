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
        public FileInfo File { get; internal set; }

        public List<Mount> Mounts { get; set; }

        public MountsConfig(FileInfo file)
        {
            if (file is null) file = new FileInfo("garrysmod/cfg/mount.cfg");
            this.File = file;
            var text = this.File.ReadAllText();
            var vp = VdfConvert.Deserialize(text, VdfSerializerSettings.Default);
            Mounts = vp.Value.Children().Select(x => new Mount(x.Key, x.Value.Value<string>())).ToList();
        }

        public MountsConfig(List<Mount> mounts)
        {
            this.Mounts = mounts;
        }

        public void save()
        {
            var bakfile = File.Combine(".bak");
            var str = ""; // VdfConvert.Serialize(VToken()); // TODO: https://github.com/shravan2x/Gameloop.Vdf/issues/19
            if (!bakfile.Exists || bakfile.ReadAllText() != str)
                File.CopyTo(bakfile.FullName);
            File.WriteAllText(str);
        }
    }

    public class Mount
    {
        // [JsonIgnore]
        public bool Enabled { get; set; }

        public bool isSourceMod { get; set; }

        public string Name { get; set; }
        public DirectoryInfo Path { get; set; }

        public Mount(string name, string path)
        {
            this.Name = name; this.Path = new DirectoryInfo(path);
        }
    }
}