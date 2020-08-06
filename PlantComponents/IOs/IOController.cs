using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graus.PlantComponents;
using Graus;

namespace Graus.PlantComponents.IOs
{
    class IOController
    {
        public string Name;
        private readonly Unit parent;
        public IList<IOCard> IOCards;

        public IOController(Unit parent, string name)
        {
            IOCards = new List<IOCard>();
            Name = name;
            this.parent = parent;
        }

        public Tag AddIO(char sign, string datatype, string name, string comment, string signal, string tianame)
        {
            string address;
            Tag tag;

            foreach (var card in IOCards)
            {
                if (card.Sign != sign) continue;
                if (card.MaxBit == card.BitPosition) continue;
                address = String.Format("%{0}{1}.{2}", sign, card.GlobalPosition, card.BitPosition++);
                tag = new Tag(datatype, address, name, comment, signal, tianame);
                card.tags.Add(tag);
                return tag;
            }
            IOCard iocard = AddIOCard(sign);
            address = String.Format("%{0}{1}.{2}", sign, iocard.GlobalPosition, iocard.BitPosition++);
            tag = new Tag(datatype, address, name, comment, signal, tianame);
            iocard.tags.Add(tag);
            return tag;
        }

        public IOCard AddIOCard(char sign, int size = 8)
        {
            string identifier = "";
            switch (sign)
            {
                case 'Q':
                case 'I':
                    identifier = String.Format("D{0}{1}", sign, size);
                    break;
            }
            IOCard io = new IOCard(identifier, sign, size, GetCardIndex(sign));
            IOCards.Add(io);
            return io;
        }
        private int GetCardIndex(char sign)
        {
            if (parent.ByteIndex.ContainsKey(sign)) return parent.ByteIndex[sign]++;
            else parent.ByteIndex.Add(sign, 0);
            return parent.ByteIndex[sign]++;
        }
    }
}
