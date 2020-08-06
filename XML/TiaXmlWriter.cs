using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Graus.PlantComponents;
using System.Net;
using System.IO.Packaging;

/// <summary>
/// If it is stupid and it works, it aint stupid
/// </summary>
namespace Graus.XML
{
    class TiaXmlWriter
    {

        private static void WriteObject(XmlWriter writer, XmlObject obj)
        {
            writer.WriteStartElement(obj.ElementName);
            foreach(var attribute in obj.Attributes)
            {
                writer.WriteAttributeString(attribute.Name, attribute.Value);
            }
            if(obj.Value == null) { writer.WriteEndElement(); return; }
            if(obj.Value != "") { writer.WriteValue(obj.Value); writer.WriteEndElement(); return; }
            foreach (var o in obj.Childs) WriteObject(writer, o);
            writer.WriteEndElement();
        }

        public static void WriteDBXML(IList<Root> roots, string filename, string blockname, int number, EquipmentModule em)
        {
            XmlWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
            writer.WriteStartElement("Document");
            writer.WriteStartElement("Engineering");
            writer.WriteAttributeString("version", "V16");
            writer.WriteEndElement();
            writer.WriteStartElement("SW.Blocks.InstanceDB");
            writer.WriteAttributeString("ID", "0");
            writer.WriteStartElement("AttributeList");
            writer.WriteStartElement("Interface");
            writer.WriteStartElement("Sections");
            writer.WriteAttributeString("xmlns", "http://www.siemens.com/automation/Openness/SW/Interface/v4");
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Input");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Output");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "InOut");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Static");
            for (int i = 0; roots != null && i < roots.Count; i++)
            {
                var root = roots[i];
                writer.WriteStartElement("Member");
                writer.WriteAttributeString("Name", root.Name);
                writer.WriteAttributeString("Datatype", "\"" + root.Type + "\"");
                writer.WriteStartElement("Comment");
                writer.WriteStartElement("MultiLanguageText");
                writer.WriteAttributeString("Lang", "de-DE");
                writer.WriteValue(blockname + String.Format("{0:D2}", em.ControllModules[i].ID) + ";" + em.ControllModules[i].Comment);
                writer.WriteEndElement();
                writer.WriteStartElement("MultiLanguageText");
                writer.WriteAttributeString("Lang", "en-US");
                writer.WriteValue(blockname + String.Format("{0:D2}", em.ControllModules[i].ID) + ";" + em.ControllModules[i].Comment);
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("Sections");
                foreach (var obj in root.Obj.Childs.Last().Childs)
                {
                    WriteObject(writer, obj);
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("InstanceOfName");
            writer.WriteValue(blockname.Replace("_DB", string.Empty));
            writer.WriteEndElement();
            writer.WriteStartElement("InstanceOfType");
            writer.WriteValue("FB");
            writer.WriteEndElement();
            writer.WriteStartElement("Name");
            writer.WriteValue(blockname);
            writer.WriteEndElement();
            writer.WriteStartElement("Number");
            writer.WriteValue(number);
            writer.WriteEndElement();
            writer.WriteStartElement("ProgrammingLanguage");
            writer.WriteValue("DB");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();

        }

        public static void WriteFBXML(IList<Root> roots, string filename, string blockname, int number, EquipmentModule em)
        {
            XmlWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
            writer.WriteStartElement("Document");
            writer.WriteStartElement("Engineering");
            writer.WriteAttributeString("version", "V16");
            writer.WriteEndElement();
            writer.WriteStartElement("SW.Blocks.FB");
            writer.WriteAttributeString("ID", "0");
            writer.WriteStartElement("AttributeList");
            writer.WriteStartElement("Interface");
            writer.WriteStartElement("Sections");
            writer.WriteAttributeString("xmlns", "http://www.siemens.com/automation/Openness/SW/Interface/v4");
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Input");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Output");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "InOut");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Static");
            for (int i = 0; i < roots.Count; i++)
            {
                var root = roots[i];
                writer.WriteStartElement("Member");
                writer.WriteAttributeString("Name", root.Name);
                writer.WriteAttributeString("Datatype", "\""+ root.Type +"\"");
                writer.WriteStartElement("Comment");
                writer.WriteStartElement("MultiLanguageText");
                writer.WriteAttributeString("Lang", "de-DE");
                writer.WriteValue(blockname + String.Format("{0:D2}", em.ControllModules[i].ID) + ";" + em.ControllModules[i].Comment);
                writer.WriteEndElement();
                writer.WriteStartElement("MultiLanguageText");
                writer.WriteAttributeString("Lang", "en-US");
                writer.WriteValue(blockname + String.Format("{0:D2}", em.ControllModules[i].ID) + ";" + em.ControllModules[i].Comment);
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("Sections");
                foreach (var obj in root.Obj.Childs.Last().Childs) 
                {
                    WriteObject(writer, obj);
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("Name");
            writer.WriteValue(blockname);
            writer.WriteEndElement();
            writer.WriteStartElement("Number");
            writer.WriteValue(number);
            writer.WriteEndElement();
            writer.WriteStartElement("ProgrammingLanguage");
            writer.WriteValue("FBD");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("ObjectList");
            int id = 1;
            for(int i = 0; i < roots.Count; i++)
            {
                List<string> inputs = SearchIOs(roots[i], "Input");
                List<string> outputs = SearchIOs(roots[i], "Output");
                int uid = 10;
                writer.WriteStartElement("SW.Blocks.CompileUnit");
                writer.WriteAttributeString("ID", id++.ToString());
                writer.WriteAttributeString("CompositionName", "CompileUnits");
                writer.WriteStartElement("AttributeList");
                writer.WriteStartElement("NetworkSource");
                writer.WriteStartElement("StructuredText");
                writer.WriteAttributeString("xmlns", "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v3");
                writer.WriteStartElement("Access");
                writer.WriteAttributeString("Scope", "Call");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteStartElement("CallInfo");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteAttributeString("BlockType", "FB");
                writer.WriteAttributeString("Name", em.ControllModules[i].Type);
                writer.WriteStartElement("Instance");
                writer.WriteAttributeString("Scope", "LocalVariable");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteStartElement("Component");
                writer.WriteAttributeString("Name", roots[i].Name);
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement(); // Component
                writer.WriteEndElement(); // Instance
                writer.WriteStartElement("Token");
                writer.WriteAttributeString("Text", "(");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement(); // Token
                for (int x = 0; x < inputs.Count; x++)
                {
                    writer.WriteStartElement("Parameter");
                    writer.WriteAttributeString("Name", inputs[x]);
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteStartElement("Blank");
                    writer.WriteAttributeString("Num", 1.ToString());
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Blank
                    writer.WriteStartElement("Token");
                    writer.WriteAttributeString("Text", ":=");
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Token
                    writer.WriteStartElement("Blank");
                    writer.WriteAttributeString("Num", 1.ToString());
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Blank
                    writer.WriteStartElement("Access");
                    writer.WriteAttributeString("Scope", "GlobalVariable");
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteStartElement("Symbol");
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteStartElement("Component");
                    writer.WriteAttributeString("Name", FindIO(em.ControllModules[i], inputs[x]));
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Component
                    writer.WriteEndElement(); // Symbol
                    writer.WriteEndElement(); // Access
                    writer.WriteEndElement(); // Parameter
                    if (outputs.Count == 0 && x == inputs.Count - 1) continue;
                    writer.WriteStartElement("Token");
                    writer.WriteAttributeString("Text", ",");
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Token
                    writer.WriteStartElement("NewLine");
                    writer.WriteAttributeString("Num", 1.ToString());
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // NewLine
                    writer.WriteStartElement("Blank");
                    writer.WriteAttributeString("Num", 4.ToString());
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Blank
                }
                for (int x = 0; x < outputs.Count; x++)
                {
                    writer.WriteStartElement("Parameter");
                    writer.WriteAttributeString("Name", outputs[x]);
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteStartElement("Blank");
                    writer.WriteAttributeString("Num", 1.ToString());
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Blank
                    writer.WriteStartElement("Token");
                    writer.WriteAttributeString("Text", ":=");
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Token
                    writer.WriteStartElement("Blank");
                    writer.WriteAttributeString("Num", 1.ToString());
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Blank
                    writer.WriteStartElement("Access");
                    writer.WriteAttributeString("Scope", "GlobalVariable");
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteStartElement("Symbol");
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteStartElement("Component");
                    writer.WriteAttributeString("Name", FindIO(em.ControllModules[i], outputs[x]));
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Component
                    writer.WriteEndElement(); // Symbol
                    writer.WriteEndElement(); // Access
                    writer.WriteEndElement(); // Parameter
                    if (x == outputs.Count - 1) continue;
                    writer.WriteStartElement("Token");
                    writer.WriteAttributeString("Text", ",");
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Token
                    writer.WriteStartElement("NewLine");
                    writer.WriteAttributeString("Num", 1.ToString());
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // NewLine
                    writer.WriteStartElement("Blank");
                    writer.WriteAttributeString("Num", 4.ToString());
                    writer.WriteAttributeString("UId", uid++.ToString());
                    writer.WriteEndElement(); // Blank
                }
                writer.WriteStartElement("Token");
                writer.WriteAttributeString("Text", ")");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement(); // Token
                writer.WriteEndElement(); // CallInfo
                writer.WriteEndElement(); // Access
                writer.WriteStartElement("Token");
                writer.WriteAttributeString("Text", ";");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement(); // Token
                writer.WriteEndElement(); // StructuredText
                writer.WriteEndElement(); // NetworkSource
                writer.WriteStartElement("ProgrammingLanguage");
                writer.WriteValue("SCL");
                writer.WriteEndElement(); // ProgrammingLanguage
                writer.WriteEndElement(); // AttributeList
                writer.WriteEndElement(); // SW.Blocks.CompileUnit

            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();
        }

        public static void WriteFBXML(string filename, string blockname, int number, ProcessCell pc)
        {
            int id = 1;
            int uid = 20;
            XmlWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
            writer.WriteStartElement("Document");
            writer.WriteStartElement("Engineering");
            writer.WriteAttributeString("version", "V16");
            writer.WriteEndElement();
            writer.WriteStartElement("SW.Blocks.FB");
            writer.WriteAttributeString("ID", "0");
            writer.WriteStartElement("AttributeList");
            writer.WriteStartElement("Interface");
            writer.WriteStartElement("Sections");
            writer.WriteAttributeString("xmlns", "http://www.siemens.com/automation/Openness/SW/Interface/v4");
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Input");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Output");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "InOut");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Static");
            writer.WriteEndElement();
            writer.WriteEndElement(); //Sections
            writer.WriteEndElement(); //Interface
            writer.WriteStartElement("Name");
            writer.WriteValue(blockname);
            writer.WriteEndElement();
            writer.WriteStartElement("Number");
            writer.WriteValue(number);
            writer.WriteEndElement();
            writer.WriteStartElement("ProgrammingLanguage");
            writer.WriteValue("FBD");
            writer.WriteEndElement(); //AttributeList
            writer.WriteEndElement();
            writer.WriteStartElement("ObjectList");
            writer.WriteStartElement("SW.Blocks.CompileUnit");
            writer.WriteAttributeString("ID", id++.ToString());
            writer.WriteAttributeString("CompositionName", "CompileUnits");
            writer.WriteStartElement("AttributeList");
            writer.WriteStartElement("NetworkSource");
            writer.WriteStartElement("StructuredText");
            writer.WriteAttributeString("xmlns", "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v3");

            foreach (var em  in pc.EquipmentModules)
            {
                writer.WriteStartElement("Access");
                writer.WriteAttributeString("Scope", "Call");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteStartElement("CallInfo");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteAttributeString("BlockType", "FB");
                writer.WriteAttributeString("Name", em.GetName());
                writer.WriteStartElement("Instance");
                writer.WriteAttributeString("Scope", "GlobalVariable");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteStartElement("Component");
                writer.WriteAttributeString("Name", em.GetName() + "_DB");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement(); //Instance
                writer.WriteStartElement("Token");
                writer.WriteAttributeString("Text", "(");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("Token");
                writer.WriteAttributeString("Text", ")");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement(); //CallInfo
                writer.WriteEndElement(); //Access
                writer.WriteStartElement("Token");
                writer.WriteAttributeString("Text", ";");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("NewLine");
                writer.WriteAttributeString("Num", "1");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); //StructuredText
            writer.WriteEndElement(); //NetworkSource
            writer.WriteStartElement("ProgrammingLanguage");
            writer.WriteValue("SCL");
            writer.WriteEndElement(); //AttributeList
            writer.WriteEndElement(); //SW.Blocks.CompileUnit
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();
        }

