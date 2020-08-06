using Graus.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graus.PlantComponents.IOs;

namespace Graus.PlantComponents
{
    class ControllModule
    {
        public string Type;
        public IList<Tag> Tags;
        public int ID;
        public string Comment;

        public ControllModule(string type, int listLength, EquipmentModule parent, string comment)
        {
            Comment = comment;
            Tags = new List<Tag>();
            ID = listLength + 1;
            var pendant = ControllModulesData.Pendants[type];
            this.Type = pendant.PendantName;
            foreach (var io in pendant.IOs)
            {
                string name = String.Format("{0:D2}_{1:D2}_{2}{3:D2}_{4}", parent.Parent.Parent.ID, parent.Parent.ID, parent.ID, ID, SignalData.Signals[io.Signal].Ending);
                Tags.Add(parent.Parent.Parent.AddIOController(parent.IOControllerName).AddIO(io.Sign, io.Datatype, name, "huch", io.Signal, io.TIAName));
            }
        }
    }
}
