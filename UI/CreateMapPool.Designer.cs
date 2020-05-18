namespace GModMountManager.UI
{
    partial class CreateMapPool
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lst_maps = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.txt_dev = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_url = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_create = new System.Windows.Forms.Button();
            this.btn_upload = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_create_addon = new System.Windows.Forms.Button();
            this.txt_shortname = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_description = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btn_baseimg = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_longname = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.lst_maps)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lst_maps
            // 
            this.lst_maps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lst_maps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lst_maps.Location = new System.Drawing.Point(0, 0);
            this.lst_maps.Name = "lst_maps";
            this.lst_maps.Size = new System.Drawing.Size(304, 488);
            this.lst_maps.TabIndex = 0;
            this.lst_maps.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.lst_maps_CellContextMenuStripNeeded);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Game Name:";
            // 
            // txt_name
            // 
            this.txt_name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_name.Location = new System.Drawing.Point(86, 37);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(242, 20);
            this.txt_name.TabIndex = 2;
            this.txt_name.TextChanged += new System.EventHandler(this.txt_TextChanged);
            this.txt_name.Enter += new System.EventHandler(this.txt_name_Enter);
            // 
            // txt_dev
            // 
            this.txt_dev.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_dev.Location = new System.Drawing.Point(86, 89);
            this.txt_dev.Name = "txt_dev";
            this.txt_dev.Size = new System.Drawing.Size(242, 20);
            this.txt_dev.TabIndex = 4;
            this.txt_dev.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Developer:";
            // 
            // txt_url
            // 
            this.txt_url.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_url.Location = new System.Drawing.Point(86, 115);
            this.txt_url.Name = "txt_url";
            this.txt_url.Size = new System.Drawing.Size(242, 20);
            this.txt_url.TabIndex = 6;
            this.txt_url.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "URL:";
            // 
            // btn_create
            // 
            this.btn_create.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_create.Enabled = false;
            this.btn_create.Location = new System.Drawing.Point(134, 453);
            this.btn_create.Name = "btn_create";
            this.btn_create.Size = new System.Drawing.Size(74, 23);
            this.btn_create.TabIndex = 7;
            this.btn_create.Text = "Create GMA";
            this.btn_create.UseVisualStyleBackColor = true;
            this.btn_create.Click += new System.EventHandler(this.btn_create_Click);
            // 
            // btn_upload
            // 
            this.btn_upload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_upload.Enabled = false;
            this.btn_upload.Location = new System.Drawing.Point(12, 453);
            this.btn_upload.Name = "btn_upload";
            this.btn_upload.Size = new System.Drawing.Size(56, 23);
            this.btn_upload.TabIndex = 8;
            this.btn_upload.Text = "Upload";
            this.btn_upload.UseVisualStyleBackColor = true;
            this.btn_upload.Click += new System.EventHandler(this.btn_upload_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(143, 70);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // btn_create_addon
            // 
            this.btn_create_addon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_create_addon.Location = new System.Drawing.Point(214, 453);
            this.btn_create_addon.Name = "btn_create_addon";
            this.btn_create_addon.Size = new System.Drawing.Size(80, 23);
            this.btn_create_addon.TabIndex = 9;
            this.btn_create_addon.Text = "Create Addon";
            this.btn_create_addon.UseVisualStyleBackColor = true;
            this.btn_create_addon.Click += new System.EventHandler(this.btn_create_addon_Click);
            // 
            // txt_shortname
            // 
            this.txt_shortname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_shortname.Location = new System.Drawing.Point(86, 12);
            this.txt_shortname.Name = "txt_shortname";
            this.txt_shortname.Size = new System.Drawing.Size(242, 20);
            this.txt_shortname.TabIndex = 10;
            this.txt_shortname.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Addon ID:";
            // 
            // txt_description
            // 
            this.txt_description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_description.Location = new System.Drawing.Point(22, 142);
            this.txt_description.Multiline = true;
            this.txt_description.Name = "txt_description";
            this.txt_description.Size = new System.Drawing.Size(306, 299);
            this.txt_description.TabIndex = 12;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btn_baseimg);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.txt_longname);
            this.splitContainer1.Panel1.Controls.Add(this.txt_description);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btn_create_addon);
            this.splitContainer1.Panel1.Controls.Add(this.btn_upload);
            this.splitContainer1.Panel1.Controls.Add(this.btn_create);
            this.splitContainer1.Panel1.Controls.Add(this.txt_name);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.txt_dev);
            this.splitContainer1.Panel1.Controls.Add(this.txt_shortname);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.txt_url);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lst_maps);
            this.splitContainer1.Size = new System.Drawing.Size(646, 488);
            this.splitContainer1.SplitterDistance = 338;
            this.splitContainer1.TabIndex = 14;
            // 
            // btn_baseimg
            // 
            this.btn_baseimg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_baseimg.Location = new System.Drawing.Point(300, 453);
            this.btn_baseimg.Name = "btn_baseimg";
            this.btn_baseimg.Size = new System.Drawing.Size(28, 23);
            this.btn_baseimg.TabIndex = 15;
            this.btn_baseimg.Text = "...";
            this.btn_baseimg.UseVisualStyleBackColor = true;
            this.btn_baseimg.Click += new System.EventHandler(this.btn_baseimg_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Addon Name:";
            // 
            // txt_longname
            // 
            this.txt_longname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_longname.Location = new System.Drawing.Point(86, 63);
            this.txt_longname.Name = "txt_longname";
            this.txt_longname.Size = new System.Drawing.Size(242, 20);
            this.txt_longname.TabIndex = 14;
            this.txt_longname.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // CreateMapPool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 488);
            this.Controls.Add(this.splitContainer1);
            this.Name = "CreateMapPool";
            this.Text = "CreateMapPool";
            this.Load += new System.EventHandler(this.CreateMapPool_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lst_maps)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView lst_maps;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.TextBox txt_dev;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_url;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_create;
        private System.Windows.Forms.Button btn_upload;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.Button btn_create_addon;
        private System.Windows.Forms.TextBox txt_shortname;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_description;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_longname;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Button btn_baseimg;
    }
}