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

            int sampleRate = 44100;
            double[] buffer = new double[512];
            double amplitude = 0.7;
            double frequency1 = 250;
            double frequency2 = 752;

            for (int i = 0; i < buffer.Length; i++)
            {
                double time_in_seconds = (double)i / sampleRate;
                buffer[i] = 22 * Math.Sin(2 * Math.PI * frequency1 * time_in_seconds) + (57) * Math.Sin(2 * Math.PI * frequency2 * time_in_seconds);
            }

            //for (int n = 0; n < buffer.Length; n++)
            //{
            //    buffer[n] = (amplitude * Math.Sin((2 * Math.PI * n * frequency1))) + (amplitude * Math.Sin((2 * Math.PI * n * frequency2)));
            //}


            //double[] inputarr = new double[8] { 0, 0.707, 1, 0.707, 0, -0.707, -1, -0.707 };
            SampleSegment soundSampleSegment = new SampleSegment(buffer, sampleRate);

            soundSampleSegment.PrintInputArray();

            soundSampleSegment.DiscreteFourierTransform();

            soundSampleSegment.PrintFreqArrays();

            soundSampleSegment.MutiplyWithPhasor(0.05);

            soundSampleSegment.InverseFourierTransform();

            soundSampleSegment.PrintOutputArray();

            Console.ReadLine();
        }


        //FFT radix-2 algorithm
        //N chose is: N = 2^B


        //      f = (k/N) * f.s

    }
}
