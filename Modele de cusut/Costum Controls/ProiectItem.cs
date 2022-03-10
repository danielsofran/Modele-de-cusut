using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modele_de_cusut.Classes;
using System.Windows.Markup;
using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;
using System.Windows.Forms.VisualStyles;
using Modele_de_cusut.Editor;
using System.Drawing.Printing;
using System.Windows.Media;

namespace Modele_de_cusut.Costum_Controls
{
    public partial class ProiectItem : UserControl
    {
        public Proiect _proiect = new Proiect();
        public Proiect Proiect
        {
            get => _proiect; 
            set{
                _proiect = value;
                if (_proiect == null) { }
                else 
                {
                    label1.Text = Proiect.Nume;
                    label2.Text = Proiect.NumeCamasa;
                    label3.Text = ro[Proiect.LastAccess.DayOfWeek.ToString()] + ", " + data(Proiect.LastAccess.Hour) + ":" + data(Proiect.LastAccess.Minute);
                    label4.Text = data(Proiect.LastAccess.Day) + "." + data(Proiect.LastAccess.Month) + "." + Proiect.LastAccess.Year;
                    try { pictureBox1.BackgroundImage = Image.FromFile(Proiect.ImagePath); }
                    catch (Exception ex) {
                        Bitmap b = new Bitmap(100, 100);
                        for (int i = 0; i < 100; ++i)
                            for (int j = 0; j < 100; ++j)
                                b.SetPixel(i, j, System.Drawing.Color.White);
                        pictureBox1.BackgroundImage = b;
                    }
                }
            }
        }
        public int TransitionSpeed { get; set; } = 3;
        //public bool IsDisposed { get; set; } = false;

        Dictionary<string, string> ro = new Dictionary<string, string>();
        public ProiectItem()
        {
            /// load dictionary
            ro["Monday"] = "Luni"; ro["Tuesday"] = "Marți"; ro["Wednesday"] = "Miercuri"; ro["Thursday"] = "Joi"; ro["Friday"] = "Vineri"; ro["Saturday"] = "Sâmbătă"; ro["Sunday"] = "Duminică";
            InitializeComponent();
            //label3.BackColor = label4.BackColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
        }

        private void removeFromListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = System.Drawing.Color.FromArgb(255, SystemColors.ActiveCaption);
            while (this.Size.Height > 0)
            {
                this.Size = new Size(this.Size.Width, this.Size.Height - this.TransitionSpeed);
                Thread.Sleep(1);
            }
            this.Proiect.isHiddenFromList = true;
            Proiect.ToFile();
            this.Dispose();
        }

        private void ProiectItem_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = System.Drawing.Color.FromArgb(255, SystemColors.ActiveCaption);
            this.Focus();
        }

        public void ProiectItem_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.Control;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PrewiewImage prewiewImage = new PrewiewImage(pictureBox1.BackgroundImage);
            prewiewImage.Enabled = true;
            prewiewImage.ShowDialog();
        }

        private void ProiectItem_Click(object sender, EventArgs e)
        {
            /// here we load the editor
            ModelEditor modelEditor = new ModelEditor(Proiect);
            modelEditor.Text = Proiect.Nume;
            Manager m = this.ParentForm as Manager;
            modelEditor.ManagerParent = m;
            modelEditor.Size = new Size(1900, 900);
            modelEditor.Show();
            m.Hide();
            m.ShowInTaskbar = false;
        }

        private void editeazăToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditProiect form = new EditProiect(this.Proiect);
            if (form.ShowDialog() == DialogResult.OK)
            {
                ToDoList.Add("DELETE " + Proiect.ImagePath);
                Proiect = form.ProiectFinal;
            }
        }

        public void Hide()
        {
            this.Size = new Size(this.Size.Width, 0);
        }

        public void Unhide()
        {
            this.Size = new Size(this.Size.Width, 94); // the default value
        }

        public string data(int value)
        {
            if (value < 10) return "0" + value;
            else return value.ToString();
        }

        private void printeazăModelulToolStripMenuItem_Click(object sender0, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            if(pictureBox1.BackgroundImage.Width > pictureBox1.BackgroundImage.Height) 
                pd.DefaultPageSettings.Landscape = true;
            else pd.DefaultPageSettings.Landscape = false;
            IEnumerable<PaperSize> paperSizes = new PrinterSettings().PaperSizes.Cast<PaperSize>();
            PaperSize sizeA4 = paperSizes.First<PaperSize>(size => size.Kind == PaperKind.A4); // setting paper size to A4 size
            pd.DefaultPageSettings.PaperSize = sizeA4;
            pd.PrintPage += (sender, args) =>
            {
                RectangleF rectangle = new RectangleF(0,0,0,0);
                Font font = new Font(System.Drawing.FontFamily.GenericSansSerif, 20);
                int dW = Math.Abs(args.PageBounds.Width - pictureBox1.BackgroundImage.Width);
                int dH = Math.Abs(args.PageBounds.Height - pictureBox1.BackgroundImage.Height);
                if (dW > dH)
                {
                    float multiplayer = (float)args.PageBounds.Height / (float)pictureBox1.BackgroundImage.Height;
                    rectangle.Size = new SizeF(multiplayer * pictureBox1.BackgroundImage.Width, args.PageBounds.Height);
                    rectangle.X = (args.PageBounds.Width - multiplayer * pictureBox1.BackgroundImage.Width) / 2;
                    args.Graphics.DrawString(this.Proiect.NumeCamasa, font, System.Drawing.Brushes.Black, rectangle.X - 50, args.PageBounds.Height/2 - 30, new StringFormat(StringFormatFlags.DirectionVertical));
                }
                else
                {
                    float max = Math.Max(args.PageBounds.Width, pictureBox1.BackgroundImage.Width);
                    float min = Math.Min(args.PageBounds.Width, pictureBox1.BackgroundImage.Width);
                    float multiplayer = args.PageBounds.Width / pictureBox1.BackgroundImage.Width;
                    rectangle.Size = new SizeF(args.PageBounds.Width, multiplayer * pictureBox1.BackgroundImage.Height);
                    //rectangle.Y = (args.PageBounds.Height - multiplayer * pictureBox1.BackgroundImage.Height) / 2;
                    //args.Graphics.DrawString(this.Proiect.NumeCamasa, font, System.Drawing.Brushes.Black, args.PageBounds.Width / 2 - 50, args.PageBounds.Height - 100);
                }
                args.Graphics.DrawImage(pictureBox1.BackgroundImage, rectangle);
            };
            ((Form)printPreviewDialog1).WindowState = FormWindowState.Maximized;
            printPreviewDialog1.Document = pd;
            if(printPreviewDialog1.ShowDialog()==DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void veziÎnFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Proiect.Path+"\\Proiect\\Imagini");
        }
    }
}
