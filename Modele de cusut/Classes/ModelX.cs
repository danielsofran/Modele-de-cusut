using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modele_de_cusut.Editor;

namespace Modele_de_cusut.Classes
{
    public static class ModelX
    {
        public static void Save(Dictionary<Point, Color> Xuri, string path)
        {
            using (StreamWriter fout = new StreamWriter(path))
            {
                foreach (KeyValuePair<Point, Color> x in Xuri)
                    fout.WriteLine(x.Key.X + " " + x.Key.Y + " " + x.Value.ToArgb());
            }
        }

        public static void Load(out Dictionary<Point, Color> Xuri, string path)
        {
            Xuri = new Dictionary<Point, Color>();
            try
            {
                foreach (string line in File.ReadAllLines(path))
                {
                    Point point = new Point(int.Parse(line.Split(' ')[0]), int.Parse(line.Split(' ')[1]));
                    Color color = Color.FromArgb(int.Parse(line.Split(' ')[2]));
                    Xuri[point] = color;
                }
            }
            catch(Exception ex){ }
        }

        public static Tuple<Point, Point> MinMax(Dictionary<Point, Color> Xuri)
        {
            int xmin = 10000, xmax = -10000, ymin = 10000, ymax = -10000;
            foreach (KeyValuePair<Point, Color> pair in Xuri)
            {
                if (pair.Key.X < xmin) xmin = pair.Key.X;
                if (pair.Key.X > xmax) xmax = pair.Key.X;
                if (pair.Key.Y < ymin) ymin = pair.Key.Y;
                if (pair.Key.Y > ymax) ymax = pair.Key.Y;
            }
            return new Tuple<Point, Point>(new Point(xmin, ymin), new Point(xmax, ymax));
        }

        public enum FillType { Culoare, Forma};

        public static Dictionary<Point, Color> Fill(Dictionary<Point, Color> Xuri, Point start, FillType type)
        {
            Dictionary<Point, Color> rez = new Dictionary<Point, Color>();
            Stack<Point> q = new Stack<Point>();
            q.Push(start);
            while(q.Count>0)
            {
                Point p = q.Pop();
                rez[p] = Xuri[p];
                for (int k = 0; k < ModelEditor.directii.Count; ++k)
                {
                    int i = p.X + ModelEditor.directii[k].X;
                    int j = p.Y + ModelEditor.directii[k].Y;
                    Point p0 = new Point(i, j);
                    if (!Xuri.ContainsKey(p0) || rez.ContainsKey(p0)) continue;
                    if(type == FillType.Culoare && Xuri[p]==Xuri[p0])
                        q.Push(p0);
                    if (type == FillType.Forma)
                        q.Push(p0);
                }
            }
            return rez;
        }
    }
}
