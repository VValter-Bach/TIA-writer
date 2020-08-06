using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graus.XML
{
    class Root
    {
        public string Type;
        public string Name;
        public XmlObject Obj;

        public Root(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
