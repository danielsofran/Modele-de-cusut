using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Microsoft.VisualBasic.CompilerServices;
using Modele_de_cusut.Classes;
using Modele_de_cusut.Costum_Controls;
using Modele_de_cusut.Editor.Costum_Controls;
using Modele_de_cusut.Properties;
using X = System.Collections.Generic.KeyValuePair<System.Drawing.Point, System.Drawing.Color>;


namespace Modele_de_cusut.Editor
{
    public partial class ModelEditor : Form
    {
        #region globals
        public Proiect Proiect { get; set; }
        public Manager ManagerParent { get; set; }
        public ModelInfo ModelInfo { get; set; } = new ModelInfo();
        public Dictionary<Point, Color> Xuri = new Dictionary<Point, Color>();
        public static Dictionary<Point, Color> XuriCache { get; set; } = new Dictionary<Point, Color>();
        private Dictionary<Point, Color> LastShape = new Dictionary<Point, Color>();
        Point cursorpoint = default, pmin = new Point(), pstartshape = default, _pendshape = default;
        bool pdrawing = false, copycolor=false;
        PictureBox oldcolor;
        Size msize = new Size();
        Graphics g; 
        SelectionTool SelectPanel = null; Panel SelectedRectangle = null; Point selectedPoint = new Point();

        public ModelEditor(Proiect proiect)
        {
            Proiect = proiect;
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0; 
            //typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panelDraw, new object[] { true });
            g = panelDraw.CreateGraphics();
            Init_dir();
            History.CanUndo_Changed += UndoChanged;
            History.CanRedo_Changed += RedoChanged;

        }

        public static List<Point> directii = new List<Point>(), dir4 = new List<Point>(), dir8 = new List<Point>();
        void Init_dir()
        {
            dir4.Add(new Point(0, 1));
            dir4.Add(new Point(1, 0));
            dir4.Add(new Point(0, -1));
            dir4.Add(new Point(-1, 0));
            dir8.AddRange(dir4);
            dir8.Add(new Point(1, 1));
            dir8.Add(new Point(-1, 1));
            dir8.Add(new Point(1, -1));
            dir8.Add(new Point(-1, -1));
            directii = dir4;
        }
        #endregion

