using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dialogowe.BazaDanych
{
    class DodawanieZdjec
    {
        public byte[] dodajZdjecie(string URIzdjecie)
        {
            byte[] b = File.ReadAllBytes("..\\..\\Obrazy\\" + URIzdjecie);
            return b;
        }
    }
}
