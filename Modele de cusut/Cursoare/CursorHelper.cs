using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class CursorHelper
{
    public static Cursor FromByteArray(byte[] array)
    {
        using (MemoryStream memoryStream = new MemoryStream(array))
        {
            return new Cursor(memoryStream);
        }
    }
}