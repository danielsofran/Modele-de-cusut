using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X = System.Collections.Generic.KeyValuePair<System.Drawing.Point, System.Drawing.Color>;
using Forma = System.Collections.Generic.Dictionary<System.Drawing.Point, System.Drawing.Color>;
using System.Drawing;
using System.Windows.Forms;

namespace Modele_de_cusut.Classes
{
    public static class ShapeDraw
    {
        public static Forma Square(Color c, Point start, Point end)
        {
            Forma rez = new Forma();
            for(int x=Math.Min(start.X, end.X); x<=Math.Max(start.X, end.X); ++x)
            {
                rez[new Point(x, start.Y)] = c;
                rez[new Point(x, end.Y)] = c;
            }
            for(int y = Math.Min(start.Y, end.Y); y<=Math.Max(start.Y, end.Y); ++y)
            {
                rez[new Point(start.X, y)] = c;
                rez[new Point(end.X, y)] = c;
            }
            return rez;
        }

        public static Forma Line(Color c, Point start, Point end, PointF astart, PointF aend, int l, Func<PointF, Point> f)
        {
            // y = (y2-y1)/(x2-x1) * x + (x2*y1-x1*y2)/(x2-x1)
            Forma rez = new Forma();
            rez[start] = rez[end] = c;
            //int angle = (int)(Math.Atan(((double)(aend.Y-astart.Y))/((double)(aend.X-astart.X)))*(double)180/Math.PI);
            int ys = (int)Math.Round(astart.Y - l / 2f), ye = (int)Math.Round(aend.Y-l/2f), xs = (int)Math.Round(astart.X - l / 2f), xe = (int)Math.Round(aend.X-l/2f);
            int iy = Math.Sign(ye - ys), ix = Math.Sign(xe - xs);
            if (Math.Abs(start.Y - end.Y) == Math.Abs(start.X - end.X))
            {
                for (int i = start.X, j = start.Y; i != end.X && j != end.Y; i += Math.Sign(end.X - start.X), j += Math.Sign(end.Y - start.Y))
                {
                    rez[new Point(i, j)] = c;
                }
            }
            else if (Math.Abs(start.Y - end.Y) < Math.Abs(start.X - end.X))
            {
                for (int i = xs; i != xe; i += ix)
                {
                    float y = getY(i, astart, aend);
                    //if (y.apart(astart.Y, aend.Y))
                    {
                        rez[f(new PointF(i, (float)Math.Round(y)))] = c;
                    }
                }
            }
            else if(Math.Abs(start.Y - end.Y) > Math.Abs(start.X - end.X))
            {
                for(int i = ys; i!= ye; i+=iy)
                {
                    float x = getX(i, astart, aend);
                    rez[f(new PointF((float)Math.Round(x), i))] = c;
                }
            }
            return rez;
        }

        private static float getY(int x, PointF p1, PointF p2)
        {
            return ((p2.Y - p1.Y) / (p2.X - p1.X) * x + (p2.X * p1.Y - p2.Y * p1.X) / (p2.X - p1.X));
        }

        private static float getX(int y, PointF p1, PointF p2)
        {
            return ((p1.X - p2.X) / (p1.Y - p2.Y) * y + (p2.X * p1.Y - p2.Y * p1.X) / (p1.Y - p2.Y));
        }

        public static Forma Circle(Color c, Point start, Point end, PointF astart, PointF aend, int l, Func<PointF, Point> f)
        {
            Forma rez = new Forma();
            int r = (int)(distance(astart, aend) / l);
            for (int x = start.X - r; x <= start.X + r + 1; x += 1)
                for (int y = start.Y - r; y <= start.Y + r + 1; y += 1)
                    if (distance(new Point(x, y), start) <= distance(start, end))
                        rez[new Point(x, y)] = c;
            Forma r2 = new Forma();
            foreach (X x in rez)
            {
                bool g = false;
                foreach(Point p0 in Modele_de_cusut.Editor.ModelEditor.dir4)
                {
                    Point p = PointHelper.Sum(x.Key, p0);
                    if(!rez.ContainsKey(p))
                    {
                        r2[x.Key] = x.Value;
                    }
                }
            }
            return r2;
            /*int r = (int)(distance(astart, aend)/l);
            for (int x = start.X - r; x <= start.X+r+1; x += 1)
                for (int y = start.Y - r; y <= start.Y + r + 1; y += 1)
                    if (distance(new Point(x, y), start) <= distance(start, end))
                        rez[new Point(x, y)] = c;
            float r = distance(astart, aend);
            for (float x = astart.X - r; x <= astart.X + r + l; x += l)
                for (float y = start.Y - r; y <= astart.Y + r + l; y += l)
                    if(distance(new PointF(x, y), astart)<=r)
                        rez[f(new PointF(x, y))] = c;
            */
            return rez;
        }

        private static float distance(PointF astart, PointF aend)
        {
            return (float)Math.Sqrt((astart.X - aend.X) * (astart.X - aend.X) + (astart.Y - aend.Y) * (astart.Y - aend.Y));
        }
        private static float distance(Point astart, Point aend)
        {
            return (float)Math.Sqrt((astart.X - aend.X) * (astart.X - aend.X) + (astart.Y - aend.Y) * (astart.Y - aend.Y));
        }
    }
}
