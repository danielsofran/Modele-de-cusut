using Modele_de_cusut.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modele_de_cusut.Enums;

namespace Modele_de_cusut.Costum_Controls
{
    public partial class CreateProiect : Form
    {
        public Proiect proiect;

        public CreateProiect()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /// test
            if (textBox1.Text == "") { label5.Visible = true; }
            this.Refresh();

            if(label5.Visible == false)
            {
                // creating proiect
                proiect = new Proiect();
                proiect.Nume = textBox1.Text;
                proiect.Path = Manager.BaseDirectoryPath + "\\" + proiect.Nume;
                proiect.LastAccess = DateTime.Now;
                // save
                proiect.Save();
                // close form
                this.DialogResult = DialogResult.OK;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label5.Visible = false;
        }
    }
}
