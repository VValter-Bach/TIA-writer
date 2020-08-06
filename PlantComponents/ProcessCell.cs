using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graus.PlantComponents.IOs;

namespace Graus.PlantComponents
{
    class ProcessCell
    {
        /// <summary>
        /// Plc Unit which controlls this module
        /// </summary>
        public PLC Parent;
        /// <summary>
        /// List of inferior equipment modules
        /// </summary>
        public IList<EquipmentModule> EquipmentModules;
        /// <summary>
        /// Name of the Process Cell
        /// </summary>
        public string Name;
        /// <summary>
        /// The Number of the Process Cell
        /// </summary>
        public int ID;

        public ProcessCell(string name, int listLength, PLC parent)
        {
            Name = name;
            Parent = parent;
            ID = listLength + 1;
            EquipmentModules = new List<EquipmentModule>();
        }

        public EquipmentModule AddEquipmentModule(string name, string ioControllerName,string type)
        {
            EquipmentModule equipmentModule = new EquipmentModule(name, EquipmentModules.Count, this, ioControllerName, type);
            EquipmentModules.Add(equipmentModule);
            return equipmentModule;
        }

        public string GetName()
        {
            return String.Format("{0:D2}_{1:D2}", Parent.ID, ID);
        }
    }
}
