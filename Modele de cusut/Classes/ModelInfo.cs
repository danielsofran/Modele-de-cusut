using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modele_de_cusut.Classes
{
    public class ModelInfo
    {
        public int Patrat { get; set; } = 10;
        public Point indexdraw = Point.Empty;
        public Color BackgroundColor { get; set; } = SystemColors.ControlDark;
        public float Weight { get; set; } = 2;
        public float CursorWeight { get; set; } = 1.5f;
        public int MoveDist { get; set; } = 1;
        public Color MainColor { get; set; } = Color.Black;
        public List<Color> Colors { get; set; } 
        public int Fire { get; set; } = 3;
        public bool Fill { get; set; } = false;
        public void FromFile(string path)
        {
            try
            {
                using (StreamReader fin = new StreamReader(path))
                {
                    Patrat = int.Parse(fin.ReadLine());
                    indexdraw.X = int.Parse(fin.ReadLine());
                    indexdraw.Y = int.Parse(fin.ReadLine());
                    BackgroundColor = Color.FromArgb(int.Parse(fin.ReadLine()));
                    Weight = float.Parse(fin.ReadLine());
                    CursorWeight = float.Parse(fin.ReadLine());
                    MoveDist = int.Parse(fin.ReadLine());
                    Fire = int.Parse(fin.ReadLine());
                    MainColor = Color.FromArgb(int.Parse(fin.ReadLine()));
                    Colors = new List<Color>();
                    for (int i = 1; i <= 24; ++i)
                        Colors.Add(Color.FromArgb(int.Parse(fin.ReadLine())));
                    //try { Fill = bool.Parse(fin.ReadLine()); } catch (Exception ex) { Fill = false; }
                }
            }
            catch (Exception ex) 
            {
                path = Manager.BaseDirectoryPath + "\\_Defaults\\model.info";
                using (StreamReader fin = new StreamReader(path))
                {
                    Patrat = int.Parse(fin.ReadLine());
                    indexdraw.X = int.Parse(fin.ReadLine());
                    indexdraw.Y = int.Parse(fin.ReadLine());
                    BackgroundColor = Color.FromArgb(int.Parse(fin.ReadLine()));
                    Weight = float.Parse(fin.ReadLine());
                    CursorWeight = float.Parse(fin.ReadLine());
                    MoveDist = int.Parse(fin.ReadLine());
                    Fire = int.Parse(fin.ReadLine());
                    MainColor = Color.FromArgb(int.Parse(fin.ReadLine()));
                    Colors = new List<Color>();
                    for (int i = 1; i <= 24; ++i)
                        Colors.Add(Color.FromArgb(int.Parse(fin.ReadLine())));
                    Fill = false;
                }
            }
        }
        public void ToFile(string path)
        {
            using (StreamWriter fout = new StreamWriter(path))
            {
                fout.WriteLine(Patrat);
                fout.WriteLine(indexdraw.X);
                fout.WriteLine(indexdraw.Y);
                fout.WriteLine(BackgroundColor.ToArgb());
                fout.WriteLine(Weight);
                fout.WriteLine(CursorWeight);
                fout.WriteLine(MoveDist);
                fout.WriteLine(Fire);
                fout.WriteLine(MainColor.ToArgb());
                foreach(Color color in Colors)
                    fout.WriteLine(color.ToArgb());
            }
        }
    }
}
