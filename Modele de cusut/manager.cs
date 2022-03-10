using Modele_de_cusut.Classes;
using Modele_de_cusut.Costum_Controls;
using Modele_de_cusut.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modele_de_cusut
{
    public partial class Manager : Form
    {
        public static string BaseDirectoryPath;

        public Manager()
        {
            InitializeComponent();
            SetSortParameters();
        }

        private void Manager_Load(object sender, EventArgs e)
        {
            AutoCompleteStringCollection source = new AutoCompleteStringCollection();
            List<Proiect> proiecte = Proiect.FromBaseDirectory(BaseDirectoryPath);
            
            foreach (Proiect proiect in proiecte)
            {
                ProiectItem proiectItem = new ProiectItem();
                proiectItem.Proiect = proiect;
                if (!proiect.isHiddenFromList)
                {
                    flowLayoutPanel1.Controls.Add(proiectItem);
                    source.Add(proiect.Nume);
                }
            }
            textBox1.AutoCompleteCustomSource = source;
            comboBox1_SelectedIndexChanged(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateProiect form = new CreateProiect();
            if (form.ShowDialog() == DialogResult.OK)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                Thread.Sleep(500);
                ModelEditor modelEditor = new ModelEditor(form.proiect);
                modelEditor.Icon = this.Icon;
                modelEditor.ManagerParent = this;
                modelEditor.Show();
            }
        }

        private void Manager_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach(ProiectItem proiectItem in flowLayoutPanel1.Controls)
            {
                proiectItem.Proiect.Save();
                //proiectItem.pictureBox1.BackgroundImage.Dispose();
            }
            using (StreamWriter fout = new StreamWriter(BaseDirectoryPath + "\\_System\\sortparameters.txt"))
            {
                fout.WriteLine(comboBox1.Text);
                fout.WriteLine(comboBox2.Text);
            }
            StreamWriter fout2 = new StreamWriter(Environment.GetEnvironmentVariable("ProgramData") + "\\Model Manager\\BaseDirectoryPath.txt");
            fout2.WriteLine(BaseDirectoryPath);
            fout2.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            foreach (ProiectItem item in flowLayoutPanel1.Controls)
            {
                if (item.Proiect.Nume.StartsWith(textBox1.Text)) item.Unhide();
                else item.Hide();
            }
        }

        private void SetSortParameters()
        {
            using (StreamReader fin = new StreamReader(BaseDirectoryPath + "\\_System\\sortparameters.txt"))
            {
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(fin.ReadLine());
                comboBox2.SelectedIndex = comboBox2.Items.IndexOf(fin.ReadLine());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // sortam itemele
            List<Proiect> proiects = new List<Proiect>();
            foreach (ProiectItem item in flowLayoutPanel1.Controls)
            {
                proiects.Add(item.Proiect);
                //flowLayoutPanel1.Controls.Remove(item);
            }
            flowLayoutPanel1.Controls.Clear();
            if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 0)
            {
                proiects.Sort(CompareProiects.CrescData);
            }
            else if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 1)
            {
                proiects.Sort(CompareProiects.DescrescData);
            }
            else if (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 0)
            {
                proiects.Sort(CompareProiects.CrescNume);
            }
            else if (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 1)
            {
                proiects.Sort(CompareProiects.DescrescNume);
            }
            //else MessageBox.Show("eroare");

            foreach (Proiect proiect in proiects)
            {
                ProiectItem proiectItem = new ProiectItem();
                proiectItem.Proiect = proiect;
                if (!proiect.isHiddenFromList)
                    flowLayoutPanel1.Controls.Add(proiectItem);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = BaseDirectoryPath;
            openFileDialog1.Filter = "Proiect files (*.proiect)|*.proiect|All files (*.*)|*.*";
            openFileDialog1.FileName = "";

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Proiect proiect = new Proiect();
                proiect.FromFile(openFileDialog1.FileName);
                proiect.isHiddenFromList = false;
                ProiectItem item = new ProiectItem();
                item.Proiect = proiect;
                flowLayoutPanel1.Controls.Add(item);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Foldere.DirectoryCopy(BaseDirectoryPath, folderBrowserDialog1.SelectedPath, true);
                ToDoList.Add("DELETE_DIR " + BaseDirectoryPath);
                BaseDirectoryPath = folderBrowserDialog1.SelectedPath;
            }    
        }

        private void Manager_Resize(object sender, EventArgs e)
        {
            flowLayoutPanel1.Height = this.Height - 50 - flowLayoutPanel1.Top;
            flowLayoutPanel1.Width = button1.Left - 100 - flowLayoutPanel1.Left;
            foreach(ProiectItem p in flowLayoutPanel1.Controls)
            {
                p.Width = flowLayoutPanel1.Width - 20;
            }
        }

        private void Manager_MouseLeave(object sender, EventArgs e)
        {
            foreach(ProiectItem p in flowLayoutPanel1.Controls)
                p.ProiectItem_MouseLeave(sender, e);
        }

        private void flowLayoutPanel1_MouseLeave(object sender, EventArgs e)
        {
            foreach(Control c in flowLayoutPanel1.Controls)
            {
                if(c is ProiectItem p)
                {
                    p.ProiectItem_MouseLeave(this, e);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog()==DialogResult.OK)
            {
                ZipFile.CreateFromDirectory(BaseDirectoryPath, saveFileDialog1.FileName);
            }
        }
    }
}
