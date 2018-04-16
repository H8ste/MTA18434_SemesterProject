using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{
    public static class ActivationFunctions
    {
        /// <summary>
        /// The Sigmoid function takes in a value x which it squishes inbetween the range of 0-1. 
        /// X can be from negative NaN to positive NaN.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Sigmoid(double x)
        {
            return 1 / (float)(1 + Math.Exp(-x));
        }

        public static float ReLu()
        {
            return 0;
        }
    }
}
