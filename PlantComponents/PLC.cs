using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graus.PlantComponents.IOs;

namespace Graus.PlantComponents
{
    class PLC
    {
        /// <summary>
        /// List of extern iocontrollers
        /// </summary>
        public IList<IOController> IOControllers;
        /// <summary>
        /// The Plant-Unit which contains everything
        /// </summary>
        public Unit Parent;
        /// <summary>
        /// The Name of the Plc
        /// </summary>
        public string Name;
        /// <summary>
        /// The Type of the PLC (ObjectIdentifier.csv)
        /// </summary>
        public string Type;
        /// <summary>
        /// List of inferior ProcessCells
        /// </summary>
        public IList<ProcessCell> ProcessCells;
        /// <summary>
        /// The Number of the Plc
        /// </summary>
        public int ID;
        /// <summary>
        /// Used for Generating functionblock numbers in Writer.cs
        /// </summary>
        public int BlockCount;
        /// <summary>
        /// List of IoController 
        /// </summary>
        public IList<string> IOs;


        public PLC(string name, int listLength, Unit parent, string type)
        {
            IOs = new List<string>();
            Name = name;
            Type = type;
            ID = listLength + 1;
            ProcessCells = new List<ProcessCell>();
            Parent = parent;
            BlockCount = 0;
            IOControllers = new List<IOController>();
        }

        public ProcessCell AddProcessCell(string name)
        {
            ProcessCell processCell = new ProcessCell(name, ProcessCells.Count, this);
            ProcessCells.Add(processCell);
            return processCell;
        }

        public IOController AddIOController(string name)
        {
            foreach (var ioController in IOControllers)
            {
                if (ioController.Name == name) return ioController;
            }
            IOController controller = new IOController(this.Parent, name);
            IOControllers.Add(controller);
            return controller;
        }
    }
}
