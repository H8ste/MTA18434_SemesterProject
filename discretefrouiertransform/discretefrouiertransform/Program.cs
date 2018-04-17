using System;
using System.Numerics;


namespace discretefrouiertransform
{
    class Program
    {
       

        static void Main(string[] args)
        {
            DOAclass DOA = new DOAclass();
            WaveFileObject fileObject1 = new WaveFileObject("/Users/Lynge/Desktop/Audio 1HejSiriStoej.wav");
            WaveFileObject fileObject2 = new WaveFileObject("/Users/Lynge/Desktop/Audio 2HejSiriStoej.wav");

            int Samples = fileObject1.soundData.Count;


            double[] buffer1 = new double[Samples];
            double[] buffer2 = new double[Samples];

            for (int i = 0; i < Samples; i++)
            {
                buffer1[i] = fileObject1.soundData[i];
                buffer2[i] = fileObject2.soundData[i];
            }

           
            var CalDelay = DOA.CrossCorrelation(buffer1,buffer2,buffer1.Length,200);

            Console.WriteLine("Best delay " + CalDelay);
        }
    }
}
