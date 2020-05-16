using GModMountManager.Classes;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GModMountManager.UI
{
    public partial class CreateMapPool : Form
    {
        private Game game { get; set; }
        private BindingSource source;

        public CreateMapPool(Game game)
        {
            this.game = game;
            InitializeComponent();
            Text = $"Create Map Pool for {game.Name}";
        }

        private void CreateMapPool_Load(object sender, EventArgs e)
        {
            txt_name.Text = game.Name;
            txt_dev.Text = game.Developer;
            txt_url.Text = game.Homepage;

            source = new BindingSource() { DataSource = game.Maps };

            lst_maps.AutoGenerateColumns = true;
            lst_maps.DataSource = source;
            lst_maps.Columns["Order"].ReadOnly = true;
            lst_maps.AutoResizeColumns();
            lst_maps.StretchLastColumn();
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
        }

        private void btn_create_Click(object sender, EventArgs e)
        {
            var fileName = $"{game.Name}.gma";
            var file = Utils.saveFile($"Save {fileName}", Path.GetTempPath());
            if (file is null || !file.Exists) return;
            Logger.Log("Saved {}", file.FullName);
            btn_upload.Enabled = true;
        }

        private void lst_maps_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            Logger.Debug("lst_maps_CellContextMenuStripNeeded");
            DataGridView dgv = (DataGridView)sender;

            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;
            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                // Logger.Debug(cell.OwningRow.DataBoundItem.ToJson());
            }
            e.ContextMenuStrip = contextMenuStrip1;
        }
    }
}