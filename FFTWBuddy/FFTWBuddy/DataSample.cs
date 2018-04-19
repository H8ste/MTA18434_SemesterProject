using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFTWBuddy
{
    public class DataSample
    {
        public double[] data;
        public int label;
        public static int labelSize;

        public DataSample(double[] data)
        {
            this.data = data;
            labelSize = Enum.GetNames(typeof(Label)).Length;
        }

        public DataSample(double[] data, Label label)
        {
            this.data = data;
            this.label = (int)label;
            labelSize = Enum.GetNames(typeof(Label)).Length;
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
