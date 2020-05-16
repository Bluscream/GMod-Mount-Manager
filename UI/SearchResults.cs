using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GModMountManager.UI
{
    public partial class SearchResults : Form
    {
        public SearchResults()
        {
            InitializeComponent();
        }

        private void SearchGames_Load(object sender, EventArgs e)
        {

        }

        public List<string> Search()
        {
            var files = new List<string>();
            foreach (DriveInfo d in DriveInfo.GetDrives().Where(x => x.IsReady))
            {
                try
                {
                    files.AddRange(Directory.GetFiles(d.RootDirectory.FullName, "*.txt", SearchOption.AllDirectories));
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message); // Log it and move on
                }
            }

            return files;
        }
    }
}