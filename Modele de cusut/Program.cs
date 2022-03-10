using Modele_de_cusut.Classes;
using Modele_de_cusut.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modele_de_cusut
{
    public static class Program
    {
        public enum Align { Left, Top, Both };
        public static void Center(this Control ctrlToCenter, Align align = Align.Both)
        {
            if(align!=Align.Top) ctrlToCenter.Left = (ctrlToCenter.Parent.Width - ctrlToCenter.Width) / 2;
            if(align!=Align.Left) ctrlToCenter.Top = (ctrlToCenter.Parent.Height - ctrlToCenter.Height) / 2;
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            start:
            try
            {
                StreamReader fin = new StreamReader(Environment.GetEnvironmentVariable("ProgramData") + "\\Model Manager\\BaseDirectoryPath.txt");
                Manager.BaseDirectoryPath = fin.ReadLine();
                fin.Close();
            }
            catch(Exception ex)
            {
                if(MessageBox.Show("Atentie!\nCreeati in folderul ProgramData fisierul \\Model Manager\\BaseDirectoryPath.txt")==DialogResult.OK)
                    Application.Exit();
            }
            ToDoList.Load();
            ToDoList.Do();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // am schimbat pt test
            Application.Run(new Manager());
            ToDoList.Save();
        }
    }
}
