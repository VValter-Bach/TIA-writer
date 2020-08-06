using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System;

namespace Graus.Data
{
    class ControllModulesData
    {
        public static void ReadSettings()
        {
            Pendants = new Dictionary<string, ControllModulesPendant>();
            string filename = "ControllModulesData.csv";
            if (!File.Exists(filename)) throw new FileNotFoundException("Could not locate File: " + filename);
            var reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("TODO")) continue;
                var values = line.Split(';');
                Pendants.Add(values[0], new ControllModulesPendant(values.Skip(1).ToArray()));
            }

        }


        public static Dictionary<string, ControllModulesPendant> Pendants;

        public struct ControllModulesPendant
        {
            public ControllModulesPendant(string[] line)
            {
                IOs = new List<IO>();
                PendantName = line[0];
                for (int i = 1; i < line.Length; i++) 
                {
                    SignalData.Signal signal = SignalData.Signals[line[i]];
                    IOs.Add(new IO(line[i++], signal.Datatype, signal.Sign, line[i]));
                }
            }

            public string PendantName;
            public IList<IO> IOs;
        }


        public struct IO
        {
            public string Signal;
            public string Datatype;
            public char Sign;
            public string TIAName;
            public IO(string signal, string datatype, char sign, string tianame)
            {
                Signal = signal;
                Datatype = datatype;
                Sign = sign;
                TIAName = tianame;
            }
        }

    }
}
