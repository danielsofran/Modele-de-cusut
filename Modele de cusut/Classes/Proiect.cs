using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Drawing;

namespace Modele_de_cusut.Classes
{
    public class Proiect
    {
        public string Nume { get; set; }
        public string NumeCamasa { get; set; }
        public string Path { get; set; } // path of the directory
        public DateTime LastAccess { get; set; }
        public bool isFav { get; set; }
        public bool isHiddenFromList { get; set; }

        bool img_seted = false;
        public string ImagePath
        {
            get {
                if (!img_seted) return Directory.GetParent(Path).FullName + "\\_Imagini\\" + Nume + ".png";
                else return ImagePath;
            }
            set
            {
                ImagePath = value;
                img_seted = true;
            }
        }

        public void FromFile(string path)
        {
            try
            {
                StreamReader fin = new StreamReader(path);
                Nume = System.IO.Path.GetFileNameWithoutExtension(Directory.GetParent(path).FullName);
                NumeCamasa = fin.ReadLine();
                LastAccess = DateTime.Parse(fin.ReadLine());
                isFav = bool.Parse(fin.ReadLine());
                isHiddenFromList = bool.Parse(fin.ReadLine());
                string imgpath = fin.ReadLine();
                if (imgpath == "-") { img_seted = false; } // default 
                else { img_seted = true; ImagePath = imgpath; }
                FileInfo info = new FileInfo(path);
                Path = info.DirectoryName;
                fin.Close();
            }
            catch (FileNotFoundException ex) { MessageBox.Show("Ne pare rău, dar nu am putut găsi nimciun proiect.", "Eroare", MessageBoxButtons.OKCancel); throw ex; }
        }

        public void FromDirectory(string path)
        {
            Nume = System.IO.Path.GetFileNameWithoutExtension(path);
            string file = path + "\\Proiect.proiect";
            if (File.Exists(file)) FromFile(file);
            else { MessageBox.Show("Ne pare rău, dar nu am putut găsi nimciun proiect.", "Eroare", MessageBoxButtons.OKCancel); throw new FileNotFoundException(); }
        }

        public static List<Proiect> FromBaseDirectory(string path)
        {
            string[] files = Directory.GetFiles(path, "*.proiect", SearchOption.AllDirectories);
            List<Proiect> rez = new List<Proiect>();
            foreach(string file in files)
            {
                Proiect pr = new Proiect();
                pr.FromFile(file);
                rez.Add(pr);
            }
            return rez;
        }

        public void ToFile() /// untested
        {
            string path = Directory.GetParent(Path).FullName + "\\" + Nume;
            try
            {
                using (StreamWriter fout = new StreamWriter(path + "\\Proiect.proiect", false))
                {
                    fout.WriteLine(NumeCamasa);
                    fout.WriteLine(LastAccess);
                    fout.WriteLine(isFav);
                    fout.WriteLine(isHiddenFromList);
                    if (img_seted) fout.WriteLine(ImagePath);
                    else fout.WriteLine("-");
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public void CreateDirectiries()
        {
            Directory.CreateDirectory(Directory.GetParent(Path).FullName + "\\_Imagini");
            Directory.CreateDirectory(Path);
            Directory.CreateDirectory(Path + "\\Proiect");
            Directory.CreateDirectory(Path + "\\Proiect\\Imagini");
            Directory.CreateDirectory(Path + "\\Proiect\\Model");
        }

        public void Save()
        {
            CreateDirectiries();
            ToFile();
        }
    }
}
