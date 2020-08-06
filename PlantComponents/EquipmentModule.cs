using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graus.PlantComponents.IOs;
namespace Graus.PlantComponents
{
    class EquipmentModule
    {
        /// <summary>
        /// The ProcessCell which contains this equiptment module
        /// </summary>
        public ProcessCell Parent;
        /// <summary>
        /// List of inferior ControllModules
        /// </summary>
        public IList<ControllModule> ControllModules;
        /// <summary>
        /// Name of this ControllModules
        /// </summary>
        public string Name;
        /// <summary>
        /// the name of the according ioController
        /// </summary>
        public string IOControllerName;
        /// <summary>
        /// The Number of the Process Cell
        /// </summary>
        public int ID;
        public string Type;

        public EquipmentModule(string name, int listLength, ProcessCell parent, string ioControllerName,string type)
        {
            Type = type;
            Name = name;
            ID = listLength + 1;
            Parent = parent;
            ControllModules = new List<ControllModule>();
            IOControllerName = ioControllerName;
            Parent.Parent.IOs.Add(ioControllerName);
        }

        public ControllModule AddControllModule(string type, string comment)
        {
            ControllModule controllModule = new ControllModule(type, ControllModules.Count, this, comment);
            ControllModules.Add(controllModule);
            return controllModule;
        }

        public string GetName()
        {
            return String.Format("{0:D2}_{1:D2}_{2:D2}", Parent.Parent.ID, Parent.ID, ID);
        }
    }
}
