using Graus.PlantComponents;
using Graus.PlantComponents.IOs;
using System;
using System.Collections.Generic;
using System.Linq;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.Library;
using Siemens.Engineering.Library.Types;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.Types;
using System.IO;
using Graus.Data;
using Siemens.Engineering.HmiUnified;
using Graus.XML;
using Siemens.Engineering.Compiler;

namespace Graus
{
    class TiaWriter
    {

        static public string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        private readonly Unit unit;
        private TiaPortal portal = null;
        private Project project = null;
        private GlobalLibrary library = null;

        public TiaWriter(Unit unit, string libpath)
        {
            this.unit = unit;
            Writer(libpath);
        }

        private void Writer(string libpath)
        {

            portal = new TiaPortal(TiaPortalMode.WithoutUserInterface);
            Console.WriteLine("TIA-Portal Opened");
            project = portal.Projects.Create(new DirectoryInfo(Path.GetFullPath("C:\\Users\\AZO\\Documents")), unit.Name);
            Console.WriteLine("Empty Project Created");
            library = portal.GlobalLibraries.Open(new FileInfo(libpath), OpenMode.ReadOnly);
            Console.WriteLine("Library Loaded");
            PorjLibWriter();
            Console.WriteLine("Filled Project Library");
            PLCWriter();
            Console.WriteLine("Writing Data To Project");
            project.Save();
            project.Close();
            portal.Dispose();
            Console.WriteLine("Saving and Exiting");
        }

        #region Plc

        private void PLCWriter()
        {
            foreach (var plc in unit.Plcs)
            {
                IoDevices(plc);
                var device = project.Devices.CreateWithItem("OrderNumber:" + ObjectIdentifier.Identifier[plc.Type], plc.Name, plc.Name);
                var software = FindPlcSoftware(device);
                PLCTypeWriter(plc, software);
                TagWriter(plc, software);
                FBWriter(plc, software);
                AddToSubnet(plc, device);
            }
        }

        private void PrintAttribute(IEngineeringObject obj)
        {
            var list = obj.GetAttributeInfos();
            List<string> attrnames = new List<string>();
            foreach(var item in list)
            {
                attrnames.Add(item.Name);
            }
            var attrvalues = obj.GetAttributes(attrnames);
            for(int i = 0; i < attrnames.Count; i++)
            {
                Console.WriteLine(attrnames[i] + "\t:" + attrvalues[i]);
            }
        }

        private PlcBlock FindPlcBlock(PlcBlockGroup group, string name)
        {
            var block = group.Blocks.Find(name);
            if (block != null) return block;
            foreach (var g in group.Groups) return FindPlcBlock(g, name);
            return null;
        }

        private string FindAttribute(DeviceItem device, string attribute)
        {

            var list = device.GetAttributeInfos();
            foreach (var item in list) if (item.Name == attribute) return device.GetAttribute(attribute).ToString();
            return "";
        }

        private NetworkInterface FindNetworkInterface(DeviceItemComposition devices)
        {
            NetworkInterface network = null;
            foreach (var device in devices)
            {
                if (network != null) continue;
                network = device.GetService<NetworkInterface>();
                if (network != null) 
                {
                    if(FindAttribute(device, "InterfaceType") != "Ethernet") network = null;
                    else if (network.Nodes == null) network = null;
                    else if (network.Nodes.Count == 0) network = null;
                }
                if (network == null) network = FindNetworkInterface(device.DeviceItems);
            }
            return network;
        }

        private void CompileBlock(PlcBlock block)
        {
            var c = block.GetService<ICompilable>();
            c.Compile();
        }

        private void AddToSubnet(PLC plc, Device device)
        {
            var network = FindNetworkInterface(device.DeviceItems);
            network.Nodes.Last().ConnectToSubnet(project.Subnets.Find("GlobalNetwork"));
            var controller = network.IoControllers.Last();
            if (controller == null) return;
            var system = controller.CreateIoSystem("GeneralSystem");
            foreach (var dev in project.Devices)
            {
                if (!plc.IOs.Contains(dev.Name)) continue;
                var ni = FindNetworkInterface(dev.DeviceItems);
                ni.IoConnectors.Last().ConnectToIoSystem(system);
            }
        }

        private void All(DeviceItemComposition devices)
        {
            foreach(DeviceItem device in devices)
            {
                Console.WriteLine(device.Name);
                All(device.DeviceItems);
            }
        }

