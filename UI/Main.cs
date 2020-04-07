using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using EpicMorg.SteamPathsLib;

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
            var GetActiveProcessSteamData = SteamPathsUtil.GetActiveProcessSteamData();
            var GetAllSteamAppManifestData = SteamPathsUtil.GetAllSteamAppManifestData();
            var GetLibrarySteamDataList = SteamPathsUtil.GetLibrarySteamDataList();
            var GetSteamAppsKeyRegistry = SteamPathsUtil.GetSteamAppsKeyRegistry();
            var GetSteamConfig = SteamPathsUtil.GetSteamConfig();
            var GetSteamData = SteamPathsUtil.GetSteamData();
            var gmod = SteamPathsUtil.GetSteamAppDataById(4000);
            if (!gmod.Installed) cfgFile = Utils.pickFile("Select mount.cfg", filter: "Mount CFG|mount.cfg");
            else
            {
                // var data = SteamPathsUtil.GetSteamAppManifestDataById(4000);
                cfgFile = new DirectoryInfo(SteamFinder.GetSteamLocation(4000)).CombineFile(MountsConfig.RelativePath);
            }
            cfg = new MountsConfig(cfgFile);
            source = new BindingSource() { DataSource = cfg.Mounts };
            source.ListChanged += Source_ListChanged;

            InitializeComponent();
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
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = source;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.AutoResizeColumns(); // DataGridViewAutoSizeColumnsMode.Fill
            dataGridView1.StretchLastColumn();
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
            Logger.Debug(cfg.ToJson());
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            Logger.Debug("contextMenuStrip1_Opening");
            int itemCount = dataGridView1.SelectedRows.Count;
            string voidPaymentText = "&Void Payment"; // to be localized
            if (itemCount > 1)
                voidPaymentText = "&Void Payments"; // to be localized
                                                    // if (tsmiVoidPayment.Text != voidPaymentText) // avoid possible flicker
                                                    // tsmiVoidPayment.Text = voidPaymentText;
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
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dir = Utils.pickFolder("Select game path (like \"Half Life 2/hl2\")");
            if (!dir.Exists) { MessageBox.Show("Invalid path!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            cfg.Mounts.Add(new Mount(dir.FullName, dir.Name)); source.ResetBindings(false);
        }
    }
}