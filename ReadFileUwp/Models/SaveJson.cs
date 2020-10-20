using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFileUwp.Models
{
    class SaveJson

    {
        public static void savedata(object obj, string filename)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            TextWriter writer = new StreamWriter(filename);
            jsonSerializer.Serialize(writer, obj);
            writer.Close();
        }
    }
}
