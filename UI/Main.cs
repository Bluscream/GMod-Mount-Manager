using GModMountManager.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace GModMountManager
{
    internal partial class Main : Form
    {
        private MountsConfig cfg;
        private BindingSource source;

        private void InvokeUI(Action a)
        {
            BeginInvoke(new MethodInvoker(a));
        }

        public Main()
        {
            Logger.Trace("START");
            if (Program.Arguments.IgnoreSSLErrors)
            {
                Logger.Warn("\"--ignoresslerrors\" is set, ignoring SSL Errors!");
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            }
            FileInfo cfgFile;
            try
            {
                cfgFile = new DirectoryInfo(SteamFinder.GetSteamLocation(4000)).CombineFile(MountsConfig.RelativePath);
            }
            catch (Exception ex) { Logger.Warn("Could not find GMod directory! ({})", ex.Message); cfgFile = Utils.pickFile("Select mount.cfg", filter: "Mount CFG|mount.cfg"); }
            InitializeComponent();
            LoadMountsCFG(cfgFile);
        }

        public void LoadMountsCFG(FileInfo cfgFile)
        {
            cfg = new MountsConfig(cfgFile);
            source = new BindingSource() { DataSource = cfg.Mounts };
            source.ListChanged += Source_ListChanged;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = source;
            dataGridView1.Columns["SourceMod"].ReadOnly = true;
            dataGridView1.AutoResizeColumns(); // DataGridViewAutoSizeColumnsMode.Fill
            dataGridView1.StretchLastColumn();
        }

        private void Source_ListChanged(object sender, ListChangedEventArgs e)
        {
            Logger.Debug("Mounts changed, saving...");
            // cfg.save();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (Program.config.Sections.ContainsSection("Window"))
            {
                var state = Program.config["Window"]["State"]; var loc = Program.config["Window"]["Location"].Split(':'); var size = Program.config["Window"]["Size"].Split(':');
                Logger.Debug("Was {} Location: {} Size: {}", Program.config["Window"]["State"], loc.ToJson(false), size.ToJson(false));
                switch (state)
                {
                    case "Maximized":
                        WindowState = FormWindowState.Maximized;
                        break;

                    default:
                        Location = new Point(int.Parse(loc[0]), int.Parse(loc[1]));
                        Size = new Size(int.Parse(size[1]), int.Parse(size[0]));
                        break;
                }
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                if (!c.Selected)
                {
                    c.DataGridView.ClearSelection();
                    c.DataGridView.CurrentCell = c;
                    c.Selected = true;
                }
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F10 && e.Shift) || e.KeyCode == Keys.Apps)
            {
                e.SuppressKeyPress = true;
                DataGridViewCell currentCell = (sender as DataGridView).CurrentCell;
                if (currentCell != null)
                {
                    ContextMenuStrip cms = currentCell.ContextMenuStrip;
                    if (cms != null)
                    {
                        Rectangle r = currentCell.DataGridView.GetCellDisplayRectangle(currentCell.ColumnIndex, currentCell.RowIndex, false);
                        Point p = new Point(r.X + r.Width, r.Y + r.Height);
                        cms.Show(currentCell.DataGridView, p);
                    }
                }
            }
        }

        private void dataGridView1_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            Logger.Debug("dataGridView1_CellContextMenuStripNeeded");
            DataGridView dgv = (DataGridView)sender;

            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;
            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                // Logger.Debug(cell.OwningRow.DataBoundItem.ToJson());
            }
            e.ContextMenuStrip = contextMenuStrip1;
            // Logger.Trace(cfg.ToJson());
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            Logger.Debug("contextMenuStrip1_Opening");
        }

        private void tsmiVoidPayment_Click(object sender, EventArgs e)
        {
            int paymentCount = dataGridView1.SelectedRows.Count;
            if (paymentCount == 0)
                return;

            bool voidPayments = false;
            string confirmText = "Are you sure you would like to void this payment?"; // to be localized
            if (paymentCount > 1)
                confirmText = "Are you sure you would like to void these payments?"; // to be localized
            voidPayments = (MessageBox.Show(
                            confirmText,
                            "Confirm", // to be localized
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2
                           ) == DialogResult.Yes);
            if (voidPayments)
            {
                // SQLTransaction Start
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    //do Work
                }
            }
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UI.Search(cfg.Mounts.ToList()).ShowDialog();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pickMountDir() is var dir && dir != null)
                cfg.Mounts.Add(new Mount(dir.FullName, dir.Name)); // source.ResetBindings(false);
        }

        private DirectoryInfo pickMountDir()
        {
            var dir = Utils.pickFolder("Select game path (like \"Half Life 2/hl2\")");
            if (dir is null || !dir.Exists) { MessageBox.Show("Invalid path!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); return null; }
            return dir;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pickMountDir() is var dir && dir != null)
            {
                ((Mount)dataGridView1.SelectedRows[0].DataBoundItem).Path = dir;
            }
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to remove these {dataGridView1.SelectedRows.Count} mounts?", "Delete mounts?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                cfg.Mounts.Remove((Mount)row.DataBoundItem);
            }
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Utils.StartProcess(cfg.File);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var file = Utils.pickFile("Select mount.cfg", cfg.File.Directory.FullName, "mount.cfg|mount.cfg|*.cfg|*.cfg");
            if (!file.Exists) { MessageBox.Show("Invalid path!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            LoadMountsCFG(file);
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                ((Mount)row.DataBoundItem).Path.ShowInExplorer();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Saving is not possible until \"https://github.com/shravan2x/Gameloop.Vdf/issues/18\" is solved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Clipboard.SetText(cfg.Mounts.ToJson());
        }

        private void createMapPoolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var mount = (Mount)row.DataBoundItem;
                var game = new Game(mount.Path);
                Logger.Debug(game.ToJson());
                new UI.CreateMapPool(game).ShowDialog();
            }
        }
    }
}