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
using Modele_de_cusut;
using System.Drawing.Imaging;

namespace Modele_de_cusut.Costum_Controls
{
    public partial class EditProiect : Form
    {
        public Proiect ProiectInitial;
        public Proiect ProiectFinal = new Proiect();
        public EditProiect(Proiect pr)
        {
            this.ProiectInitial = pr;
            InitializeComponent();
            ProiectToForm(pr);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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
                FormToProiect();
                //MessageBox.Show(ProiectFinal.Path);
                if(ProiectInitial.Nume != ProiectFinal.Nume)
                {// scimbam doar numele
                    // folder-ul cu fisierul .proiect
                    Foldere.Rename(ProiectInitial.Path, ProiectFinal.Nume);
                    // imaginea de previzualizare
                    try
                    {
                        File.Copy(ProiectInitial.ImagePath, Path.GetDirectoryName(ProiectInitial.ImagePath) + "\\" + ProiectFinal.Nume + Path.GetExtension(ProiectInitial.ImagePath));
                    }
                    catch (Exception ex) { }// alreay exusts
                }
                ProiectFinal.LastAccess = DateTime.Now;
                this.DialogResult = DialogResult.OK;
                this.Dispose();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label5.Visible = false;
        }

        void FormToProiect()
        {
            try
            {
                ProiectFinal.Nume = textBox1.Text;
                ProiectFinal.Path = Directory.GetParent(ProiectInitial.Path).FullName + "\\" + ProiectFinal.Nume;
                if (pictureBox1.Tag != null) ProiectFinal.ImagePath = pictureBox1.Tag.ToString();
            }
            catch(Exception ex) { ProiectFinal = ProiectInitial; MessageBox.Show(ex.Message); }
        }

        void ProiectToForm(Proiect pr)
        {
            textBox1.Text = pr.Nume;
            pictureBox1.BackgroundImage = Image.FromFile(pr.ImagePath);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PrewiewImage prewiewImage = new PrewiewImage(pictureBox1.BackgroundImage);
            prewiewImage.Show();
        }

        /*private void schimbăImagineaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = ProiectInitial.Path;
                var codecs = ImageCodecInfo.GetImageEncoders();
                var codecFilter = "Image Files|";
                foreach (var codec in codecs)
                {
                    codecFilter += codec.FilenameExtension + ";";
                }
                openFileDialog.Filter = codecFilter;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string imagePath = openFileDialog.FileName;
                    pictureBox1.Tag = imagePath;
                    pictureBox1.BackgroundImage = Image.FromFile(imagePath);
                }
            }
        }*/
    }
}
