using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Graus.XML
{
    class TiaXmlReader
    {
        public static Root ReadXml(string filename, string type, string name)
        {
            Root root = null;
            Stack<XmlObject> objects = new Stack<XmlObject>();
            XmlReader reader = new XmlTextReader(filename);
            while (reader.Read())
            {
                switch (reader.Name)
                {
                    case "Interface":
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            root = new Root(type, name);
                            var obj = new XmlObject("Interface", reader.IsEmptyElement);
                            root.Obj = obj;
                            objects.Push(obj);
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            reader.Close();
                            return root;
                        }
                        else throw new Exception("Dieses Typ interface net möglich tstststs.");
                        break;

                    case "Sections":
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            var obj = new XmlObject("Sections", reader.IsEmptyElement);
                            if (reader.HasAttributes) FillAttribute(reader, obj);
                            objects.Push(objects.Peek().AddXmlObject(obj));
                        }
                        else if(reader.NodeType == XmlNodeType.EndElement)
                        {
                            objects.Pop();
                        }
                        else throw new Exception("Brudaa Sone Sections net gesehen net möglich.");
                        break;

                    case "Section":
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            var isEmpty = reader.IsEmptyElement;
                            var obj = new XmlObject("Section", isEmpty);
                            if (reader.HasAttributes) FillAttribute(reader, obj);
                            if (reader.GetAttribute("Name") == "Temp" || reader.GetAttribute("Name") == "Constant") break;
                            objects.Peek().AddXmlObject(obj);
                            if (!isEmpty) objects.Push(obj);
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            objects.Pop();
                        }
                        else throw new Exception("Pasbleau! Section nichte moglich");
                        break;

                    case "Member":
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            var isEmpty = reader.IsEmptyElement;
                            var obj = new XmlObject("Member", isEmpty);
                            if (reader.HasAttributes) FillAttribute(reader, obj);
                            objects.Peek().AddXmlObject(obj);
                            if (!isEmpty) objects.Push(obj);
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            objects.Pop();
                        }
                        else throw new Exception("Pasbleau");
                        break;

                    case "Comment":
                        var comment = new XmlObject("Comment", reader.IsEmptyElement);
                        if (reader.HasAttributes) FillAttribute(reader, comment);
                        comment.Value = reader.Value;
                        objects.Peek().AddXmlObject(comment);
                        break;
                }
            }
            reader.Close();
            return null;
        }

        private static void FillAttribute(XmlReader reader, XmlObject obj)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "Remanence" || reader.Name == "Accessibility") continue;
                obj.AddAttribute(reader.Name, reader.Value);
            }
        }
    }
}
