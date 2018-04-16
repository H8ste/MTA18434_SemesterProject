using System;
using System.Numerics;

namespace MachineLearning
{
    public static class DSProcess
    {
        /// <summary>
        /// The Fourier Transform returns a Complex array that represents the input array 
        /// in the frequency domain, rather than the time domain
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Complex[] FFT(short[] arr)
        {
            Complex[] freqDomain = new Complex[arr.Length];

            for (int k = 0; k < arr.Length; k++)
            {
                Complex tempSum = 0;
                for (int n = 0; n < arr.Length; n++)
                {
                    double angle = ((2 * Math.PI) * k / arr.Length) * n;
                    tempSum += arr[n] * Complex.Exp(new Complex(0, -angle));
                }

                freqDomain[k] = new Complex((1.0 / freqDomain.Length) * (tempSum.Real * 2), 1.0 / freqDomain.Length * tempSum.Imaginary * 2);
            }

            return freqDomain;
        }

        public static Complex[] FFT(int[] arr)
        {
            Complex[] freqDomain = new Complex[arr.Length];

            for (int k = 0; k < arr.Length; k++)
            {
                Complex tempSum = 0;
                for (int n = 0; n < arr.Length; n++)
                {
                    double angle = ((2 * Math.PI) * k / arr.Length) * n;
                    tempSum += arr[n] * Complex.Exp(new Complex(0, -angle));
                }
                freqDomain[k] = new Complex((1.0 / freqDomain.Length) * (tempSum.Real * 2), 1.0 / freqDomain.Length * tempSum.Imaginary * 2);
            }

            return freqDomain;
        }

        public static Complex[] FFT(float[] arr)
        {
            Complex[] freqDomain = new Complex[arr.Length];

            for (int k = 0; k < arr.Length; k++)
            {
                Complex tempSum = 0;
                for (int n = 0; n < arr.Length; n++)
                {
                    double angle = ((2 * Math.PI) * k / arr.Length) * n;
                    tempSum += arr[n] * Complex.Exp(new Complex(0, -angle));
                }
                freqDomain[k] = new Complex((1.0 / freqDomain.Length) * (tempSum.Real * 2), 1.0 / freqDomain.Length * tempSum.Imaginary * 2);
            }

            return freqDomain;
        }

        /// <summary>
        /// Fourier Transform with zero-padding, the zeropadding amount 
        /// adds x amount of empty spots between 
        /// the values of the input array to the output frequencies
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="zeroPadAmount"></param>
        /// <returns></returns>
        public static Complex[] FFT(short[] arr, int zeroPadAmount)
        {
            Complex[] freqDomain = new Complex[arr.Length * zeroPadAmount];

            for (int k = 0; k < freqDomain.Length; k++)
            {
                Complex tempSum = 0;
                for (int n = 0; n < arr.Length; n++)
                {
                    double angle = ((2 * Math.PI) * k / arr.Length) * n;
                    tempSum += arr[n] * Complex.Exp(new Complex(0, -angle));
                }
                freqDomain[k] = new Complex((1.0 / freqDomain.Length) * (tempSum.Real * 2), 1.0 / freqDomain.Length * tempSum.Imaginary * 2);

            }

            return freqDomain;
        }

        public static Complex[] FFT(int[] arr, int zeroPadAmount)
        {
            Complex[] freqDomain = new Complex[arr.Length * zeroPadAmount];

            for (int k = 0; k < freqDomain.Length; k++)
            {
                Complex tempSum = 0;
                for (int n = 0; n < arr.Length; n++)
                {
                    double angle = ((2 * Math.PI) * k / arr.Length) * n;
                    tempSum += arr[n] * Complex.Exp(new Complex(0, -angle));
                }
                freqDomain[k] = new Complex((1.0 / freqDomain.Length) * (tempSum.Real * 2), 1.0 / freqDomain.Length * tempSum.Imaginary * 2);

            }
            return freqDomain;
        }

        public static Complex[] FFT(float[] arr, int zeroPadAmount)
        {
            Complex[] freqDomain = new Complex[arr.Length * zeroPadAmount];

            for (int k = 0; k < freqDomain.Length; k++)
            {
                Complex tempSum = 0;
                for (int n = 0; n < arr.Length; n++)
                {
                    double angle = ((2 * Math.PI) * k / arr.Length) * n;
                    tempSum += arr[n] * Complex.Exp(new Complex(0, -angle));
                }
                freqDomain[k] = new Complex((1.0 / freqDomain.Length) * (tempSum.Real * 2), 1.0 / freqDomain.Length * tempSum.Imaginary * 2);

            }
            return freqDomain;
        }

        public static Complex[][] STFT(short[][] arr)
        {
            Complex[][] tempFreq = new Complex[arr.GetLength(0)][];

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                tempFreq[i] = FFT(arr[i]);
            }

            return tempFreq;
        }
    }
}
