using Newtonsoft.Json;
using ReadFileUwp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibraries
{
    public static class JsonService
    {
        public static void WriteToJsonFileCorrect(string filepath, Persons person)
        {
            try
            {
                using StreamReader reader = new StreamReader(filepath);
                var json = reader.ReadToEnd();
                reader.Close();

                if (json != string.Empty)
                {
                    var list = JsonConvert.DeserializeObject<List<Persons>>(json);
                    list.Add(new Persons { });

                    var json2 = JsonConvert.SerializeObject(list);

                    using StreamWriter writer = new StreamWriter(@$"C:\Users\Micke\Desktop\EC-WIN20\Files\persons.json");

                    writer.Write(json2);
                    writer.Close();
                }

            }
            catch
            {
                using StreamWriter writer = new StreamWriter(filepath);
                var list = new List<Persons>() { person };
                var json = JsonConvert.SerializeObject(list);

                writer.Write(json);
                writer.Close();
            }

        }
    }

}

