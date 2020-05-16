using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GModMountManager.UI
{
    public partial class SearchResults : Form
    {
        public List<DriveInfo> Drives;
        public DirectoryInfo Path;
        private BindingSource source;
        private ObservableCollection<Mount> Results = new ObservableCollection<Mount>();

        public SearchResults(List<DriveInfo> drives)
        {
            Drives = drives;
            InitializeComponent();
        }

        public SearchResults(DirectoryInfo path)
        {
            Path = path;
            InitializeComponent();
        }

        private async void SearchGames_Load(object sender, EventArgs e)
        {
            source = new BindingSource() { DataSource = Results };

            lst_results.AutoGenerateColumns = true;
            lst_results.DataSource = source;
            // lst_results.Columns["SourceMod"].ReadOnly = true;
            lst_results.AutoResizeColumns();
            // lst_results.StretchLastColumn();
            if (Path != null)
            {
                Logger.Info("Searching {}", Path.FullName);
                await Task.Run(SearchDirectory(Path));
            }
            else if (Drives != null)
            {
                foreach (DriveInfo drive in Drives)
                {
                    Logger.Info("Searching drive \"{}\" ({})", drive.VolumeLabel, drive.Name);
                    await Task.Run(SearchDirectory(drive.RootDirectory));
                }
            }
            Logger.Info("Finished searching");
            lst_results.DataSource = source;
            lst_results.Columns["SourceMod"].ReadOnly = true;
            lst_results.AutoResizeColumns();
        }

        public Action SearchDirectory(DirectoryInfo directory)
        {
            return () =>
              {
                  try
                  {
                      foreach (var file in directory.GetFiles("gameinfo.txt", SearchOption.AllDirectories))
                      {
                          var mount = new Mount(file.DirectoryName, file.Directory.Name);
                          Logger.Debug("Found Game: {}", mount.Path);
                          Results.Add(mount);
                          // lst_results.DataSource = null;
                          // lst_results.DataSource = source;
                      }
                      // Logger.Warn(Results.ToJson(false));
                  }
                  catch (UnauthorizedAccessException ex)
                  {
                      Logger.Trace(ex.Message);
                  }
                  // lst_results.StretchLastColumn();
              };
        }
    }
}