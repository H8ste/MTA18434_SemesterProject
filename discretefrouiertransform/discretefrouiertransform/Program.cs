using System;
using System.Numerics;


namespace discretefrouiertransform
{
    class Program
    {
        //static short[] audioStored_temp1;
        //static short[] audioStored_temp2;

        //static short[] audioStored_temp3;

        //private static String filepath = "C:/Users/Nickl/Documents/Visual Studio 2017/Projects/beamformingphasor/discretefrouiertransform/discretefrouiertransform/testingrec.m4a";
        //private static String filepath = AppDomain.CurrentDomain.BaseDirectory + "testing.wav";

        static void Main(string[] args)
        {
            /*
            audioStored_temp1 = new short[512];
            audioStored_temp2 = new short[512];
            audioStored_temp3 = new short[512];

            WaveFileObject storedAudioFile = new WaveFileObject(filepath);

            for (int i = 0; i < 512 * 2; i++)
            {
                if (i < 512)
                {
                    audioStored_temp1[i] = storedAudioFile.soundData[i];
                }

                if (i >= 512 / 2 && i < 512 + 512 / 2)
                {
                    audioStored_temp2[i - 512 / 2] = storedAudioFile.soundData[i];
                }

                if (i >= 512)
                {
                    audioStored_temp3[i - 512] = storedAudioFile.soundData[i];
                }
            }


            //storedAudioFile.PrintData();

            List<short[]> storedInputs = new List<short[]>();
            storedInputs.Add(audioStored_temp1);
            storedInputs.Add(audioStored_temp2);
            storedInputs.Add(audioStored_temp3);

            List<Complex[]> frequency_arr = new List<Complex[]>();

            //ReadInitialArray();
            */

            int sampleRate = 1024;
            double[] buffer1 = new double[1024];
            double[] buffer2 = new double[1024];
            double frequency1 = 250;

            for (int i = 0; i < buffer1.Length; i++)
            {
                double time_in_seconds = (double)i / sampleRate;
                buffer1[i] = 22 * Math.Sin(2 * Math.PI * frequency1 * time_in_seconds);
                buffer2[i] = 22 * Math.Cos(2 * Math.PI * frequency1 * time_in_seconds);

            }

            //for (int n = 0; n < buffer.Length; n++)
            //{
            //    buffer[n] = (amplitude * Math.Sin((2 * Math.PI * n * frequency1))) + (amplitude * Math.Sin((2 * Math.PI * n * frequency2)));
            //}


            //double[] inputarr = new double[8] { 0, 0.707, 1, 0.707, 0, -0.707, -1, -0.707 };
            SampleSegment soundSampleSegment = new SampleSegment(buffer1, sampleRate);

            soundSampleSegment.PrintInputArray();

            soundSampleSegment.FourierTransform();

            soundSampleSegment.PrintFreqArrays();

            soundSampleSegment.InverseFourierTransform();

            //soundSampleSegment.PrintOutputArray();

            Console.ReadLine();

            //For testing DOAclass

            double[] x = { 0, 0, 0, 1, 2, 3, 0, 0 };
            double[] y = { 1, 2, 3, 0, 0, 0, 0, 0 };

            DOAclass dO = new DOAclass();
            int DelayBack = dO.CrossCorrelation(buffer1,buffer2,buffer1.Length,1000);
            Console.WriteLine("Best delay " + DelayBack);

        }


        //FFT radix-2 algorithm
        //N chose is: N = 2^B


        //      f = (k/N) * f.s

    }
}
