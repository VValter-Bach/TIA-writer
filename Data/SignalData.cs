using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Graus.Data
{
    class SignalData
    {

        public static void ReadSettings()
        {
            Signals = new Dictionary<string, Signal>();
            string filename = "SignalData.csv";
            if (!File.Exists(filename)) throw new FileNotFoundException("Could not locate File: " + filename);
            var reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("TODO")) continue;
                var values = line.Split(';');
                Signals.Add(values[0], new Signal(values[1], values[3], values[2][0]));
            }
        }

        public static Dictionary<string, Signal> Signals;

        public struct Signal
        {
            public Signal(string ending, string datatype, char sign)
            {
                Ending = ending;
                Datatype = datatype;
                Sign = sign;
            }
            public string Ending;
            public string Datatype;
            public char Sign;
        }
    }
}
