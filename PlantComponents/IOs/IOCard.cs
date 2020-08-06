using Siemens.Engineering.Hmi.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graus.PlantComponents.IOs
{
    class IOCard
    {
        public int MaxBit, BitPosition, GlobalPosition;
        public char Sign;
        public string Identifier;
        public IList<Tag> tags;


        public IOCard(string identifier, char sign, int size, int globalPosition)
        {
            GlobalPosition = globalPosition;
            Identifier = identifier;
            Sign = sign;
            MaxBit = size;
            tags = new List<Tag>(size);
            if (Unit.IoCardFreeSlots && size > 4) BitPosition = 2;
            else if (Unit.IoCardFreeSlots && size > 2) BitPosition = 1;
            else BitPosition = 0;
        }
    }
}
