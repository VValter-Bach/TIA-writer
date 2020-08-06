using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Graus.XML
{
    class XmlObject
    {
        public IList<Attribute> Attributes;
        public IList<XmlObject> Childs;
        public string ElementName;
        public string Value;

        public XmlObject(string name, bool isEmpty)
        {
            ElementName = name;
            Value = isEmpty ? null : "";
            Attributes = new List<Attribute>();
            Childs = new List<XmlObject>();
        }

        public XmlObject AddXmlObject(string name, bool isEmpty)
        {
            var obj = new XmlObject(name, isEmpty);
            Childs.Add(obj);
            return obj;
        }

        public XmlObject AddXmlObject(XmlObject obj)
        {
            Childs.Add(obj);
            return obj;
        }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(new Attribute(name, value));
        }

        public struct Attribute
        {
            public string Name;
            public string Value;

            public Attribute(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}
