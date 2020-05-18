using GModMountManager.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GModMountManager.UI
{
    public partial class CreateMapPool : Form
    {
        private DirectoryInfo gmodDir;
        private Game game;
        private BindingSource source;
        private DirectoryInfo addonDir;
        private FileInfo addonFile;
        private Random random = new Random();
        private Color RandomColor;
        private List<ImageFile> baseImages = new List<ImageFile>();
        private bool ShownSuccessMessage = false;

        public CreateMapPool(Game game, DirectoryInfo gmodDir)
        {
            this.game = game; this.gmodDir = gmodDir;
            InitializeComponent();
            RandomColor = Color.FromArgb(random.Next(200, 256), random.Next(200, 256), random.Next(200, 256));
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
            txt_description.Text = Properties.Resources.steam_description.Format(txt_url.Text, txt_name.Text.Replace("\\n", " "), txt_shortname.Text, game.TypeStr);
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
            addonDir.CombineFile("addon.json").WriteAllText(Properties.Resources.addon_json.Format(txt_longname.Text));
            var gameModeDir = addonDir.Combine("gamemodes", txt_shortname.Text, "gamemode");
            gameModeDir.Create();
            gameModeDir.Parent.CombineFile(txt_shortname.Text + ".txt").WriteAllText(Properties.Resources.gamemode_txt.Format(txt_shortname.Text, txt_name.Text.Replace("\\n", " "), string.Join("|", game.Maps.Select(m => m.Name))));
            gameModeDir.CombineFile("cl_init.lua").WriteAllText(Properties.Resources.cl_init_lua);
            gameModeDir.CombineFile("init.lua").WriteAllText(Properties.Resources.init_lua);
            gameModeDir.CombineFile("shared.lua").WriteAllText(Properties.Resources.shared_lua.Format(txt_longname.Text, txt_dev.Text, txt_url.Text));
            if (lst_maps.Rows.Count > 0)
            {
                var thumbDir = addonDir.Combine("maps", "thumb");
                thumbDir.Create();
                foreach (var map in game.Maps)
                {
                    var stream = thumbDir.CombineFile($"{map.Name}.png").OpenWrite();
                    Image baseImage = null;
                    var baseImg = baseImages.Where(i => i.Name == map.Name).FirstOrDefault();
                    if (baseImg != null) baseImage = Image.FromFile(baseImg.Path);
                    CreateThumbnailOverlay(title: txt_name.Text, font: new Font("Segoe UI", 25), order: map.Order, _textColor: null/*RandomColor*/, baseImage: baseImage).Save(stream, ImageFormat.Png);
                    stream.Close();
                }
            }
            btn_create.Enabled = true;
            if (!ShownSuccessMessage)
            {
                var Dialogresult = MessageBox.Show($"Addon {txt_longname.Text} has been generated.\n\nDo you want to open it's folder now?", "Finished creating addon", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Dialogresult == DialogResult.Yes) addonDir.ShowInExplorer();
                ShownSuccessMessage = true;
            }
            var _maplist = game.GenerateMapList();
            if (!game.MapList.SequenceEqual(_maplist))
            {
                var Dialogresult = MessageBox.Show($"Maplist has been changed ({game.MapList.Count} > {_maplist.Count}).\n\nDo you want to overwrite it with the new order so it's loaded automatically next time?", "Maplist changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Dialogresult == DialogResult.Yes) saveMapLists(_maplist);
            }
        }

        private void saveMapLists(List<string> maps = null)
        {
            maps = maps ?? game.GenerateMapList();
            game.MapListPath.Backup(true);
            game.MapListPath.WriteAllText(maps.Join("\n"));

            game.MapCyclePath.Backup(true);
            game.MapCyclePath.WriteAllText(game.GenerateMapCycle().Join("\n"));
        }

        private void saveToolStripMenuItem_Click(object sender = null, EventArgs e = null)
        {
            saveMapLists();
        }

        private static Image CreateThumbnailOverlay(string title, Font font, string mapname = null, int order = -1, Color? _textColor = null, Color? _backColor = null, int height = 256, int width = 256, Image baseImage = null)
        {
            var textBrush = new SolidBrush(_textColor ?? Color.Orange);

            Bitmap img = new Bitmap(width, height);
            Graphics drawing = Graphics.FromImage(img);
            if (baseImage != null)
            {
                img = baseImage.Resize(width, height);
                drawing = Graphics.FromImage(img);
            }
            else drawing.Clear(_backColor ?? Color.Transparent);
            // Pen textPen = new Pen(backColor); // lol
            var i = 1;
            foreach (var _title in title.Split("\\n"))
            {
                drawing.DrawString(_title, font, textBrush, x: width / 2, y: font.Height * i++, format: new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
            font = new Font(font.FontFamily, 15, font.Style);
            // var path = new GraphicsPath();
            if (!mapname.IsNullOrWhiteSpace()) drawing.DrawString(mapname, font, textBrush, x: 10, y: height - 40);

            var orderBrush = new SolidBrush(Color.FromArgb(230, Color.Black));
            if (order > 0)
            {
                if (img.GetPixel(width - 30, height - 30).GetBrightness() < 0.4) orderBrush = new SolidBrush(orderBrush.Color.Invert());
                var point = new PointF(x: width + 5, y: height + 5);
                drawing.DrawString(order.ToString(), new Font(font.FontFamily, 40, font.Style), orderBrush, x: point.X, y: point.Y, format: new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far });
                // path.AddString(s: order.ToString(), family: font.FontFamily, style: (int)font.Style, 50, origin: point, format: new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far });
                // drawing.DrawPath(textPen, path);
            }
            orderBrush.Dispose();
            textBrush.Dispose();
            font.Dispose();
            drawing.Save();
            drawing.Dispose();
            return img;
        }

        internal class ImageFile
        {
            public string Name { get; set; }
            public string Path { get; set; }

            public ImageFile(FileInfo file)
            {
                Path = file.FullName;
                Name = file.FileNameWithoutExtension().ToLower();
            }
        }

        private void btn_baseimg_Click(object sender, EventArgs e)
        {
            var folder = Utils.pickFolder("Select folder that contains base images");
            if (folder is null || !folder.Exists) return;
            baseImages = folder.GetFiles("*.*").Select(f => new ImageFile(f)).ToList();
        }

        private void txt_name_Enter(object sender, EventArgs e)
        {
            new ToolTip().Show("Use \\n to force a linebreak in generated thumbnails", (TextBox)sender, 0, 20, 5000);
        }
    }
}