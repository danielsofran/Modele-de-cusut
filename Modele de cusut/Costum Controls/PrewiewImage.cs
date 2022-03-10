using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modele_de_cusut.Costum_Controls
{
    public class PrewiewImage : Form
    {
        private ContextMenuStrip contextMenuStrip1;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem printeazăToolStripMenuItem;

        public PrewiewImage(Image img)
        {
            InitializeComponent();
            this.BackgroundImage = img;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrewiewImage));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.printeazăToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printeazăToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(123, 26);
            // 
            // printeazăToolStripMenuItem
            // 
            this.printeazăToolStripMenuItem.Image = global::Modele_de_cusut.Properties.Resources.print;
            this.printeazăToolStripMenuItem.Name = "printeazăToolStripMenuItem";
            this.printeazăToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.printeazăToolStripMenuItem.Text = "Printează";
            this.printeazăToolStripMenuItem.Click += new System.EventHandler(this.printeazăToolStripMenuItem_Click);
            // 
            // PrewiewImage
            // 
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(323, 285);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PrewiewImage";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Resize += new System.EventHandler(this.PrewiewImage_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void printeazăToolStripMenuItem_Click(object sender0, EventArgs e)
        {
            
        }

        private void PrewiewImage_Resize(object sender, EventArgs e)
        {

        }
    }
}
