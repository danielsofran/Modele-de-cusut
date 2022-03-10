using Modele_de_cusut.Editor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using X = System.Collections.Generic.KeyValuePair<System.Drawing.Point, System.Drawing.Color>;
using Forma = System.Collections.Generic.Dictionary<System.Drawing.Point, System.Drawing.Color>;
using System.IO;
using System.Windows.Forms;

namespace Modele_de_cusut.Classes
{
    public static class History
    {
        private static int index = 0;
        public static string mode = "DOWN"; // can also be left, none
        private static List<string> v = new List<string>();

        public static void Down() => mode = "DOWN";
        public static void Right() { mode = "DOWN"; v.Add(""); mode = "RIGHT"; }
        public static bool CanUndo { get => index > 0; }
        public static bool CanRedo { get => index < v.Count-1; }
        public delegate void CanChanged(bool newvalue);
        public static event CanChanged CanUndo_Changed;
        public static event CanChanged CanRedo_Changed;
        public static void Add(string task) // *+/- x y color*...
        {
            if(mode == "DOWN")
            {
                if(v.Count==0 || task.Trim() != v.Last().Trim())
                {
                    v.Add(task);
                    index = v.Count-1;
                }
            }
            else if(mode == "RIGHT")
            {
                int sindex = v.Last().LastIndexOf('*');
                if (sindex < 0) sindex = 0;
                if (task.Trim() != v.Last().Substring(sindex))
                {
                    v[index] += "*" + task;
                }
            }
            CanUndo_Changed?.Invoke(index > 0);
            CanRedo_Changed?.Invoke(index < v.Count - 1);
        }

        public static void Add(X x) => Add("+ " + x.Key.X + " " + x.Key.Y + " " + x.Value.ToArgb());
        public static void Add(Point p, Color c) => Add(new X(p, c));
        public static void Add(Forma f)
        {
            string omode = mode;
            Right();
            foreach (X x in f)
                Add(x);
            mode = omode;
        }
        public static void Remove(X x) => Add("- " + x.Key.X + " " + x.Key.Y + " " + x.Value.ToArgb());
        public static void Remove(Point p, Color c) => Remove(new X(p, c));
        public static void Remove(Forma f)
        {
            string omode = mode;
            Right();
            foreach (X x in f)
                Remove(x);
            mode = omode;
        }

        public static string Undo()
        {
            if (index > 0)
            {
                string rez1 = "";
                string rez = v.ElementAt(index);
                string[] S = rez.Split('*');
                foreach(string s in S)
                if(s!="")
                {
                    if (s[0] == '+') rez1 += "-";
                    else rez1 += "+";
                    rez1 += s.Substring(1);
                    rez1 += "*";
                }
                --index;
                CanUndo_Changed?.Invoke(index > 0);
                CanRedo_Changed?.Invoke(index < v.Count - 1);
                return rez1;
            }
            return String.Empty;
        }

        public static void Modified()
        {
            v.RemoveRange(index+1, v.Count-index);
        }

        public static string Redo()
        {
            if (index < v.Count-1)
            {
                ++index;
                CanUndo_Changed?.Invoke(index > 0);
                CanRedo_Changed?.Invoke(index < v.Count - 1);
                return v.ElementAt(index);
            }
            return String.Empty;
        }

        public static void ToFile(string path)
        {
            using (StreamWriter sr = new StreamWriter(path +"model.history"))
            {
                sr.WriteLine(index);
                foreach (string task in v)
                    sr.WriteLine(task);
            }
        }

        public static void FromFile(string path)
        {
            if (!File.Exists(path + "\\model.history")) return;
            using (StreamReader sr = new StreamReader(path + "\\model.history"))
            {
                index = int.Parse(sr.ReadLine());
                string task;
                while ((task = sr.ReadLine()) != null)
                    v.Add(task);
            }
        }

        public static string ToString()
        {
            string rez = index + "\n";
            foreach(string s in v)
            {
                rez += s;
                rez += "\n";
            }
            return rez;
        }
    }
}
