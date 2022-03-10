using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Modele_de_cusut.Program;

namespace Modele_de_cusut.Costum_Controls
{
    public partial class SwapColor : UserControl
    {
        public Color OldColor { get => pictureBox1.BackColor; set => pictureBox1.BackColor = value; }
        public Color NewColor { get => pictureBox2.BackColor; set => pictureBox2.BackColor = value; }
        public bool Modified { get; set; } = false;
        public ChangeColors form { get; set; }

        public SwapColor()
        {
            InitializeComponent();
            //form = this.ParentForm as ChangeColors;
        }

        private void SwpaColor_Resize(object sender, EventArgs e)
        {
            pictureBox3.Center(Align.Left);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(((sender as ContextMenuStrip).SourceControl as PictureBox).Name.EndsWith("1"))
                lipeșteToolStripMenuItem.Enabled = schimbăToolStripMenuItem.Enabled = false;
            else lipeșteToolStripMenuItem.Enabled = schimbăToolStripMenuItem.Enabled = true;
        }

        private void copiazăToolStripMenuItem_Click(object sender, EventArgs e)
        {

            PictureBox box = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as PictureBox;
            form.ColorCache = box.BackColor;
        }

        private void lipeșteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox box = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as PictureBox;
            box.BackColor = form.ColorCache;
        }

        private void schimbăToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox box = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as PictureBox;
            colorDialog1.Color = box.BackColor;
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                box.BackColor = colorDialog1.Color;
            }
        }

        private void pictureBox2_BackColorChanged(object sender, EventArgs e)
        {
            //PictureBox box = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as PictureBox;
            // apply changes
            //form.modelDraft1.NewColors[OldColor] = NewColor;
            Modified = true;
        }
    }
}