        private void IoDevices(PLC plc)
        {
            IList<Device> devices = new List<Device>();
            var subnet = project.Subnets.Create("System:Subnet.Ethernet", "GlobalNetwork");
            foreach (var controller in plc.IOControllers)
            {
                var device = project.Devices.CreateWithItem("OrderNumber:6ES7 155-6AU30-0CN0/V4.2", controller.Name, controller.Name);
                devices.Add(device);
                FillIOController(device, controller);
                var network = FindNetworkInterface(device.DeviceItems);
                network.Nodes.Last().ConnectToSubnet(subnet);
            }
        }

        private void FillIOController(Device device, IOController controller)
        {
            int index = 0;
            DeviceItem rail = FindRail(device.DeviceItems);
            foreach(IOCard card in controller.IOCards)
            {
                for (int i = index; i < 200; i++)
                {
                    if (!rail.CanPlugNew("OrderNumber:" + ObjectIdentifier.Identifier[card.Identifier], card.Identifier + "_" + card.GlobalPosition, i)) continue;
                    index = i;
                    rail.PlugNew("OrderNumber:" + ObjectIdentifier.Identifier[card.Identifier], card.Identifier + "_" + card.GlobalPosition, i);
                    break;
                }
            }
        }

        private DeviceItem FindRail(DeviceItemComposition devices)
        {
            foreach(var item in devices)
            {
                if (item.Name.Contains("Rack")) return item;
                return FindRail(item.DeviceItems);
            }
            return null;
        }

        private void FBWriter(PLC plc, PlcSoftware software)
        {
            var fi = new FileInfo(Path.GetFullPath(GetGuid() + ".xml"));
            software.GetService<ICompilable>().Compile();
            var root = software.BlockGroup.Groups.Create("Plant");
            foreach (var pc in plc.ProcessCells)
            {
                var folder_pc = root.Groups.Create(pc.Name);
                foreach (var em in pc.EquipmentModules)
                {
                    IList<Root> roots = new List<Root>();
                    //var folder_em = folder_pc.Groups.Create(em.Name);
                    foreach (var cm in em.ControllModules)
                    {
                        var file = new FileInfo(Path.GetFullPath(GetGuid()));
                        var block = FindPlcBlock(software.BlockGroup, cm.Type);
                        block.Export(file, ExportOptions.None);
                        roots.Add(TiaXmlReader.ReadXml(file.FullName, cm.Type, String.Format("{0:D2}_{1:D2}_{2:D2}{3:D2}", plc.ID, pc.ID, em.ID, cm.ID)));
                        File.Delete(file.FullName);
                    }
                    fi = new FileInfo(Path.GetFullPath(GetGuid() + ".xml"));
                    XML.TiaXmlWriter.WriteFBXML(roots, fi.FullName, em.GetName(), plc.BlockCount +=10, em);
                    folder_pc.Blocks.Import(fi, ImportOptions.None);
                    File.Delete(fi.FullName);
                    fi = new FileInfo(Path.GetFullPath(GetGuid() + ".xml"));
                    XML.TiaXmlWriter.WriteDBXML(roots, fi.FullName, em.GetName() + "_DB", plc.BlockCount += 10, em);
                    folder_pc.Blocks.Import(fi, ImportOptions.None);
                    File.Delete(fi.FullName);
                }
                fi = new FileInfo(Path.GetFullPath(GetGuid() + ".xml"));
                XML.TiaXmlWriter.WriteFBXML(fi.FullName, pc.GetName(), plc.BlockCount += 10, pc);
                folder_pc.Blocks.Import(fi, ImportOptions.None);
                File.Delete(fi.FullName);
                fi = new FileInfo(Path.GetFullPath(GetGuid() + ".xml"));
                XML.TiaXmlWriter.WriteDBXML(null, fi.FullName, pc.GetName() + "_DB", plc.BlockCount += 10, null);
                folder_pc.Blocks.Import(fi, ImportOptions.None);
                File.Delete(fi.FullName);
            }
            fi = new FileInfo(Path.GetFullPath(GetGuid() + ".xml"));
            XML.TiaXmlWriter.WriteFBXML(fi.FullName, String.Format("{0:D2}", plc.ID), plc.BlockCount += 10, plc);
            root.Blocks.Import(fi, ImportOptions.None);
            File.Delete(fi.FullName);
            fi = new FileInfo(Path.GetFullPath(GetGuid() + ".xml"));
            XML.TiaXmlWriter.WriteDBXML(null, fi.FullName, String.Format("{0:D2}_DB", plc.ID), plc.BlockCount += 10, null);
            root.Blocks.Import(fi, ImportOptions.None);
            File.Delete(fi.FullName);
            software.BlockGroup.Blocks.Find("Main").Delete();
            fi = new FileInfo(Path.GetFullPath(GetGuid() + ".xml"));
            XML.TiaXmlWriter.WriteMainFBXML(fi.FullName, plc.ID);
            software.BlockGroup.Blocks.Import(fi, ImportOptions.None);
            File.Delete(fi.FullName);
        }

