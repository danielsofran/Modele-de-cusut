using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modele_de_cusut.Editor.Costum_Controls
{
    public partial class ColorBox : UserControl
    {
        Color color;
        public Color Color 
        { 
            get => color;
            set
            {
                color = value;
                panel1.BackColor = value;
            }
        }
        public ColorBox()
        {
            InitializeComponent();
            panel1.Size = this.Size;
        }

        private void ColorBox_Resize(object sender, EventArgs e)
        {
            panel1.Size = this.Size;
        }

        private void schimbăToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color = colorDialog1.Color;
            }
        }
    }
}
