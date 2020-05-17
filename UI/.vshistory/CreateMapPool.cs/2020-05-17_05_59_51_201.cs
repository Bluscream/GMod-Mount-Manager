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
using System.Windows.Controls.Primitives;
using NLog.Fluent;
using System.Drawing.Imaging;

namespace GModMountManager.UI
{
    public partial class CreateMapPool : Form
    {
        private DirectoryInfo gmodDir;
        private Game game;
        private BindingSource source;
        private DirectoryInfo addonDir;
        private FileInfo addonFile;

        public CreateMapPool(Game game, DirectoryInfo gmodDir)
        {
            this.game = game; this.gmodDir = gmodDir;
            InitializeComponent();
            txt_longname.Text = game.LongName;
            Text = $"Create Map Pool for {game.Name}";
            btn_create.Enabled = addonDir.Exists;
            btn_upload.Enabled = addonFile.Exists;
        }

        private void CreateMapPool_Load(object sender, EventArgs e)
        {
            txt_shortname.Text = game.MountPath.Name;
            txt_name.Text = game.Name;
            txt_dev.Text = game.Developer;
            txt_url.Text = game.Homepage;

            source = new BindingSource() { DataSource = game.Maps };

            lst_maps.AutoGenerateColumns = true;
            lst_maps.DataSource = source;
            // lst_maps.Columns["Order"].ReadOnly = true;
            lst_maps.AutoResizeColumns();
            lst_maps.StretchLastColumn();
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == txt_longname)
            {
                addonDir = gmodDir.Combine("garrysmod", "addons", txt_longname.Text.RemoveInvalidFileNameChars());
                addonFile = gmodDir.CombineFile("garrysmod", "addons", txt_longname.Text.RemoveInvalidFileNameChars() + ".gma");
            }
            txt_description.Text = Properties.Resources.steam_description.Format(txt_url.Text, txt_name.Text, txt_shortname.Text, game.TypeStr);
        }

        private void txt_longname_TextChanged(object sender = null, EventArgs e = null)
        {
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
        }

        private void btn_create_Click(object sender, EventArgs e)
        {
            var file = Utils.saveFile($"Save {addonFile.Name}", addonFile.DirectoryName, fileName: addonFile.Name);
            file.Create(); // Todo: Change
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
                // Logger.Debug(cell.OwningRow.DataBoundItem.ToJSON());
            }
            e.ContextMenuStrip = contextMenuStrip1;
        }

        private void btn_create_addon_Click(object sender, EventArgs e)
        {
            addonDir.Create();
            addonDir.CombineFile("addon.json").WriteAllText(Properties.Resources.addon_json.Format(txt_name.Text)); // "{\"title\":\"" + txt_longname + "\",\"type\":\"gamemode\"}");
            var gameModeDir = addonDir.Combine("gamemodes", txt_shortname.Text, "gamemode");
            gameModeDir.Create();
            gameModeDir.Parent.CombineFile(txt_shortname.Text + ".txt").WriteAllText(Properties.Resources.gamemode_txt.Format(txt_shortname.Text, txt_name.Text, string.Join("|", game.Maps.Select(m => m.Name))));
            gameModeDir.CombineFile("cl_init.lua").WriteAllText(Properties.Resources.cl_init_lua);
            gameModeDir.CombineFile("init.lua").WriteAllText(Properties.Resources.init_lua);
            gameModeDir.CombineFile("shared.lua").WriteAllText(Properties.Resources.shared_lua.Format(txt_name.Text, txt_dev.Text, txt_url.Text));
            if (lst_maps.Rows.Count > 0)
            {
                var thumbDir = addonDir.Combine("maps", "thumb");
                thumbDir.Create();
                foreach (var map in game.Maps)
                {
                    var stream = thumbDir.CombineFile($"{map.Name}.png").OpenWrite();
                    var img = Utils.DrawText(game.Name, DefaultFont, Color.White, Color.Transparent, 256, 256);
                    img.DrawString(label, new Font("verdana", 10, GraphicsUnit.Pixel), myBrush, 10, 10);
                    img.Save(stream, ImageFormat.Png);
                    stream.Close();
                }
            }
            btn_create.Enabled = true;
        }

        private static Image CreateThumbnailOverlay(string title, Font font, int order = -1, Color? _textColor = null, Color? _backColor = null, int height = 256, int width = 256, Image baseImage = null)
        {
            var textColor = _textColor ?? Color.Black;
            var backColor = _backColor ?? Color.Transparent;
            Image img = baseImage ?? new Bitmap(width, height);
            Graphics drawing = Graphics.FromImage(img);
            drawing.Clear(backColor);
            Brush textBrush = new SolidBrush(textColor);
            drawing.DrawString(title, font, textBrush, 10, 10);
            if (order > -1) drawing.DrawString(order.ToString(), font, textBrush, width.Percentage(80), height.Percentage(80));
            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();
            return img;
        }
    }
}