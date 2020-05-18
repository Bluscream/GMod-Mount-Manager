using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace GModMountManager.UI
{
    public partial class SearchResults : Form
    {
        public List<DriveInfo> Drives;
        public DirectoryInfo Path;
        private BindingSource source;
        private System.ComponentModel.BindingList<Mount> Results = new System.ComponentModel.BindingList<Mount>();
        private List<Mount> already_mounted;
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        public SearchResults(List<DriveInfo> drives, List<Mount> already_mounted)
        {
            Drives = drives;
            this.already_mounted = already_mounted;
            InitializeComponent();
        }

        public SearchResults(DirectoryInfo path, List<Mount> already_mounted)
        {
            Path = path;
            this.already_mounted = already_mounted;
            InitializeComponent();
        }

        private async void SearchGames_Load(object sender, EventArgs e)
        {
            source = new BindingSource() { DataSource = Results };

            lst_results.AutoGenerateColumns = true;
            lst_results.DataSource = source;

            if (Path != null)
            {
                Logger.Info("Searching {}", Path.FullName);
                await Task.Run(SearchDirectory(Path));
            }
            else if (Drives != null)
            {
                foreach (DriveInfo drive in Drives)
                {
                    Logger.Info("Searching drive {} ({})", drive.VolumeLabel, drive.Name);
                    Logger.Info(drive.RootDirectory.ToJSON());
                    await Task.Run(SearchDirectory(drive.RootDirectory));
                }
            }
            Logger.Info("Finished searching");
            Text = $"Found {Results.Count} new games";
            lbl_status.Text = $"Finished searching, found {Results.Count} new games";
            Logger.Debug(Results.ToJSON(false));
            // lst_results.DataSource = source;
            lst_results.Columns["SourceMod"].ReadOnly = true;
        }

        public Action SearchDirectory(DirectoryInfo directory)
        {
            return () =>
              {
                  Queue<DirectoryInfo> folders = new Queue<DirectoryInfo>();
                  var uik = false;
                  folders.Enqueue(directory);
                  var _already_mounted = already_mounted.Select(a => a.Name);
                  while (folders.Count != 0)
                  {
                      try
                      {
                          var currentFolder = folders.Dequeue();
                          _dispatcher.Invoke(() => lbl_status.Text = $"Searching Folder {currentFolder.FullName}");
                          foreach (var file in currentFolder.GetFiles("gameinfo.txt", System.IO.SearchOption.TopDirectoryOnly))
                          {
                              if (_already_mounted.Contains(currentFolder.Name))
                              {
                                  Logger.Debug("Found already mounted game: {}", currentFolder.FullName);
                                  continue;
                              }
                              var mount = new Mount(file.DirectoryName, currentFolder.Name);
                              Logger.Debug("Found Game: {}", currentFolder.FullName);
                              _dispatcher.Invoke(() => Results.Add(mount));
                              if (!uik) { uik = true; _dispatcher.Invoke(() => lst_results.StretchLastColumn()); }
                              // _dispatcher.Invoke(() => Text = $"Searching in {currentFolder.Name.Quote()} - {Results.Count} Results");
                              _dispatcher.Invoke(() => toolStripProgressBar1.ProgressBar.Value++);
                          }

                          foreach (var _current in currentFolder.GetDirectories("*", System.IO.SearchOption.TopDirectoryOnly))
                          {
                              folders.Enqueue(_current);
                          }
                      }
                      catch (Exception ex)
                      {
                          if (ex is UnauthorizedAccessException || ex is DirectoryNotFoundException)
                          {
                          }
                          else
                          {
                              Logger.Error(ex.ToString());
                          }
                      }
                  }
              };
        }
    }
}