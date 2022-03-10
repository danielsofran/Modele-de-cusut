using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Modele_de_cusut.Classes
{
    public static class PointHelper
    {
        public static Point Sum(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }
        public static Point Dif(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }
        public static Point Multiply(Point p1, int multiplyer)
        {
            return new Point(p1.X *multiplyer, p1.Y*multiplyer);
        }
        public static Point Division(Point p1, int div)
        {
            return new Point(p1.X / div, p1.Y / div);
        }
        public static void Normalize(ref Point point, int patrat)
        {
            point.X -= point.X % patrat;
            point.Y -= point.Y % patrat;
        }
    }
}
