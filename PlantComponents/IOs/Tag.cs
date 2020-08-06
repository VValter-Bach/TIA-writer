using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graus.PlantComponents.IOs
{
    struct Tag
    {
        public Tag(string datatype, string address, string name, string comment, string signal, string tianame)
        {
            Datatype = datatype;
            Address = address;
            Name = name;
            Comment = comment;
            Signal = signal;
            TIAName = tianame;

        }

        public string Datatype;
        public string Address;
        public string Name;
        public string Comment;
        public string Signal;
        public string TIAName;
    }
}
