using Newtonsoft.Json;
using System.IO;

namespace MachineLearning
{
    public class SampleDatabase
    {
        public DataSample[] database;

        public SampleDatabase(string path)
        {
            if (File.Exists(path))
            {
                string file = File.ReadAllText(path);
                SampleDatabase temp = JsonConvert.DeserializeObject<SampleDatabase>(file);
                database = temp.database;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}
