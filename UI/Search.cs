using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace GModMountManager.UI
{
    public partial class Search : Form
    {
        public Search()
        {
            InitializeComponent();
        }

        public class Drive
        {
            public DriveInfo Info;

            public override string ToString()
            {
                return $"{Info.VolumeLabel} ({Info.Name})";
            }
        }

        private void Search_Load(object sender, EventArgs e)
        {
            foreach (DriveInfo Info in DriveInfo.GetDrives().Where(x => x.IsReady))
            {
                lst_drives.Items.Add(new Drive() { Info = Info }, true);
            }
        }

        private void btn_folder_Click(object sender, EventArgs e)
        {
            var dir = Utils.pickFolder("Select Folder to search...");
            if (dir is null) return;
            this.Hide();
            new SearchResults(dir).ShowDialog();
            this.Close();
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            var l = lst_drives.CheckedItems.Cast<Drive>().Select(a => a.Info).ToList();
            if (l.Count < 1) return;
            this.Hide();
            new SearchResults(l).ShowDialog();
            this.Close();
        }
    }
}