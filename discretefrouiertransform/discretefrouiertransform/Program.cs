using System;
using System.Numerics;


namespace discretefrouiertransform
{
    class Program
    {
       

        static void Main(string[] args)
        {
            DOAclass DOA;
       
            WaveFileObject fileObject1 = new WaveFileObject("/Users/Lynge/Desktop/Audio 1HejSiriStoej.wav");
            WaveFileObject fileObject2 = new WaveFileObject("/Users/Lynge/Desktop/Audio 2HejSiriStoej.wav");

            int Samples = fileObject1.soundData.Count;


            short[] buffer1 = new short[Samples];
            short[] buffer2 = new short[Samples];

            for (int i = 0; i < Samples; i++)
            {
                buffer1[i] = fileObject1.soundData[i];
                buffer2[i] = fileObject2.soundData[i];
            }

           
            //var CalDelay = DOA.CrossCorrelation(buffer1,buffer2,buffer1.Length,200);
            AudioBuffers audioBuff = new AudioBuffers(8000,2);
            var CalDelay = audioBuff.timeDelaySignalDOA(buffer1, buffer2,200);


            Console.WriteLine("Best delay " + CalDelay);
        }
    }
}
