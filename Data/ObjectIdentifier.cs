using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graus.Data
{
    class ObjectIdentifier
    {
        public static void ReadSettings()
        {
            Identifier = new Dictionary<string, string>();
            string filename = "ObjectIdentifier.csv";
            if (!File.Exists(filename)) throw new FileNotFoundException("Could not locate File: " + filename);
            var reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("TODO")) continue;
                var values = line.Split(';');
                Identifier.Add(values[0], values[1]);
            }

        }


        public static Dictionary<string, string> Identifier;
    }
}
