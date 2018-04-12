using System;
using System.Numerics;

namespace discretefrouiertransform
{
    public class SampleSegment
    {
        private double[] inputArr;
        private Complex[] freqArr;
        private double[] outputArr;
        private int sampleRate;

        public double[] InputArr
        {
            get { return inputArr; }
            private set { inputArr = value; }
        }

        private Complex[] FreqArr
        {
            get { return freqArr; }
            set { freqArr = value; }
        }
        public double[] OutputArr
        {
            get { return outputArr; }
            private set { outputArr = value; }
        }
        public int SampleRate
        {
            get { return sampleRate; }
            private set { sampleRate = value; }
        }

        /// <summary>
        /// Instantites object of SampleSegment. Takes an array of double's as an argument "inputArr".
        /// Contains several methods, one of which is 
        /// to compute and find the frequency-domain representation of the given input
        /// </summary>
        /// <param name="inputArr">Used to assign</param>
        public SampleSegment(double[] inputArr, int sampleRate)
        {
            InputArr = inputArr;
            SampleRate = sampleRate;
        }

        /// <summary>
        /// Computes the fourier transform (DFT) of the original signal.
        /// </summary>
        public void DiscreteFourierTransform()
        {
            Complex[] freqDomain = new Complex[InputArr.Length];
            for (int k = 0; k < freqDomain.Length; k++) //Outer loop. For each frequency do:
            {
                Complex tempSum = new Complex();
                //Console.WriteLine(tempSum);
                for (int n = 0; n < InputArr.Length; n++) //Inner loop. For each input signal compute:
                {
                    //double angle = 2 * Math.PI * n * k / InputArr.Length;
                    double angle = 2 * Math.PI * k / InputArr.Length * n;
                    tempSum += InputArr[n] * Complex.Exp(new Complex(0.0, -angle));
                }
                freqDomain[k] = new Complex((1.0 / freqDomain.Length) * (tempSum.Real * 2), 1.0 / freqDomain.Length * tempSum.Imaginary * 2);
            }

            FreqArr = freqDomain;
        }

        //public void ComputeDft()
        //{
        //    double[] inreal = InputArr;
        //    double[] inimag = new double[InputArr.Length];
        //    double[] outreal = new double[InputArr.Length];
        //    double[] outimag = new double[InputArr.Length];
        //    Complex[] freqDomain = new Complex[InputArr.Length];
        //    int n = inreal.Length;
        //    for (int k = 0; k < n; k++)
        //    {
        //        // For each output element
        //        double sumreal = 0;
        //        double sumimag = 0;
        //        for (int t = 0; t < n; t++)
        //        {
        //            // For each input element
        //            double angle = 2 * Math.PI * t * k / n;
        //            sumreal += inreal[t] * Math.Cos(angle) + inimag[t] * Math.Sin(angle);
        //            sumimag += -inreal[t] * Math.Sin(angle) + inimag[t] * Math.Cos(angle);
        //        }

        //        outreal[k] = sumreal;
        //        outimag[k] = sumimag;


        //        freqDomain[k] = new Complex(sumreal, sumimag);
        //    }


        //    //Used to give sensible numbers
        //    for (int i = 0; i < freqDomain.Length; i++)
        //    {
        //        freqDomain[i] = new Complex((1.0/freqDomain.Length)*(freqDomain[i].Real*2), 1.0 /freqDomain.Length * freqDomain[i].Imaginary*2);
        //    }

        //    FreqArr = freqDomain;
        //}

        /// <summary>
        /// Computes the inverse fourier transform (IFFT) of the frequency-domain.
        /// </summary>
        public void InverseFourierTransform()
        {
            double[] output = new double[InputArr.Length];
            for (int i = 0; i < output.Length; i++)
            {
                Complex tempsum = new Complex();
                for (int j = 0; j < FreqArr.Length; j++)
                {
                    double angle = (double)((2 * Math.PI) * j / output.Length) * i;
                    tempsum += FreqArr[j] * Complex.Exp(new Complex(0, angle));
                }
                tempsum = new Complex(tempsum.Real / (1.0 / output.Length) / 2, tempsum.Imaginary / 1.0 / output.Length / 2);
                output[i] = 1.0 / output.Length * (tempsum.Imaginary + tempsum.Real);
            }

            OutputArr = output;
        }

        /// <summary>
        /// Multiplies the frequency domain with a phasor to shift the signal in the time domain
        /// </summary>
        /// <param name="ms">Amount of time to shift signal in the time-domain</param>
        public void MutiplyWithPhasor(double s)
        {
            for (int i = 0; i < FreqArr.Length; i++)
            {
                double angle = ((2 * Math.PI) / FreqArr.Length) * SampleRate * i * s;
                FreqArr[i] = FreqArr[i] * Complex.Exp(new Complex(0, -angle));
            }
        }

        /// <summary>
        /// Prints the frequency array. E.g., the fft of the original time-domain signal
        /// </summary>
        public void PrintFreqArrays()
        {
            Console.WriteLine();
            Console.WriteLine();
            for (int j = 0; j < FreqArr.Length/2; j++)
            {
                if (FreqArr[j].Magnitude > 0)
                {
                    Console.Write("Frequency: " + (double)j / FreqArr.Length * SampleRate);
                    Console.Write(", Magnitude: " + FreqArr[j].Magnitude + ", Real: " + FreqArr[j].Real + ", " + "Imag: " + FreqArr[j].Imaginary);
                    Console.Write("\n");
                }

            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Prints the input array. E.g., the original time-domain signal
        /// </summary>
        public void PrintInputArray()
        {
            Console.WriteLine();
            Console.Write("Input Array: [");
            for (int i = 0; i < InputArr.Length; i++)
            {
                Console.Write(InputArr[i]);
                if (i != InputArr.Length - 1)
                {
                    Console.Write(", ");
                }
            }
            Console.WriteLine("]");
        }

        /// <summary>
        /// Prints the output array. E.g., the signal computed from conducting IFFT on the frequency-domain signal
        /// </summary>
        public void PrintOutputArray()
        {
            Console.Write("Output array [");
            for (int i = 0; i < OutputArr.Length; i++)
            {
                Console.Write(OutputArr[i]);
                if (i != OutputArr.Length - 1)
                {
                    Console.Write(", ");
                }
            }
            Console.WriteLine("]");
        }
    }
}