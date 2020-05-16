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
    public partial class Search : Form
    {
        public Search()
        {
            InitializeComponent();
        }

        private void Search_Load(object sender, EventArgs e)
        {
            lst_drives.CheckBoxes = true;
            lst_drives.SmallImageList = new ImageList() {};
            this.mainColumn.ImageGetter = delegate(object row) {
                String key = this.GetImageKey(row);
                if (!this.listView.LargeImageList.Images.ContainsKey(key)) {
                    Image smallImage = this.GetSmallImageFromStorage(key);
                    Image largeImage = this.GetLargeImageFromStorage(key);
                    this.listView.SmallImageList.Images.Add(key, smallImage);
                    this.listView.LargeImageList.Images.Add(key, largeImage);
                }
                return key;
            };
            foreach (DriveInfo d in DriveInfo.GetDrives().Where(x => x.IsReady))
            {
                lst_drives.Items.Add($"{d.VolumeLabel} ({d.Name})");
            }
        }

        private void btn_folder_Click(object sender, EventArgs e)
        {
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            new SearchResults().ShowDialog();
        }
    }
}