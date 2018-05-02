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

        public DataSample()
        {
        }

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

    public enum Label
    {
        Blank,
        Noise,
        Hej,
        Siri
    };
}
