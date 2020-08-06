using System;
using System.Linq;
using System.Collections.Generic;
using Graus.PlantComponents;
using Graus.Data;
using Graus.XML;

namespace Graus
{
    class Program
    {
        static void Main(/*string[] args*/)
        {
            SignalData.ReadSettings();
            ControllModulesData.ReadSettings();
            ObjectIdentifier.ReadSettings();
            /*
            Unit unit = new Unit("AZO-Test", true);
            PLC plc = unit.AddPLC("Rechner1", "PLC");
            ProcessCell pc = plc.AddProcessCell("MischerBereich");
            EquipmentModule em = pc.AddEquipmentModule("MischerUnoNichtDos","UNo");
            em.AddControllModule("CM_Motor","Motor");
            em.AddControllModule("CM_Valve","huch");
            em.AddControllModule("CM_Valve","huch");
            em.AddControllModule("CM_Motor","huch");
            em.AddControllModule("CM_Valve","huch");
            em.AddControllModule("CM_Valve","huch");
            em = pc.AddEquipmentModule("MischerZwei", "Dos");
            em.AddControllModule("CM_Motor", "Motor");
            em.AddControllModule("CM_Valve", "huch");
            em.AddControllModule("CM_Valve", "huch");
            em.AddControllModule("CM_Motor", "huch");
            em.AddControllModule("CM_Valve", "huch");
            em.AddControllModule("CM_Valve", "huch");
            em = pc.AddEquipmentModule("MischerioTrio", "Tres");
            em.AddControllModule("CM_Motor", "Motor");
            em.AddControllModule("CM_Valve", "huch");
            em.AddControllModule("CM_Valve", "huch");
            em.AddControllModule("CM_Motor", "huch");
            em.AddControllModule("CM_Valve", "huch");
            em.AddControllModule("CM_Valve", "huch");
            em = pc.AddEquipmentModule("Mischer4", "Quadtro");
            em.AddControllModule("CM_Motor", "Motor");
            em.AddControllModule("CM_Valve", "huch");
            em.AddControllModule("CM_Valve", "huch");
            em.AddControllModule("CM_Motor", "huch");
            em.AddControllModule("CM_Valve", "huch");
            em.AddControllModule("CM_Valve", "huch");*/

            Console.WriteLine("Added");
            var reader = new ExelReader("minimum.xlsx");
            new TiaWriter(reader.Read(), @"C:\Users\AZO\Documents\Automation\CM_Openess_Lib\CM_Openess_Lib.al16");
            
        }
    }
}

