using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graus.XML
{
    class Comment
    {
        public string Lang;
        public string Value;

        public Comment(string lang, string value)
        {
            Lang = lang;
            Value = value;
        }
    }
}