        private void PLCTypeWriter(PLC plc, PlcSoftware software)
        {
            var folder = software.BlockGroup.Groups.Create("Modules");
            var modules = library.TypeFolder.Folders.Find("Blocks").Folders.Find("Modules");
            foreach(var pc in plc.ProcessCells)
            {
                foreach(var em in pc.EquipmentModules)
                {
                    foreach(var cm in em.ControllModules)
                    {
                        if (!(modules.Types.Find(cm.Type) is CodeBlockLibraryType item)) { Console.WriteLine("Skipping " + cm.Type); continue; }
                        CompileBlock(folder.Blocks.CreateFrom((CodeBlockLibraryTypeVersion)item.Versions.Last()));
                    }
                }
            }
        }

        private void TypeGroupIterator(LibraryTypeFolder src, PlcBlockGroup blockDest, PlcTypeGroup typeDest)
        {
            foreach (var t in src.Types)
            {
                if (t is CodeBlockLibraryType block)
                {
                    blockDest.Blocks.CreateFrom((CodeBlockLibraryTypeVersion)block.Versions.Last()).Delete();
                    continue;
                }
                if (t is PlcTypeLibraryType type)
                {
                    try
                    {
                        typeDest.Types.CreateFrom((PlcTypeLibraryTypeVersion)type.Versions.Last()).Delete();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            foreach (var folder in src.Folders) TypeGroupIterator(folder, blockDest, typeDest);
        }

        #endregion

        #region SoftwareUtils

        private HmiSoftware FindHmiSoftware(Device plc)
        {
            foreach (DeviceItem deviceItem in plc.DeviceItems)
            {
                SoftwareContainer softwareContainer = deviceItem.GetService<SoftwareContainer>();
                if (softwareContainer == null) continue;
                return softwareContainer.Software as HmiSoftware;
            }
            throw new NullReferenceException("Could not locate an PlcSoftware");
        }

        private PlcSoftware FindPlcSoftware(Device plc)
        {
            foreach (DeviceItem deviceItem in plc.DeviceItems)
            {
                SoftwareContainer softwareContainer = deviceItem.GetService<SoftwareContainer>();
                if (softwareContainer == null) continue;
                return softwareContainer.Software as PlcSoftware;
            }
            throw new NullReferenceException("Could not locate an PlcSoftware");
        }

        #endregion

        #region LibraryUtils

        private void PorjLibWriter()
        {
            var device = project.Devices.CreateWithItem("OrderNumber:6ES7 516-3AN02-0AB0/V2.8", "WillBeRemoved", "WillBeRemoved");
            var plcSoftware = FindPlcSoftware(device);
            TypeGroupIterator(library.TypeFolder, plcSoftware.BlockGroup as PlcBlockGroup, plcSoftware.TypeGroup as PlcTypeGroup);
            device.Delete();
        }

        #endregion

        #region TagUtils

        private PlcTagTable TagTable(string name, PlcTagTableGroup group)
        {
            PlcTagTable t = group.TagTables.Find(name);
            if (t == null) return group.TagTables.Create(name);
            return t;
        }

        private PlcTagTableUserGroup TagGroup(string name, PlcTagTableGroup group)
        {
            PlcTagTableUserGroup g = group.Groups.Find(name);
            if (g == null) return group.Groups.Create(name);
            return g;
        }

        private void TagWriter(PLC plc, PlcSoftware software)
        {
            foreach (var ctrl in plc.IOControllers)
            {
                var table = TagTable(ctrl.Name, software.TagTableGroup);
                foreach (var io in ctrl.IOCards)
                {
                    foreach(var tag in io.tags)
                    {
                        table.Tags.Create(tag.Name, tag.Datatype, tag.Address);
                    }
                }
            }

            /*
            foreach (var pc in plc.ProcessCells)
            {
                var group = TagGroup(pc.Name, software.TagTableGroup);
                foreach (var em in pc.EquipmentModules)
                {
                    var table = TagTable(em.Name, group);
                    foreach (var cm in em.ControllModules)
                    {
                        foreach (var tag in cm.Tags)
                        {
                            table.Tags.Create(tag.Name, tag.Datatype, tag.Address);
                        }
                    }
                }
            }*/
        }

        #endregion

    }
}
