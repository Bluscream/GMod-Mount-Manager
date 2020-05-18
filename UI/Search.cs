using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GModMountManager.UI
{
    public partial class Search : Form
    {
        private List<Mount> already_mounted;

        public Search(List<Mount> already_mounted)
        {
            this.already_mounted = already_mounted;
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
                lst_drives.Items.Add(new Drive() { Info = Info }, Info.DriveType == DriveType.Fixed);
            }
        }

        private void btn_folder_Click(object sender, EventArgs e)
        {
            var dir = Utils.pickFolder("Select Folder to search...");
            if (dir is null) return;
            this.Hide();
            new SearchResults(dir, already_mounted).ShowDialog();
            this.Close();
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            var l = lst_drives.CheckedItems.Cast<Drive>().Select(a => a.Info).ToList();
            if (l.Count < 1) return;
            this.Hide();
            new SearchResults(l, already_mounted).ShowDialog();
            this.Close();
        }
    }
}