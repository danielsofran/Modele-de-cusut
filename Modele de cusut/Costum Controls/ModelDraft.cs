using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modele_de_cusut.Editor;
using Modele_de_cusut.Classes;

namespace Modele_de_cusut.Costum_Controls
{
    public partial class ModelDraft : UserControl
    {
        Graphics g;
        public ModelInfo info { get; set; } = new ModelInfo();
        Dictionary<Point, Color> source = new Dictionary<Point, Color>();
        Dictionary<Color, Color> newcolors = new Dictionary<Color, Color>();
        public Dictionary<Point, Color> Source
        {
            get => source;
            set
            {
                source = new Dictionary<Point, Color>(value);
                //info = (this.ParentForm as ChangeColors).ParentForm as ModelEditor).ModelInfo;
                Tuple<Point, Point> size = ModelX.MinMax(source);
                this.Width = (size.Item2.X - size.Item1.X + 3) * info.Patrat+2;
                this.Height = (size.Item2.Y - size.Item1.Y + 3) * info.Patrat+2;
                g = this.CreateGraphics();
                if (g == null) return;
                DrawFirst(g);
                Draw(g);
            }
        }
        public Dictionary<Color, Color> NewColors
        {
            get
            {
                //Draw(g);
                return newcolors;
            }
            set
            {
                newcolors = value;
                Draw(g);
            }
        }
        public Dictionary<Point, Color> Data {
            get
            {
                Dictionary<Point, Color> rez = new Dictionary<Point, Color>();
                foreach (KeyValuePair<Point, Color> x in Source)
                {
                    if (NewColors.ContainsKey(x.Value))
                        rez[x.Key] = NewColors[x.Value];
                    else rez[x.Key] = x.Value;
                }
                return rez;
            }
        }

        public ModelDraft()
        {
            InitializeComponent();
        }

        void DrawFirst(Graphics g)
        {
            Pen pen = new Pen(info.BackgroundColor, 1);
            // linii orizontale
            int l = 1;
            while (l < this.Height)
            {
                g.DrawLine(pen, 0, l, this.Size.Width, l);
                l += info.Patrat;
            }
            // linii verticale
            l = 1;
            while (l < this.Width)
            {
                g.DrawLine(pen, l, 0, l, this.Size.Height);
                l += info.Patrat;
            }
        }

        void DrawX(Graphics graphics, Point point0, Color color)
        {
            Pen pen = new Pen(color, info.Weight);
            point0.X++;
            point0.Y++;
            point0.X *= info.Patrat;
            point0.Y *= info.Patrat;
            PointF point = new PointF(point0.X, point0.Y);
            float margin = 0;// 1 + 0.35f * ModelInfo.Weight;
            graphics.DrawLine(pen, point.X + margin, point.Y + margin, point.X + info.Patrat - margin, point.Y + info.Patrat - margin);
            graphics.DrawLine(pen, point.X + margin, point.Y + info.Patrat - margin, point.X + info.Patrat - margin, point.Y + margin);
        }

        void Draw(Graphics graphics)
        {
            foreach (KeyValuePair<Point, Color> x in Source)
            {
                if(NewColors.ContainsKey(x.Value))
                    DrawX(graphics, x.Key, newcolors[x.Value]);
                else DrawX(graphics, x.Key, x.Value);
            }
        }

        private void ModelDraft_Paint(object sender, PaintEventArgs e)
        {
            DrawFirst(e.Graphics);
            Draw(e.Graphics);
        }

        private void ModelDraft_Load(object sender, EventArgs e)
        {
            g = this.CreateGraphics();
        }
    }
}