        public static void WriteFBXML(string filename, string blockname, int number, PLC plc)
        {
            int id = 1;
            int uid = 20;
            XmlWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
            writer.WriteStartElement("Document");
            writer.WriteStartElement("Engineering");
            writer.WriteAttributeString("version", "V16");
            writer.WriteEndElement();
            writer.WriteStartElement("SW.Blocks.FB");
            writer.WriteAttributeString("ID", "0");
            writer.WriteStartElement("AttributeList");
            writer.WriteStartElement("Interface");
            writer.WriteStartElement("Sections");
            writer.WriteAttributeString("xmlns", "http://www.siemens.com/automation/Openness/SW/Interface/v4");
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Input");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Output");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "InOut");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Static");
            writer.WriteEndElement();
            writer.WriteEndElement(); //Sections
            writer.WriteEndElement(); //Interface
            writer.WriteStartElement("Name");
            writer.WriteValue(blockname);
            writer.WriteEndElement();
            writer.WriteStartElement("Number");
            writer.WriteValue(number);
            writer.WriteEndElement();
            writer.WriteStartElement("ProgrammingLanguage");
            writer.WriteValue("FBD");
            writer.WriteEndElement(); //AttributeList
            writer.WriteEndElement();
            writer.WriteStartElement("ObjectList");
            writer.WriteStartElement("SW.Blocks.CompileUnit");
            writer.WriteAttributeString("ID", id++.ToString());
            writer.WriteAttributeString("CompositionName", "CompileUnits");
            writer.WriteStartElement("AttributeList");
            writer.WriteStartElement("NetworkSource");
            writer.WriteStartElement("StructuredText");
            writer.WriteAttributeString("xmlns", "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v3");

            foreach (var pc in plc.ProcessCells)
            {
                writer.WriteStartElement("Access");
                writer.WriteAttributeString("Scope", "Call");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteStartElement("CallInfo");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteAttributeString("BlockType", "FB");
                writer.WriteAttributeString("Name", pc.GetName());
                writer.WriteStartElement("Instance");
                writer.WriteAttributeString("Scope", "GlobalVariable");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteStartElement("Component");
                writer.WriteAttributeString("Name", pc.GetName() + "_DB");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement(); //Instance
                writer.WriteStartElement("Token");
                writer.WriteAttributeString("Text", "(");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("Token");
                writer.WriteAttributeString("Text", ")");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement(); //CallInfo
                writer.WriteEndElement(); //Access
                writer.WriteStartElement("Token");
                writer.WriteAttributeString("Text", ";");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("NewLine");
                writer.WriteAttributeString("Num", "1");
                writer.WriteAttributeString("UId", uid++.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); //StructuredText
            writer.WriteEndElement(); //NetworkSource
            writer.WriteStartElement("ProgrammingLanguage");
            writer.WriteValue("SCL");
            writer.WriteEndElement(); //AttributeList
            writer.WriteEndElement(); //SW.Blocks.CompileUnit
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();

        }

        public static void WriteMainFBXML(string filename, int ID)
        {
            int id = 1;
            int uid = 20;
            XmlWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
            writer.WriteStartElement("Document");
            writer.WriteStartElement("Engineering");
            writer.WriteAttributeString("version", "V16");
            writer.WriteEndElement();
            writer.WriteStartElement("SW.Blocks.OB");
            writer.WriteAttributeString("ID", "0");
            writer.WriteStartElement("AttributeList");
            writer.WriteStartElement("Interface");
            writer.WriteStartElement("Sections");
            writer.WriteAttributeString("xmlns", "http://www.siemens.com/automation/Openness/SW/Interface/v4");
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Input");
            writer.WriteStartElement("Member");
            writer.WriteAttributeString("Name", "Initial_Call");
            writer.WriteAttributeString("Datatype", "Bool");
            writer.WriteAttributeString("Informative", "true");
            writer.WriteEndElement();
            writer.WriteStartElement("Member");
            writer.WriteAttributeString("Name", "Remanence");
            writer.WriteAttributeString("Datatype", "Bool");
            writer.WriteAttributeString("Informative", "true");
            writer.WriteEndElement();
            writer.WriteEndElement();//Section Input
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Temp");
            writer.WriteEndElement();
            writer.WriteStartElement("Section");
            writer.WriteAttributeString("Name", "Constant");
            writer.WriteEndElement();
            writer.WriteEndElement(); //Sections
            writer.WriteEndElement(); //Interface
            writer.WriteStartElement("Name");
            writer.WriteValue("Main");
            writer.WriteEndElement();
            writer.WriteStartElement("Number");
            writer.WriteValue(1);
            writer.WriteEndElement();
            writer.WriteStartElement("ProgrammingLanguage");
            writer.WriteValue("LAD");
            writer.WriteEndElement();
            writer.WriteStartElement("SecondaryType");
            writer.WriteValue("ProgramCycle");
            writer.WriteEndElement();
            writer.WriteEndElement(); //AttributeList
            writer.WriteStartElement("ObjectList");
            writer.WriteStartElement("SW.Blocks.CompileUnit");
            writer.WriteAttributeString("ID", id++.ToString());
            writer.WriteAttributeString("CompositionName", "CompileUnits");
            writer.WriteStartElement("AttributeList");
            writer.WriteStartElement("NetworkSource");
            writer.WriteStartElement("StructuredText");
            writer.WriteAttributeString("xmlns", "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v3");

            writer.WriteStartElement("Access");
            writer.WriteAttributeString("Scope", "Call");
            writer.WriteAttributeString("UId", uid++.ToString());
            writer.WriteStartElement("CallInfo");
            writer.WriteAttributeString("UId", uid++.ToString());
            writer.WriteAttributeString("BlockType", "FB");
            writer.WriteAttributeString("Name", String.Format("{0:D2}", ID));
            writer.WriteStartElement("Instance");
            writer.WriteAttributeString("Scope", "GlobalVariable");
            writer.WriteAttributeString("UId", uid++.ToString());
            writer.WriteStartElement("Component");
            writer.WriteAttributeString("Name", String.Format("{0:D2}_DB", ID));
            writer.WriteAttributeString("UId", uid++.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement(); //Instance
            writer.WriteStartElement("Token");
            writer.WriteAttributeString("Text", "(");
            writer.WriteAttributeString("UId", uid++.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Token");
            writer.WriteAttributeString("Text", ")");
            writer.WriteAttributeString("UId", uid++.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement(); //CallInfo
            writer.WriteEndElement(); //Access
            writer.WriteStartElement("Token");
            writer.WriteAttributeString("Text", ";");
            writer.WriteAttributeString("UId", uid++.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("NewLine");
            writer.WriteAttributeString("Num", "1");
            writer.WriteAttributeString("UId", uid++.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement(); //StructuredText
            writer.WriteEndElement(); //NetworkSource
            writer.WriteStartElement("ProgrammingLanguage");
            writer.WriteValue("SCL");
            writer.WriteEndElement(); //AttributeList
            writer.WriteEndElement(); //SW.Blocks.CompileUnit
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();

        }

        private static List<string> SearchIOs(Root root, string sectionname)
        {
            List<string> names = new List<string>();
            XmlObject input = null;
            foreach(var obj in root.Obj.Childs.Last().Childs)
            {
                foreach(var attr in obj.Attributes)
                {
                    if (attr.Name == "Name" && attr.Value == sectionname) input = obj;
                }
                if (input != null) break;
            }
            foreach(var item in input.Childs)
            {
                foreach(var attr in item.Attributes)
                {
                    if (attr.Name == "Name") names.Add(attr.Value);
                }
            }
            return names;
        }

        private static string FindIO(ControllModule cm, string tianame)
        {
            foreach (var io in cm.Tags)
            {
                if (io.TIAName == tianame) return io.Name;
            }
            throw new Exception("Nicht gefunden " + tianame + " in " + cm.Comment);
        }
    }
}
