using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modele_de_cusut.Classes
{
    public static class CompareProiects 
    {
        public class ClassCrescData : IComparer<Proiect>
        {
            public int Compare(Proiect a, Proiect b)
            {
                /*if (a.LastAccess < b.LastAccess) return -1;
                else if (a.LastAccess > b.LastAccess) return 1;
                else return 0;*/
                return DateTime.Compare(a.LastAccess, b.LastAccess);
            }
        }

        public class ClassDescrescData : IComparer<Proiect>
        {
            public int Compare(Proiect a, Proiect b)
            {
                /*if (a.LastAccess > b.LastAccess) return -1;
                else if (a.LastAccess < b.LastAccess) return 1;
                else return 0;*/
                return DateTime.Compare(b.LastAccess, a.LastAccess);
            }
        }

        public class ClassCrescNume : IComparer<Proiect>
        {
            public int Compare(Proiect a, Proiect b)
            {
                return string.Compare(a.Nume, b.Nume);
            }
        }

        public class ClassDescrescNume : IComparer<Proiect>
        {
            public int Compare(Proiect a, Proiect b)
            {
                return string.Compare(b.Nume, a.Nume, comparisonType: StringComparison.OrdinalIgnoreCase);
            }
        }

        public static IComparer<Proiect> CrescData = new ClassCrescData();
        public static IComparer<Proiect> DescrescData = new ClassDescrescData();
        public static IComparer<Proiect> CrescNume = new ClassCrescNume();
        public static IComparer<Proiect> DescrescNume = new ClassDescrescNume();
    }
}
