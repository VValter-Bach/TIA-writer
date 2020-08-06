using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graus.Data
{
    /// <summary>
    /// Class for Equipment modules members the Controllmodules to have a function
    /// </summary>
    class EMCMF
    {
        public struct CMF
        {
            public string CM;
            public string Function;

            public CMF(string cm, string function)
            {
                CM = cm;
                Function = function;
            }
        }

        public static Dictionary<string, IList<CMF>> emf;


        public static void ReadSettings()
        {
            emf = new Dictionary<string, IList<CMF>>();
            string filename = "EMCMF.csv";
            if (!File.Exists(filename)) throw new FileNotFoundException("Could not locate File: " + filename);
            var reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("TODO")) continue;
                var values = line.Split(';');
                string name = values[0];
                IList<CMF> cmfs = new List<CMF>();
                for (int i = 1; i < values.Length; i++)
                {
                    var cmf = new CMF(values[i++], values[i]);
                    cmfs.Add(cmf);   
                }
                emf.Add(name, cmfs);
            }
        }
    }
}