        #region ModelEditor
        private void ModelEditor_Shown(object sender, EventArgs e)
        {
            for (int i = 1; i <= 24; ++i)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.BackColor = ModelInfo.Colors.ElementAt(i-1);
                pictureBox.Size = new Size(30, 30);
                pictureBox.Margin = new Padding(2);
                pictureBox.BorderStyle = BorderStyle.FixedSingle;
                pictureBox.Click += new EventHandler(picturebox_Click);
                pictureBox.DoubleClick += picturebox_DoubleClick;
                pictureBox.ContextMenuStrip = contextMenuStripColor;
                pictureBox.Tag = i; flowLayoutPanel1.Controls.Add(pictureBox);
            }
            mainColorBox.BackColor = ModelInfo.MainColor;
            ActiveControl = null;
        }
        private void ModelEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Bitmap img = SaveImg();
            if (MessageBox.Show("Salvați proiectul înainte de a ieși?", "Salvare", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                List<Color> Colors = new List<Color>();
                foreach (PictureBox box in flowLayoutPanel1.Controls)
                    Colors.Add(box.BackColor);
                ModelInfo.Colors = Colors;
                ModelInfo.MainColor = mainColorBox.BackColor;
                string path = Proiect.Path + "\\Proiect\\Model\\";
                ModelInfo.ToFile(path + "model.info");
                ModelX.Save(Xuri, path + "model.xuri");
                History.ToFile(path);
                img.Save(Proiect.Path + "\\Proiect\\Imagini\\" + Proiect.Nume + ".png", ImageFormat.Png);
                ToDoList.Add("COPY*" + Proiect.Path + "\\Proiect\\Imagini\\" + Proiect.Nume + ".png" + "*" + Directory.GetParent(Proiect.Path).FullName + "\\_Imagini\\" + Proiect.Nume + ".png");
                this.Proiect.NumeCamasa = (msize.Width/ModelInfo.Patrat-2) + " x " + (msize.Height/ModelInfo.Patrat-2) + " x-uri";
                this.Proiect.LastAccess = DateTime.Now;
                Proiect.ToFile();
            }
            this.DialogResult = DialogResult.Cancel;
            ManagerParent.Close();
        }
        private void ModelEditor_Load(object sender, EventArgs e)
        {
            string path = Proiect.Path + "\\Proiect\\Model\\";
            ModelInfo.FromFile(path + "model.info");
            ModelX.Load(out Xuri, path + "model.xuri");
            mainColorBox.BackColor = ModelInfo.MainColor;
            // info
            Tuple<Point, Point> p = ModelX.MinMax(Xuri);
            int xmin = p.Item1.X, xmax =p.Item2.X, ymin = p.Item1.Y, ymax = p.Item2.Y;
            if (xmin != 10000 && ymin != 10000)
            {
                labelWidth.Text = (xmax - xmin + 1).ToString() + "x-uri,    ";
                labelHeight.Text = (ymax - ymin + 1).ToString() + "x-uri,    ";
                labelWidth.Text += (float)((xmax - xmin + 1) * numericUpDown5.Value / 10) + "cm";
                labelHeight.Text += (float)((ymax - ymin + 1) * numericUpDown5.Value / 10) + "cm";
                pmin = new Point(xmin, ymin);
                msize = new Size(xmax - xmin, ymax - ymin);
            }
            else
            {
                labelWidth.Text = "0";
                labelHeight.Text = "0";
            }
        }
        private void ModelEditor_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(ModelInfo.BackgroundColor, 1);
            using (Graphics g = e.Graphics)
            {
                // linii orizontale
                int l = ModelInfo.Patrat;
                while (l < panelDraw.Height)
                {
                    g.DrawLine(pen, panelDraw.Location.X, l, panelDraw.Size.Width, l);
                    l += ModelInfo.Patrat;
                }
                // linii verticale
                l = ModelInfo.Patrat;
                while (l < panelDraw.Width)
                {
                    g.DrawLine(pen, l, panelDraw.Location.Y, l, panelDraw.Size.Height);
                    l += ModelInfo.Patrat;
                }
                if (cursorpoint != default)
                {
                    drawCursor();
                }
            }
        }
        private void ModelEditor_Activated(object sender, EventArgs e)
        {
            //this.Invalidate();
            this.ActiveControl = null;
        }
        private void ModelEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (comboBox1.SelectedIndex == 0 && (comboBox2.SelectedIndex != 1))
            {
                if (e.KeyData == Keys.Back)
                {
                    if (mainColorBox.BackgroundImage == null)
                    {
                        mainColorBox.BackgroundImage = Resources.ereaser;
                        panelDraw.Cursor = CursorHelper.FromByteArray(Resources.Cursor2);
                        comboBox2.SelectedIndex = 2;
                    }
                    else
                    {
                        mainColorBox.BackgroundImage = null;
                        panelDraw.Cursor = Cursors.Cross;
                        cursorpoint = default;
                        comboBox2.SelectedIndex = 0;
                    }
                }
            }
            if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 1)
            {
                drawCursor(this.BackColor);
                try { DrawX(cursorpoint, Xuri[cursorpoint]); }
                catch (KeyNotFoundException ex) { }
                if (e.KeyData == Keys.A) cursorpoint.X--;
                if (e.KeyData == Keys.D) cursorpoint.X++;
                if (e.KeyData == Keys.W) cursorpoint.Y--;
                if (e.KeyData == Keys.S) cursorpoint.Y++;
                if (e.KeyData == Keys.Escape) cursorpoint = default;
                if (e.KeyData == Keys.Space || e.KeyData == Keys.M)
                {
                    Xuri[cursorpoint] = mainColorBox.BackColor;
                    DrawX(cursorpoint);
                    History.Add(cursorpoint, mainColorBox.BackColor);
                }
                if (e.KeyData == Keys.Back)
                {
                    EraseX(cursorpoint);
                    History.Remove(cursorpoint, Xuri[cursorpoint]);
                    Xuri.Remove(cursorpoint);
                }
                drawCursor();
            }
            if (comboBox1.SelectedIndex == 2)
            {
                if (e.KeyData == Keys.A) ModelInfo.indexdraw.X -= ModelInfo.MoveDist;
                if (e.KeyData == Keys.D) ModelInfo.indexdraw.X += ModelInfo.MoveDist;
                if (e.KeyData == Keys.W) ModelInfo.indexdraw.Y -= ModelInfo.MoveDist;
                if (e.KeyData == Keys.S) ModelInfo.indexdraw.Y += ModelInfo.MoveDist;
                panelDraw.Refresh();
            }
            if (comboBox1.SelectedIndex == 1)
            {
                if (e.KeyData == Keys.Escape && SelectedRectangle != null)
                {
                    if (SelectedRectangle != null)
                    {
                        // trebuie salvate modificarile
                        SelectedRectangle.Dispose();
                        SelectedRectangle = null;
                        XuriCache.Clear();
                        panelDraw.ContextMenuStrip = null;
                    }
                }
                if (SelectedRectangle != null) 
                { 
                    if (e.KeyData == Keys.A)
                    {
                        Move(new Point(-ModelInfo.MoveDist, 0));
                    }
                    if (e.KeyData == Keys.W)
                    {
                        Move(new Point(0, -ModelInfo.MoveDist));
                    }
                    if (e.KeyData == Keys.S)
                    {
                        Move(new Point(0, ModelInfo.MoveDist));
                    }
                    if (e.KeyData == Keys.D)
                    {
                        Move(new Point(ModelInfo.MoveDist, 0));
                    }
                }
            }
            ActiveControl = null;
        }
        private void picturebox_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;
            if(!copycolor)
            {
                copycolor = true;
                oldcolor = pictureBox;
                ControlPaint.DrawBorder(flowLayoutPanel1.CreateGraphics(), new Rectangle(pictureBox.PointToScreen(Point.Empty), pictureBox.Size), SystemColors.ActiveCaption, ButtonBorderStyle.Dashed);
            }
            else
            {
                copycolor = false;
                Color aux = pictureBox.BackColor;
                pictureBox.BackColor = oldcolor.BackColor;
                oldcolor.BackColor = aux;
            }
        }
        private void picturebox_DoubleClick(object sender, EventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;
            //mainColorBox.BackColor = pictureBox.BackColor;
        }
        #endregion

        #region panelDraw
        private void panelDraw_Paint(object sender, PaintEventArgs e)
        {
            // desenam x-urile
            foreach (X x in Xuri)
            {
                DrawX(e.Graphics, x.Key, x.Value);
            }
            if (SelectedRectangle!=null && radioButton1.Checked)
            foreach(X x in XuriCache)
            {
                Point point = PointHelper.Sum(PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat), x.Key);
                DrawX(e.Graphics, point, x.Value);
            }
            if(radioButton2.Checked && tabControl1.SelectedIndex==1)
            foreach (X x in XuriCache)
            {
                DrawBack(e.Graphics, x.Key);
            }
        }
        private void panelDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (comboBox1.SelectedIndex == 0 && e.Button == MouseButtons.Left)
            {
                if(Shape_Drawing && pdrawing)
                {
                    Point point = panelDraw.PointToClient(Cursor.Position);
                    point.X = point.X / ModelInfo.Patrat;
                    point.Y = point.Y / ModelInfo.Patrat;
                    point.X -= ModelInfo.indexdraw.X;
                    point.Y -= ModelInfo.indexdraw.Y;
                    pendshape = point;
                }
                else panelDraw_MouseClick(sender, e);
            }
            if (comboBox1.SelectedIndex == 1 && e.Button == MouseButtons.Left) 
            {
                if (SelectPanel == null) SelectPanel = new SelectionTool(panelDraw, e.Location);
                //else this.Invalidate();
            }
                
        }
        private void panelDraw_MouseUp(object sender, MouseEventArgs e)
        {
            if (SelectPanel != null && radioButton1.Checked)
            {
                Rectangle zone = new Rectangle(SelectPanel.Location.X, SelectPanel.Location.Y, SelectPanel.Size.Width, SelectPanel.Size.Height);
                Rectangle patrat = new Rectangle(default, new Size(ModelInfo.Patrat, ModelInfo.Patrat));
                SelectPanel.Dispose();
                SelectPanel = null;
                // process the zone
                int xmin = 10000, xmax = -10000, ymin = 10000, ymax = -10000;
                bool ok = false;
                foreach (X x in Xuri)
                {
                    Point point = PointHelper.Multiply(PointHelper.Sum(x.Key, ModelInfo.indexdraw), ModelInfo.Patrat);
                    patrat.Location = point;
                    if (zone.Contains(patrat) || zone.IntersectsWith(patrat))
                    {
                        XuriCache[x.Key] = x.Value;
                        if (point.X < xmin) { xmin = point.X; ok = true; }
                        if (point.X + ModelInfo.Patrat > xmax) { xmax = point.X + ModelInfo.Patrat; ok = true; }
                        if (point.Y < ymin) { ymin = point.Y; ok = true; }
                        if (point.Y + ModelInfo.Patrat > ymax) { ymax = point.Y + ModelInfo.Patrat; ok = true; }
                    }
                }
                // draw the selected zone
                if (ok)
                {
                    SelectedRectangle = new Panel();
                    SelectedRectangle.BackColor = Color.FromArgb(100, SystemColors.ActiveCaption);
                    SelectedRectangle.Paint += new PaintEventHandler(SelectedRectangle_Paint);  // for border color, X
                    SelectedRectangle.Bounds = new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
                    SelectedRectangle.ContextMenuStrip = contextMenuStripSelectPanel;
                    panelDraw.Controls.Add(SelectedRectangle);
                    //SelectedRectangle.Invalidate();
                }
                // recalculate XuriCache
                if(ok)
                {
                    Dictionary<Point, Color> cache = new Dictionary<Point, Color>();
                    foreach(X x in XuriCache)
                    {
                        Xuri.Remove(x.Key);
                        Point point = PointHelper.Dif(x.Key, PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat));
                        cache[point] = x.Value;
                    }
                    XuriCache = cache;
                }
            }
            else if(comboBox2.SelectedIndex==0 && Shape_Drawing && e.Button == MouseButtons.Left && pdrawing)
            {//    LINIE
                Point point = panelDraw.PointToClient(Cursor.Position);
                point.X = point.X / ModelInfo.Patrat;
                point.Y = point.Y / ModelInfo.Patrat;
                point.X -= ModelInfo.indexdraw.X;
                point.Y -= ModelInfo.indexdraw.Y;
                pdrawing = false;
                pendshape = point;
            }
            ActiveControl = null;
        }
        private void panelDraw_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = panelDraw.PointToClient(Cursor.Position);
            point.X = point.X / ModelInfo.Patrat;
            point.Y = point.Y / ModelInfo.Patrat;
            point.X -= ModelInfo.indexdraw.X;
            point.Y -= ModelInfo.indexdraw.Y;
            if (comboBox1.SelectedIndex == 0 && e.Button == MouseButtons.Left)
            {
                if (comboBox2.SelectedIndex == 0)
                {
                    Xuri[point] = mainColorBox.BackColor;
                    DrawX(point);
                    History.Add(point, mainColorBox.BackColor);
                }
                else if (comboBox2.SelectedIndex == 1 && e.Button == MouseButtons.Left && panelDraw.Cursor == Cursors.Cross)
                {
                    panelDraw.Cursor = Cursors.No;
                    cursorpoint = point;
                    drawCursor();
                    ActiveControl = null;
                }
                else if (comboBox2.SelectedIndex == 2 && Xuri.ContainsKey(point))
                {
                    History.Remove(point, Xuri[point]);
                    Xuri.Remove(point);
                    EraseX(point);
                }
                else if(Shape_Drawing)
                {
                    //MessageBox.Show("shape click");
                    if(!pdrawing)
                    {
                        pdrawing = true;
                        pstartshape = point;
                        LastShape[point] = mainColorBox.BackColor;
                        DrawX(point);
                        //History.Add(point, mainColorBox.BackColor);
                    }
                    else
                    {
                        pdrawing = false;
                        //MessageBox.Show(LastShape.Count.ToString());
                        pendshape = point;
                        _pendshape = default;
                    }
                }
            }
            if(comboBox1.SelectedIndex == 1 && radioButton1.Checked && e.Button == MouseButtons.Left && SelectedRectangle!=null)
            {
                if(!SelectedRectangle.Bounds.Contains(Cursor.Position))
                {
                    History.Right();
                    foreach (X x in XuriCache)
                    {
                        Point point1 = PointHelper.Sum(PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat), x.Key);
                        Xuri[point1] = x.Value;
                        History.Add(x);
                    }
                    History.Down();
                    /* - NU STERGEM PASE UL*/
                    if (SelectedRectangle != null) SelectedRectangle.Dispose();
                    SelectedRectangle = null;
                    XuriCache.Clear();
                    panelDraw.ContextMenuStrip = null;
                }
            }
            if(comboBox1.SelectedIndex == 1 && radioButton2.Checked && e.Button == MouseButtons.Left)
            {
                if (!Xuri.ContainsKey(point)) return;
                ModelX.FillType type;
                if (radioButton3.Checked) type = ModelX.FillType.Culoare;
                else type = ModelX.FillType.Forma;
                XuriCache = ModelX.Fill(Xuri, point, type);
            }
        }
        #endregion

        #region SelectedPanel
        private void SelectedRectangle_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, SelectedRectangle.ClientRectangle, Color.Blue, ButtonBorderStyle.Solid);
        }
        public void DrawZone(Rectangle zone)
        {
            Rectangle r = new Rectangle(PointHelper.Division(zone.Location, ModelInfo.Patrat), new Size(zone.Width / ModelInfo.Patrat, zone.Height / ModelInfo.Patrat));
            foreach(X x in Xuri)
                if(r.Contains(x.Key))
                    DrawX(g, x.Key, x.Value);
        }
        #endregion
        
        #region X fct
        float dmargin { get =>  0.35f * ModelInfo.Weight; }
        public PointF ToAbsolutePoint(Point p)
        {
            PointF point0 = new PointF(p.X, p.Y);
            point0.X += ModelInfo.indexdraw.X;
            point0.Y += ModelInfo.indexdraw.Y;
            point0.X *= ModelInfo.Patrat;
            point0.Y *= ModelInfo.Patrat;
            point0.X += ModelInfo.Patrat / 2f;
            point0.Y += ModelInfo.Patrat / 2f;
            return point0;
        }
        public Point ToRelativePoint(PointF p)
        {
            Point p0 = new Point((int)p.X, (int)p.Y);
            p0 = PointHelper.Division(p0, ModelInfo.Patrat);
            return p0;
        }
        void drawCursor(Color color = default)
        {
            if (color == default) color = Color.Black;
            Pen blackpen = new Pen(color, ModelInfo.CursorWeight);
            cursorpoint.X += ModelInfo.indexdraw.X;
            cursorpoint.Y += ModelInfo.indexdraw.Y;
            g.DrawLine(blackpen, cursorpoint.X * ModelInfo.Patrat + 1, cursorpoint.Y * ModelInfo.Patrat + ModelInfo.Patrat / 2, cursorpoint.X * ModelInfo.Patrat + ModelInfo.Patrat, cursorpoint.Y * ModelInfo.Patrat + ModelInfo.Patrat / 2);
            g.DrawLine(blackpen, cursorpoint.X * ModelInfo.Patrat + ModelInfo.Patrat / 2, cursorpoint.Y * ModelInfo.Patrat + 1, cursorpoint.X * ModelInfo.Patrat + ModelInfo.Patrat / 2, cursorpoint.Y * ModelInfo.Patrat + ModelInfo.Patrat);
            cursorpoint.X -= ModelInfo.indexdraw.X;
            cursorpoint.Y -= ModelInfo.indexdraw.Y;
        }
        void DrawX(Point point0)
        {
            Pen pen = new Pen(mainColorBox.BackColor, ModelInfo.Weight);
            point0.X += ModelInfo.indexdraw.X;
            point0.Y += ModelInfo.indexdraw.Y;
            point0.X *= ModelInfo.Patrat;
            point0.Y *= ModelInfo.Patrat;
            PointF point = new PointF(point0.X, point0.Y);
            if (!ModelInfo.Fill)
            {
                g.DrawLine(pen, point.X + dmargin, point.Y + dmargin, point.X + ModelInfo.Patrat - dmargin, point.Y + ModelInfo.Patrat - dmargin);
                g.DrawLine(pen, point.X + dmargin, point.Y + ModelInfo.Patrat - dmargin, point.X + ModelInfo.Patrat - dmargin, point.Y + dmargin);
            }
            else
            {
                g.FillRectangle(new SolidBrush(mainColorBox.BackColor), point.X, point.Y, ModelInfo.Patrat, ModelInfo.Patrat);
            }
        }
        void DrawX(Point point0, Color color)
        {
            Pen pen = new Pen(color, ModelInfo.Weight);
            point0.X += ModelInfo.indexdraw.X;
            point0.Y += ModelInfo.indexdraw.Y;
            point0.X *= ModelInfo.Patrat;
            point0.Y *= ModelInfo.Patrat;
            PointF point = new PointF(point0.X, point0.Y);
            if (!ModelInfo.Fill)
            {
                g.DrawLine(pen, point.X + dmargin, point.Y + dmargin, point.X + ModelInfo.Patrat - dmargin, point.Y + ModelInfo.Patrat - dmargin);
                g.DrawLine(pen, point.X + dmargin, point.Y + ModelInfo.Patrat - dmargin, point.X + ModelInfo.Patrat - dmargin, point.Y + dmargin);
            }
            else
            {
                g.FillRectangle(new SolidBrush(color), point.X, point.Y, ModelInfo.Patrat, ModelInfo.Patrat);
            }
        }
        void DrawX(Graphics graphics, Point point0, Color color)
        {
            Pen pen = new Pen(color, ModelInfo.Weight);
            point0.X += ModelInfo.indexdraw.X;
            point0.Y += ModelInfo.indexdraw.Y;
            point0.X *= ModelInfo.Patrat;
            point0.Y *= ModelInfo.Patrat;
            PointF point = new PointF(point0.X, point0.Y);
            if (!ModelInfo.Fill)
            {
                graphics.DrawLine(pen, point.X + dmargin, point.Y + dmargin, point.X + ModelInfo.Patrat - dmargin, point.Y + ModelInfo.Patrat - dmargin);
                graphics.DrawLine(pen, point.X + dmargin, point.Y + ModelInfo.Patrat - dmargin, point.X + ModelInfo.Patrat - dmargin, point.Y + dmargin);
            }
            else
            {
                g.FillRectangle(new SolidBrush(color), point.X, point.Y, ModelInfo.Patrat, ModelInfo.Patrat);
            }
        }
        void DrawBack(Graphics graphics, Point point0)
        {
            SolidBrush brush = new SolidBrush(SystemColors.ActiveCaption);
            point0.X += ModelInfo.indexdraw.X;
            point0.Y += ModelInfo.indexdraw.Y;
            point0.X *= ModelInfo.Patrat;
            point0.Y *= ModelInfo.Patrat;
            PointF point = new PointF(point0.X, point0.Y);
            graphics.FillRectangle(brush, point.X + dmargin, point.Y + dmargin, ModelInfo.Patrat - dmargin, ModelInfo.Patrat - dmargin);
        }
        void EraseX(Point point)
        {
            SolidBrush brush = new SolidBrush(Color.White);
            Pen carou = new Pen(ModelInfo.BackgroundColor);
            point.X += ModelInfo.indexdraw.X;
            point.Y += ModelInfo.indexdraw.Y;
            point.X *= ModelInfo.Patrat;
            point.Y *= ModelInfo.Patrat;
            g.FillRectangle(brush, point.X + 1, point.Y + 1, ModelInfo.Patrat - 1, ModelInfo.Patrat - 1);
            g.DrawRectangle(carou, point.X, point.Y, ModelInfo.Patrat, ModelInfo.Patrat);
        }
        void Paste(Point p)
        {
            SelectedRectangle = new Panel();
            SelectedRectangle.BackColor = Color.FromArgb(100, SystemColors.ActiveCaption);
            SelectedRectangle.Paint += new PaintEventHandler(SelectedRectangle_Paint);  // for border color, X
            SelectedRectangle.ContextMenuStrip = contextMenuStripSelectPanel;
            panelDraw.Controls.Add(SelectedRectangle);
            SelectedRectangle.Invalidate();
            ActiveControl = null;
            // obtinem diferenta intre selectepanel si point in xuri
            p = ToRelativePoint(p);
            //Point l = PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat);
            //Point d = PointHelper.Dif(SelectedRectangle.Location, p);
            int mxw = -1000000, mxh = -1000000;
            foreach(X x in XuriCache)
            {
                //Xuri[PointHelper.Sum(p, x.Key)] = x.Value;
                DrawX(PointHelper.Sum(p, x.Key), x.Value);
                if (x.Key.X > mxw) mxw = x.Key.X;
                if (x.Key.Y > mxh) mxh = x.Key.Y;
            }
            SelectedRectangle.Location = PointHelper.Multiply(p, ModelInfo.Patrat);
            SelectedRectangle.Width = (mxw+1) * ModelInfo.Patrat;
            SelectedRectangle.Height = (mxh+1) * ModelInfo.Patrat;
            SelectedRectangle.Visible = true;
        }
        void Move(Point p) // from selectedpanel
        {
            /*foreach (X x in XuriCache)
            {
                Point point = PointHelper.Sum(PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat), x.Key);
                Xuri.Remove(point);
            }
            foreach (X x in XuriCache)
            {
                Point point = PointHelper.Sum(PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat), x.Key);
                Xuri[PointHelper.Sum(point, p)] = x.Value;
            }*/
            SelectedRectangle.Location = PointHelper.Sum(SelectedRectangle.Location, PointHelper.Multiply(p, ModelInfo.Patrat));
            //panelDraw.Invalidate();
        }
        void RotateDr()
        {
            Dictionary<Point, Color> aux = new Dictionary<Point, Color>();
            Point p = new Point();
            foreach (X x in XuriCache)
            {
                p.X = x.Key.Y;
                p.Y = x.Key.X;
                aux[p] = x.Value;
            }
            int width = SelectedRectangle.Size.Width, height = SelectedRectangle.Size.Height;
            SelectedRectangle.Size = new Size(height, width);
            XuriCache = aux;
            this.Invalidate();
        }
        Bitmap SaveImg()
        {
            // info
            int xmin = 10000, xmax = -10000, ymin = 10000, ymax = -10000;
            foreach (X pair in Xuri)
            {
                if (pair.Key.X < xmin) xmin = pair.Key.X;
                if (pair.Key.X > xmax) xmax = pair.Key.X;
                if (pair.Key.Y < ymin) ymin = pair.Key.Y;
                if (pair.Key.Y > ymax) ymax = pair.Key.Y;
            }
            if (xmin != 10000 && ymin != 10000)
            {
                --xmin; ++xmax; --ymin; ++ymax;
                pmin = PointHelper.Sum(new Point(xmin* ModelInfo.Patrat, ymin* ModelInfo.Patrat), panelDraw.PointToScreen(Point.Empty));
                msize = new Size((xmax - xmin +1)* ModelInfo.Patrat, (ymax - ymin + 1)* ModelInfo.Patrat);
                Rectangle bounds = panelDraw.Bounds;
                Bitmap bitmap = new Bitmap(msize.Width, msize.Height);
                Graphics grph = Graphics.FromImage(bitmap);
                grph.CopyFromScreen(PointHelper.Sum(pmin, PointHelper.Multiply(ModelInfo.indexdraw, ModelInfo.Patrat)), Point.Empty, msize);
                //bitmap.Save(path, ImageFormat.Png);
                return bitmap;
            }
            else
            {
                Bitmap b = new Bitmap(100, 100);
                for (int i = 0; i < 100; ++i)
                    for (int j = 0; j < 100; ++j)
                        b.SetPixel(i, j, System.Drawing.Color.White);
                return b;
            }
        }
        #endregion

        #region comboBox
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // save selectedrectangle
            History.Right();
            if(SelectedRectangle != null && SelectedRectangle.Visible == true)
                foreach (X x in XuriCache)
                {
                    Point point1 = PointHelper.Sum(PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat), x.Key);
                    Xuri[point1] = x.Value;
                    History.Add(x);
                }
            History.Down();
            // clear memory, change contextmenustrip
            if (tabControl1.SelectedIndex == 1)
            {
                if (SelectedRectangle != null) SelectedRectangle.Dispose();
                SelectedRectangle = null;
                //XuriCache.Clear();
                panelDraw.ContextMenuStrip = contextMenuStripPaste;
            }
            tabControl1.SelectedIndex = comboBox1.SelectedIndex;
            if(comboBox1.SelectedIndex == 0)
            {
                panelDraw.ContextMenuStrip = contextMenuStripDraw;
                panelDraw.Cursor = Cursors.Cross;
                if(SelectedRectangle != null) SelectedRectangle.Dispose();
                SelectedRectangle = null;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                panelDraw.ContextMenuStrip = contextMenuStripPaste;
                panelDraw.Cursor = Cursors.Default;
            }
            else if(comboBox1.SelectedIndex == 2)
            {
                panelDraw.ContextMenuStrip = null;
                panelDraw.Cursor = Cursors.No;
                if (SelectedRectangle != null) SelectedRectangle.Dispose();
                SelectedRectangle = null;
            }
            this.ActiveControl = null;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                mainColorBox.BackgroundImage = null;
                panelDraw.Cursor = Cursors.Cross;
                cursorpoint = default;
                deselect_radio();
                radioButton5.Checked = true;
            }
            else if (comboBox2.SelectedIndex == 1)
            {
                mainColorBox.BackgroundImage = null;
                panelDraw.Cursor = Cursors.Cross;
                deselect_radio();
                radioButton6.Checked = true;
            }
            else if (comboBox2.SelectedIndex == 2)
            {
                mainColorBox.BackgroundImage = Resources.ereaser;
                panelDraw.Cursor = CursorHelper.FromByteArray(Resources.Cursor2);
                deselect_radio();
                radioButton7.Checked = true;
            }
            else if (comboBox2.SelectedIndex == 3)
            {
                deselect_radio();
                radioButton8.Checked = true;
            }
            else if (comboBox2.SelectedIndex == 4)
            {
                deselect_radio();
                radioButton9.Checked = true;
            }
            //panelDraw.Refresh();
            this.ActiveControl = null;
        }
        void deselect_radio()
        {
            foreach(RadioButton r in flowLayoutPanel2.Controls)
            {
                r.Checked = false;
            }
        }
        #endregion

        #region contextMenuStrip
        // tab1
        private void schimbăToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                ContextMenuStrip context = item.Owner as ContextMenuStrip;
                if (context != null)
                {
                    PictureBox pictureBox = context.SourceControl as PictureBox;
                    colorDialog1.Color = pictureBox.BackColor;
                    if (colorDialog1.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox.BackColor = colorDialog1.Color;
                    }
                }
            }
        }
        private void copiazăCuloareaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                ContextMenuStrip context = item.Owner as ContextMenuStrip;
                if (context != null)
                {
                    Point point = new Point(context.Left, context.Top);
                    point = panelDraw.PointToClient(point);
                    point.X = point.X / ModelInfo.Patrat;
                    point.Y = point.Y / ModelInfo.Patrat;
                    point.X -= ModelInfo.indexdraw.X;
                    point.Y -= ModelInfo.indexdraw.Y;
                    try
                    {
                        mainColorBox.BackColor = Xuri[point];
                    }
                    catch (KeyNotFoundException ex) { }
                }
            }
        }
        private void coloreazăToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem; if (item == null) return;
            ToolStripMenuItem item0 = item.OwnerItem as ToolStripMenuItem; if (item0 == null) return;
            ContextMenuStrip context = item0.Owner as ContextMenuStrip; if (context == null) return;
            Point point = new Point(context.Left, context.Top);
            point = panelDraw.PointToClient(point);
            point.X = point.X / ModelInfo.Patrat;
            point.Y = point.Y / ModelInfo.Patrat;
            point.X -= ModelInfo.indexdraw.X;
            point.Y -= ModelInfo.indexdraw.Y;
            Color color = new Color();
            try { color = Xuri[point]; }
            catch (KeyNotFoundException ex) { Fill(point); }
            finally
            {
                if (color != new Color() && color != mainColorBox.BackColor) Fill_Change(point, color);
            }
        }
        private void ștergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem; if (item == null) return;
            ToolStripMenuItem item0 = item.OwnerItem as ToolStripMenuItem; if (item0 == null) return;
            ContextMenuStrip context = item0.Owner as ContextMenuStrip; if (context == null) return;
            Point point = new Point(context.Left, context.Top);
            point = panelDraw.PointToClient(point);
            point.X = point.X / ModelInfo.Patrat;
            point.Y = point.Y / ModelInfo.Patrat;
            point.X -= ModelInfo.indexdraw.X;
            point.Y -= ModelInfo.indexdraw.Y;
            try { Fill_Erase(point, Xuri[point]); }
            catch (KeyNotFoundException ex) { }
        }
        private void adaugăToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem; if (item == null) return;
            ToolStripMenuItem item0 = item.OwnerItem as ToolStripMenuItem; if (item0 == null) return;
            ContextMenuStrip context = item0.Owner as ContextMenuStrip; if (context == null) return;
            Point point = new Point(context.Left, context.Top);
            point = panelDraw.PointToClient(point);
            point.X = point.X / ModelInfo.Patrat;
            point.Y = point.Y / ModelInfo.Patrat;
            point.X -= ModelInfo.indexdraw.X;
            point.Y -= ModelInfo.indexdraw.Y;
            Fill_AddContur(point);
        }
        private void ștergeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem; if (item == null) return;
            ToolStripMenuItem item0 = item.OwnerItem as ToolStripMenuItem; if (item0 == null) return;
            ContextMenuStrip context = item0.Owner as ContextMenuStrip; if (context == null) return;
            Point point = new Point(context.Left, context.Top);
            point = panelDraw.PointToClient(point);
            point.X = point.X / ModelInfo.Patrat;
            point.Y = point.Y / ModelInfo.Patrat;
            point.X -= ModelInfo.indexdraw.X;
            point.Y -= ModelInfo.indexdraw.Y;
            Fill_RemoveContur(point);
        }
        private void culoarePrincipalăToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Point point = new Point(contextMenuStripDraw.Left, contextMenuStripDraw.Top);
            point = panelDraw.PointToClient(point);
            point.X = point.X / ModelInfo.Patrat;
            point.Y = point.Y / ModelInfo.Patrat;
            point.X -= ModelInfo.indexdraw.X;
            point.Y -= ModelInfo.indexdraw.Y;
            Color color = new Color();
            try { color = Xuri[point]; }
            catch (KeyNotFoundException ex) { }
            finally
            {
                if (color != new Color()) Fill_ChangeContur(point, color);
            }
        }

        // tab2
        private void copiazăToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelDraw.ContextMenuStrip = contextMenuStripPaste;
            // adăugam xurile copiate inapoi in memorie
            foreach(X x in XuriCache)
            {
                Xuri[PointHelper.Sum(x.Key, PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat))] = x.Value;
            }
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // salvam fostul paste
            History.Right();
            if(SelectedRectangle!=null && SelectedRectangle.Size != Size.Empty)
            {
                foreach(X x in XuriCache)
                {
                    Point p = PointHelper.Sum(PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat), x.Key);
                    Xuri[p] = x.Value;
                    // DrawX(p, x.Value);
                    History.Add(x);
                }
            }
            History.Down();
            // facem paste efectif
            ToolStripMenuItem item = sender as ToolStripMenuItem; if (item == null) return;
            ContextMenuStrip context = item.Owner as ContextMenuStrip; if (context == null) return;
            Point point = new Point(context.Left, context.Top);
            point = panelDraw.PointToClient(point);
            panelDraw.Controls.Clear();
            Paste(point);
            panelDraw.ContextMenuStrip = contextMenuStripPaste;
        }
        private void șterge2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*foreach(X x in XuriCache)
            {
                Point point = PointHelper.Sum(PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat), x.Key);
                Xuri.Remove(point);
                EraseX(point);
            }*/
            XuriCache.Clear();
            SelectedRectangle.Visible = false;
        }
        private void decupeazăToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelDraw.ContextMenuStrip = contextMenuStripPaste;
        }
        private void orizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int n = SelectedRectangle.Width / ModelInfo.Patrat;
            Dictionary<Point, Color> aux = new Dictionary<Point, Color>();
            foreach(X x in XuriCache)
                aux[new Point(n - x.Key.X - 1, x.Key.Y)] = x.Value;
            XuriCache = aux;
            this.Invalidate();
        }
        private void verticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int m = SelectedRectangle.Height / ModelInfo.Patrat;
            Dictionary<Point, Color> aux = new Dictionary<Point, Color>();
            foreach (X x in XuriCache)
                aux[new Point(x.Key.X, m-x.Key.Y-1)] = x.Value;
            XuriCache = aux;
            this.Invalidate();
        }
        private void contextMenuStripSelectPanel_Opening(object sender, CancelEventArgs e)
        {
            Bitmap bitmap = new Bitmap(100, 100);
            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    bitmap.SetPixel(x, y, mainColorBox.BackColor);
                }
            }
            culoarePrincipalăToolStripMenuItem.Image = bitmap;
        }
        private void culoarePrincipalăToolStripMenuItem_Click(object sender, EventArgs e)
        {
            History.Right();
            foreach(X x in XuriCache.ToList())
            {
                Point point = PointHelper.Sum(PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat), x.Key);
                XuriCache[x.Key] = mainColorBox.BackColor;
                Xuri[point] = mainColorBox.BackColor;
                History.Add(point, mainColorBox.BackColor);
            }
            History.Down();
            SelectedRectangle.Invalidate();
        }
        private void alegeAltăCuloareToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Point point = new Point(contextMenuStripDraw.Left, contextMenuStripDraw.Top);
            point = panelDraw.PointToClient(point);
            point.X = point.X / ModelInfo.Patrat;
            point.Y = point.Y / ModelInfo.Patrat;
            point.X -= ModelInfo.indexdraw.X;
            point.Y -= ModelInfo.indexdraw.Y;
            Color color = new Color();
            try { color = Xuri[point]; }
            catch (KeyNotFoundException ex) { }
            finally
            {
                colorDialog1.Color = color;
                if (color != new Color() && colorDialog1.ShowDialog() == DialogResult.OK) Fill_ChangeContur(point, color, colorDialog1.Color);
            }
        }
        private void alegeAltăCuloareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                History.Right();
                foreach (X x in XuriCache.ToList())
                {
                    Point point = PointHelper.Sum(PointHelper.Division(SelectedRectangle.Location, ModelInfo.Patrat), x.Key);
                    XuriCache[x.Key] = colorDialog1.Color;
                    Xuri[point] = colorDialog1.Color;
                    History.Add(x);
                }
                History.Down();
                SelectedRectangle.Invalidate();
            }
        }
        #endregion

        #region Fill
        void Fill(Point point)
        {
            Color color = new Color();
            Point point1 = new Point();
            Stack<Point> s = new Stack<Point>();
            s.Push(point);
            History.Right();
            while(s.Count > 0)
            {
                point = s.Pop();
                Xuri[point] = mainColorBox.BackColor;
                DrawX(point);
                History.Add(point, mainColorBox.BackColor);
                foreach(Point p in directii)
                {
                    point1.X = point.X + p.X;
                    point1.Y = point.Y + p.Y;
                    try { color = Xuri[point1]; }
                    catch (KeyNotFoundException ex) { s.Push(point1); }
                }
            }
            History.Down();
        }
        void Fill_Erase(Point point, Color color)
        {
            Color c = new Color();
            Point point1 = new Point();
            Stack<Point> s = new Stack<Point>();
            s.Push(point);
            History.Right();
            while (s.Count > 0)
            {
                point = s.Pop();
                History.Remove(point, Xuri[point]);
                Xuri.Remove(point);
                EraseX(point);
                foreach (Point p in directii)
                {
                    point1.X = point.X + p.X;
                    point1.Y = point.Y + p.Y;
                    try
                    {
                        c = Xuri[point1];
                        if (c == color) s.Push(point1);
                    }
                    catch (KeyNotFoundException ex) { }
                }
            }
            History.Down();
        }
        void Fill_Change(Point point, Color color)
        {
            bool found = true;
            Color c = new Color();
            Point point2 = new Point();
            Stack<Point> s = new Stack<Point>();
            Xuri[point] = mainColorBox.BackColor;
            s.Push(point);
            History.Right();
            while (s.Count > 0)
            {
                point = s.Pop();
                if (Xuri[point] == color)
                {
                    Xuri[point] = mainColorBox.BackColor;
                    DrawX(point);
                    History.Add(point, mainColorBox.BackColor);
                }
                foreach (Point p in directii)
                {
                    point2.X = point.X + p.X;
                    point2.Y = point.Y + p.Y;
                    found = true;
                    try { Xuri.TryGetValue(point2, out c); }
                    catch (KeyNotFoundException ex) { found = false; }
                    finally
                    {
                        if (c == color && found) s.Push(point2);
                    }
                }
            }
            History.Down();
        }
        void Fill_AddContur(Point point)
        {
            Point point1 = new Point();
            Stack<Point> s = new Stack<Point>();
            Dictionary<Point, bool> pus = new Dictionary<Point, bool>();
            Stack<Point> contur = new Stack<Point>();
            s.Push(point);
            while (s.Count > 0)
            {
                point = s.Pop();
                pus[point] = true;
                foreach (Point p in dir4)
                {
                    point1.X = point.X + p.X;
                    point1.Y = point.Y + p.Y;
                    if (!Xuri.ContainsKey(point1))
                    {
                        DrawX(point1);
                        contur.Push(point1);
                    }
                    else {
                        bool ok;
                        pus.TryGetValue(point1, out ok);
                        if (ok == false)
                            s.Push(point1);
                    }
                }
            }
            History.Right();
            foreach (Point p in contur)
            {
                Xuri[p] = mainColorBox.BackColor;
                History.Add(p, mainColorBox.BackColor);
                History.Down();
            }
        }
        void Fill_RemoveContur(Point point)
        {
            Point point1 = new Point();
            Stack<Point> s = new Stack<Point>();
            Dictionary<Point, bool> pus = new Dictionary<Point, bool>();
            Stack<Point> contur = new Stack<Point>();
            s.Push(point);

            while (s.Count > 0)
            {
                point = s.Pop();
                pus[point] = true;
                foreach (Point p in dir4)
                {
                    point1.X = point.X + p.X;
                    point1.Y = point.Y + p.Y;
                    if (!Xuri.ContainsKey(point1))
                    {
                        contur.Push(point);
                    }
                    else
                    {
                        bool ok;
                        pus.TryGetValue(point1, out ok);
                        if (ok == false)
                            s.Push(point1);
                    }
                }
            }
            History.Right();
            foreach (Point p in contur)
            {
                History.Remove(point, Xuri[point]);
                Xuri.Remove(p);
                EraseX(p);
            }
            History.Down();
        }
        void Fill_ChangeContur(Point point, Color color)
        {
            bool found = true;
            Color c = new Color();
            Point point2 = new Point();
            Stack<Point> s = new Stack<Point>();
            Xuri[point] = mainColorBox.BackColor;
            s.Push(point);
            History.Right();
            while (s.Count > 0)
            {
                point = s.Pop();
                if (Xuri[point] == color)
                {
                    Xuri[point] = mainColorBox.BackColor;
                    DrawX(point);
                    History.Add(point, mainColorBox.BackColor);
                }
                foreach (Point p in dir8)
                {
                    point2.X = point.X + p.X;
                    point2.Y = point.Y + p.Y;
                    found = true;
                    try { Xuri.TryGetValue(point2, out c); }
                    catch (KeyNotFoundException ex) { found = false; }
                    finally
                    {
                        if (c == color && found) s.Push(point2);
                    }
                }
            }
            History.Down();
        }
        void Fill_ChangeContur(Point point, Color color0, Color color1)
        {
            bool found = true;
            Color c = new Color();
            Point point2 = new Point();
            Stack<Point> s = new Stack<Point>();
            Xuri[point] = color1;
            s.Push(point);
            History.Right();
            while (s.Count > 0)
            {
                point = s.Pop();
                if (Xuri[point] == color0)
                {
                    Xuri[point] = color1;
                    DrawX(point, color1);
                    History.Add(point, color1);
                }
                foreach (Point p in dir8)
                {
                    point2.X = point.X + p.X;
                    point2.Y = point.Y + p.Y;
                    found = true;
                    try { Xuri.TryGetValue(point2, out c); }
                    catch (KeyNotFoundException ex) { found = false; }
                    finally
                    {
                        if (c == color0 && found) s.Push(point2);
                    }
                }
            }
            History.Down();
        }
        #endregion

        #region Controls tab3
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            ModelInfo.MoveDist = (int)numericUpDown1.Value;
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            ModelInfo.Patrat = (int)numericUpDown2.Value;
            this.Refresh();
            this.ActiveControl = null;
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            ModelInfo.Weight = (float)numericUpDown3.Value;
            this.Invalidate();
            this.ActiveControl = null;
        }
        private void pictureBox3_BackColorChanged(object sender, EventArgs e)
        {
            ModelInfo.BackgroundColor = pictureBox3.BackColor;
            //Refresh();
        }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            ModelInfo.CursorWeight = (float)numericUpDown4.Value;
            this.Refresh();
            this.ActiveControl = null;
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            ModelInfo.Fire = (int)numericUpDown5.Value;
            int xmin = 10000, xmax = -10000, ymin = 10000, ymax = -10000;
            foreach (KeyValuePair<Point, Color> pair in Xuri)
            {
                if (pair.Key.X < xmin) xmin = pair.Key.X;
                if (pair.Key.X > xmax) xmax = pair.Key.X;
                if (pair.Key.Y < ymin) ymin = pair.Key.Y;
                if (pair.Key.Y > ymax) ymax = pair.Key.Y;
            }
            if (xmin != 10000 && ymin != 10000)
            {
                labelWidth.Text = (xmax - xmin + 1).ToString() + "x-uri,    ";
                labelHeight.Text = (ymax - ymin + 1).ToString() + "x-uri,    ";
                labelWidth.Text += (float)((xmax - xmin + 1) * numericUpDown5.Value / 10) + "cm";
                labelHeight.Text += (float)((ymax - ymin + 1) * numericUpDown5.Value / 10) + "cm";
            }
            else
            {
                labelWidth.Text = "0";
                labelHeight.Text = "0";
            }
            this.ActiveControl = null;
        }
        private void culoriPredefiniteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Doriți să resetați paleta de culori?\nAceasta acțiune nu poate fi anulată.", "Atenție!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (StreamReader fin = new StreamReader(Directory.GetParent(Proiect.Path).FullName + "\\_Defaults\\model.info"))
                {
                    for (int i = 1; i <= 8; ++i) fin.ReadLine();
                    mainColorBox.BackColor = Color.FromArgb(int.Parse(fin.ReadLine()));
                    foreach(PictureBox pictureBox in flowLayoutPanel1.Controls)
                    {
                        pictureBox.BackColor = Color.FromArgb(int.Parse(fin.ReadLine()));
                    }
                }
            }
        }
        private void ștergeToateCulorileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Doriți să resetați paleta de culori?\nAceasta acțiune nu poate fi anulată.", "Atenție!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (PictureBox pictureBox in flowLayoutPanel1.Controls)
                    pictureBox.BackColor = Color.White;
            }
        }
        private void ajutorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("file:///C:/Users/SOFRAN%20ROMULUS/Workspace/ASP%20Exercitii/Tutorial.html");
            /*HelpPage helpPage = new HelpPage();
            helpPage.Show();*/
        }
        private void contextMenuStripDraw_Opening(object sender, CancelEventArgs e)
        {
            Bitmap bitmap = new Bitmap(100, 100);
            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    bitmap.SetPixel(x, y, mainColorBox.BackColor);
                }
            }
            culoarePrincipalăToolStripMenuItem1.Image = bitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ModelInfo.indexdraw = Point.Empty;
            panelDraw.Invalidate();
            this.ActiveControl = null;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ChangeColors changeColors = new ChangeColors();
            changeColors.Info = this.ModelInfo;
            changeColors.Xuri = XuriCache;
            if(changeColors.ShowDialog()==DialogResult.OK)
            {
                XuriCache = changeColors.Data;
                SelectedRectangle.Invalidate();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) { ModelInfo.Fill = checkBox1.Checked; panelDraw.Invalidate(); }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton r = sender as RadioButton;
            int c = int.Parse(r.Name.Substring(11));
            comboBox2.SelectedIndex = c - 5;
            this.ActiveControl = null;
        }

        private void RemoveClick(object sender, EventArgs e) => this.ActiveControl = null;

        private void pictureBoxArrow_Click(object sender, EventArgs e)
        {
            PictureBox box = sender as PictureBox;
            string[] array = box.Tag.ToString().Split(' ');
            Point point = new Point(int.Parse(array[0]), int.Parse(array[1]));
            if(box.BackColor == SystemColors.Control)
            {
                box.BackColor = SystemColors.ActiveCaption;
                directii.Add(point);
            }
            else
            {
                box.BackColor = SystemColors.Control;
                directii.Remove(point);
            }
            this.ActiveControl = null;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            this.Refresh();
            this.ActiveControl = null;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Bitmap b = SaveImg();
            b.Save(Proiect.Path + "\\Proiect\\Imagini\\" + DateTime.Now.ToString().Replace(':', '-') + ".png", ImageFormat.Png);
            PrewiewImage prewiew = new PrewiewImage(b);
            prewiew.Show();
            ActiveControl = null;
        }

        #endregion

        #region History
        public void ExecuteTask(string task)
        {
            if (task == String.Empty) {  return; }//MessageBox.Show("nu se executa");
            string[] v = task.Split('*');
            foreach (string x1 in v)
                if (x1 != "")
                {
                    string[] s = x1.Split(' ');
                    Point p = new Point(int.Parse(s[1]), int.Parse(s[2]));
                    if (s[0] == "+")
                    {
                        Xuri[p] = Color.FromArgb(int.Parse(s[3]));
                        DrawX(p, Xuri[p]);
                    }
                    else
                    {
                        Xuri.Remove(p);
                        EraseX(p);
                    }
                }
        }

        private void Redo_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < numericUpDown6.Value; ++i)
            {
                string task = History.Redo();
                ExecuteTask(task);
            }
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            for(int i=0;i<numericUpDown6.Value;++i)
            {
                string task = History.Undo();
                //MessageBox.Show(task);
                ExecuteTask(task);
            }
            
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown6.Value <= 9)
                numericUpDown6.Width = 36;
            else if (numericUpDown6.Value <= 99)
                numericUpDown6.Width = 49;
            else numericUpDown6.Value = 64;
        }

        void UndoChanged(bool newvalue) => button3.Enabled = newvalue;
        void RedoChanged(bool newvalue) => button4.Enabled = newvalue;
        #endregion

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 1)
            {
                if(SelectedRectangle != null) SelectedRectangle.Dispose();
                SelectedRectangle = null;
                //XuriCache.Clear();
                panelDraw.ContextMenuStrip = contextMenuStripPaste;
            }
            comboBox1.SelectedIndex = tabControl1.SelectedIndex;
            
            if (comboBox1.SelectedIndex == 2)
            {
                // load from model info
                numericUpDown1.Value = ModelInfo.MoveDist;
                numericUpDown2.Value = ModelInfo.Patrat;
                numericUpDown3.Value = (decimal)ModelInfo.Weight;
                numericUpDown4.Value = (decimal)ModelInfo.CursorWeight;
                numericUpDown5.Value = ModelInfo.Fire;
                pictureBox3.BackColor = ModelInfo.BackgroundColor;
                checkBox1.Checked = ModelInfo.Fill;
                // info
                int xmin = 10000, xmax = -10000, ymin = 10000, ymax = -10000;
                foreach (KeyValuePair<Point, Color> pair in Xuri)
                {
                    if (pair.Key.X < xmin) xmin = pair.Key.X;
                    if (pair.Key.X > xmax) xmax = pair.Key.X;
                    if (pair.Key.Y < ymin) ymin = pair.Key.Y;
                    if (pair.Key.Y > ymax) ymax = pair.Key.Y;
                }
                if (xmin != 10000 && ymin != 10000)
                {
                    labelWidth.Text = (xmax - xmin + 1).ToString() + "x-uri,    ";
                    labelHeight.Text = (ymax - ymin + 1).ToString() + "x-uri,    ";
                    labelWidth.Text += (float)((xmax - xmin + 1) * numericUpDown5.Value / 10) + "cm";
                    labelHeight.Text += (float)((ymax - ymin + 1) * numericUpDown5.Value / 10) + "cm";
                    pmin = new Point(xmin, ymin);
                    msize = new Size(xmax - xmin, ymax - ymin);
                }
                else
                {
                    labelWidth.Text = "0";
                    labelHeight.Text = "0";
                }
                // directii
                foreach (Point point in directii)
                {
                    string s = point.X + " " + point.Y;
                    foreach (Control p in tabPage3.Controls)
                    {
                        if (p is PictureBox && p.Tag != null && p.Tag.ToString() == s)
                        {
                            p.BackColor = SystemColors.ActiveCaption;
                        }
                    }
                }
            }
            this.ActiveControl = null;
        }
        private bool Shape_Drawing
        {
            get
            {
                foreach (Control c in flowLayoutPanel2.Controls)
                    if(c is RadioButton r)
                        if (r.Text != "Pointer" && r.Text != "Taste" && r.Text != "Șterge" && r.Checked)
                            return true;
                return false;
            }
        }
        private Point pendshape
        {
            get => _pendshape;
            set
            {
                Dictionary<Point, Color> f=null, o= new Dictionary<Point, Color>(LastShape);
                if (radioButton8.Checked)
                    f = ShapeDraw.Line(mainColorBox.BackColor, pstartshape, value, ToAbsolutePoint(pstartshape), ToAbsolutePoint(value), ModelInfo.Patrat, ToRelativePoint);
                else if(radioButton9.Checked)
                    f = ShapeDraw.Square(mainColorBox.BackColor, pstartshape, value);
                else if(radioButton10.Checked)
                    f = ShapeDraw.Circle(mainColorBox.BackColor, pstartshape, value, ToAbsolutePoint(pstartshape), ToAbsolutePoint(value), ModelInfo.Patrat, ToRelativePoint);
                //delete
                if(pendshape!=default)
                foreach(X x in o)
                {
                    if(!f.Contains(x) && x.Value==mainColorBox.BackColor)
                    {
                        LastShape.Remove(x.Key);
                        EraseX(x.Key);
                        Color c;
                        if (Xuri.TryGetValue(x.Key, out c))
                        {
                            DrawX(x.Key, c);
                        }
                    }
                }
                _pendshape = value;
                //draw
                foreach(X x in f)
                if(!o.Contains(x))
                {
                    LastShape[x.Key] =  x.Value;
                    DrawX(x.Key, x.Value);
                }
                // add to main
                if(pdrawing == false)
                {
                    History.Right();
                    foreach (X x in LastShape)
                    {
                        Xuri[x.Key] = x.Value;
                        History.Add(x);
                    }
                    History.Down();
                    LastShape.Clear();
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e) { }
    }
}