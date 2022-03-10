using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modele_de_cusut.Classes;
using Modele_de_cusut.Editor;

namespace Modele_de_cusut.Costum_Controls
{
    public partial class ChangeColors : Form
    {
        public Color ColorCache { get; set; }
        public ModelInfo Info { get => modelDraft1.info; set => modelDraft1.info = value; }
        public Dictionary<Point, Color> Data { get; set; }
        public Dictionary<Point, Color> Xuri
        {
            get => modelDraft1.Source;
            set
            {
                //MessageBox.Show((value == null).ToString());
                modelDraft1.Source = value;
                foreach(KeyValuePair<Point, Color> pair in modelDraft1.Source)
                {
                    if (!modelDraft1.NewColors.ContainsKey(pair.Value))
                    {
                        //MessageBox.Show("add " + pair.Value.ToString());
                        SwapColor swap = new SwapColor();
                        swap.form = this;
                        swap.OldColor = pair.Value;
                        swap.NewColor = pair.Value;
                        flowLayoutPanel1.Controls.Add(swap);
                        //flowLayoutPanel1.Height += flowLayoutPanel1.Padding.Top + flowLayoutPanel1.Padding.Bottom + swap.Height;
                        modelDraft1.NewColors.Add(pair.Value, pair.Value);
                    }
                }
            }
        }

        public ChangeColors()
        {
            this.Icon = Icon.FromHandle(Properties.Resources.color_picker1.GetHicon());
            InitializeComponent();
            panel1.Width = this.Width - flowLayoutPanel1.Width - 50;
            modelDraft1.Center(Program.Align.Both);
            //MessageBox.Show(this.ParentForm.ToString());
            //modelDraft1.info = (this.ParentForm as ModelEditor).ModelInfo;
        }

        private void ChangeColors_Resize(object sender, EventArgs e)
        {
            panel1.Width = this.Width - flowLayoutPanel1.Width - 50;
            modelDraft1.Center(Program.Align.Both);
        }

        private void deseneazaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modelDraft1.Invalidate();
        }

        private void aplicaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modelDraft1.NewColors.Clear();
            foreach(SwapColor c in flowLayoutPanel1.Controls)
            {
                if(c.Modified)
                {
                    modelDraft1.NewColors[c.OldColor] = c.NewColor;
                }
            }
            modelDraft1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            aplicaToolStripMenuItem_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            modelDraft1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Data = new Dictionary<Point, Color>(modelDraft1.Data);
        }

        private void ChangeColors_Load(object sender, EventArgs e)
        {
            modelDraft1.Center(Program.Align.Both);
        }
    }
}
