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

            WaveFileObject fileObject1 = new WaveFileObject("/Users/Lynge/Desktop/Audio 1HejSiriStoej.wav");
            WaveFileObject fileObject2 = new WaveFileObject("/Users/Lynge/Desktop/Audio 2HejSiriStoej.wav");
            int Samples = fileObject1.soundData.Count;
            Console.WriteLine(Samples);
            double[] buffer1 = new double[Samples];
            double[] buffer2 = new double[Samples];

            

            for (int i = 0; i < Samples; i++)
            {
                buffer1[i] = fileObject1.soundData[i];
                buffer2[i] = fileObject2.soundData[i];
            }

            DOAclass dO = new DOAclass();
            int CalDelay = dO.CrossCorrelation(buffer1,buffer2,buffer1.Length,200);
            Console.WriteLine("Best delay " + CalDelay);

        }
    }
}
