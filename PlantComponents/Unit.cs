using System;
using Graus.PlantComponents.IOs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graus.PlantComponents
{
    class Unit
    {
        public Dictionary<char, int> ByteIndex;
        /// <summary>
        /// true if IoCards should have free slots 
        /// </summary>
        public static bool IoCardFreeSlots;
        /// <summary>
        /// List of Inferior Plcs
        /// </summary>
        public IList<PLC> Plcs;
        /// <summary>
        /// The Name of the Plant
        /// </summary>
        public string Name;

        public Unit(string name, bool freeSlots)
        {
            ByteIndex = new Dictionary<char, int>();
            IoCardFreeSlots = freeSlots;
            Name = name;
            Plcs = new List<PLC>();
        }

        public PLC AddPLC(string name, string type)
        {
            PLC plc = new PLC(name, Plcs.Count, this, type);
            Plcs.Add(plc);
            return plc;
        }

    }
}
