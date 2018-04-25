using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{
    public class DataSample
    {
        public double[] data;
        public int label;
        public static int labelSize = Enum.GetNames(typeof(Label)).Length;

        public DataSample(double[] data)
        {
            this.data = data;
        }

        public DataSample(double[] data, int label)
        {
            this.data = data;
            this.label = label;
        }
    }

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

    public enum Label
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine
    };
}
