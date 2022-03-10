using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modele_de_cusut.Classes
{
    public static class ToDoList
    {
        public static List<string> Tasks = new List<string>();

        public static void Add(string task)
        {
            Tasks.Add(task);
        }
        public static void Do()
        {
            foreach(string task in Tasks)
            {
                if (task.StartsWith("DELETE_DIR"))
                {
                    string dir = task.Substring(11);
                    Directory.Delete(dir, true);
                }
                else if (task.StartsWith("DELETE"))
                {
                    string file = task.Substring(7);
                    if(File.Exists(file))
                    File.Delete(file);
                }
                else if(task.StartsWith("COPY"))
                {
                    string[] cv = task.Split('*');
                    if (File.Exists(cv[1]))
                        File.Copy(cv[1], cv[2], true);
                }
            }
            Tasks.Clear();
        }
        public static void Save()
        {
            StreamWriter fout = new StreamWriter(Manager.BaseDirectoryPath + "\\_System\\todolist.txt");
            foreach (string task in Tasks)
            {
                fout.WriteLine(task);
            }
            fout.Close();
        }
        public static void Load()
        {
            foreach(string line in File.ReadAllLines(Manager.BaseDirectoryPath + "\\_System\\todolist.txt"))
            {
                Add(line);
            }
        }
    }
}
