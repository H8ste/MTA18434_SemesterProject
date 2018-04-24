using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{
    public class DataSample
    {
        public float[] data;
        public int label;
        public static int labelSize = Enum.GetNames(typeof(Label)).Length;

        public DataSample(float[] data, Label label)
        {
            this.data = data;
            this.label = (int)label;
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
