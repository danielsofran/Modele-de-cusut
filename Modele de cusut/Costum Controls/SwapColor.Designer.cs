namespace Modele_de_cusut.Costum_Controls
{
    partial class SwapColor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copiazăToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lipeșteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.schimbăToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(15, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(66, 66);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copiazăToolStripMenuItem,
            this.lipeșteToolStripMenuItem,
            this.schimbăToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(121, 70);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // copiazăToolStripMenuItem
            // 
            this.copiazăToolStripMenuItem.Image = global::Modele_de_cusut.Properties.Resources.copy;
            this.copiazăToolStripMenuItem.Name = "copiazăToolStripMenuItem";
            this.copiazăToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.copiazăToolStripMenuItem.Text = "Copiază";
            this.copiazăToolStripMenuItem.Click += new System.EventHandler(this.copiazăToolStripMenuItem_Click);
            // 
            // lipeșteToolStripMenuItem
            // 
            this.lipeșteToolStripMenuItem.Image = global::Modele_de_cusut.Properties.Resources.paste;
            this.lipeșteToolStripMenuItem.Name = "lipeșteToolStripMenuItem";
            this.lipeșteToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.lipeșteToolStripMenuItem.Text = "Lipește";
            this.lipeșteToolStripMenuItem.Click += new System.EventHandler(this.lipeșteToolStripMenuItem_Click);
            // 
            // schimbăToolStripMenuItem
            // 
            this.schimbăToolStripMenuItem.Image = global::Modele_de_cusut.Properties.Resources.color_picker1;
            this.schimbăToolStripMenuItem.Name = "schimbăToolStripMenuItem";
            this.schimbăToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.schimbăToolStripMenuItem.Text = "Schimbă";
            this.schimbăToolStripMenuItem.Click += new System.EventHandler(this.schimbăToolStripMenuItem_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBox2.Location = new System.Drawing.Point(165, 15);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(66, 66);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.BackColorChanged += new System.EventHandler(this.pictureBox2_BackColorChanged);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackgroundImage = global::Modele_de_cusut.Properties.Resources.ArrowRight;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox3.Location = new System.Drawing.Point(90, 13);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(66, 69);
            this.pictureBox3.TabIndex = 2;
            this.pictureBox3.TabStop = false;
            // 
            // SwapColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Name = "SwapColor";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.Size = new System.Drawing.Size(246, 96);
            this.Resize += new System.EventHandler(this.SwpaColor_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copiazăToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lipeșteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem schimbăToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}
