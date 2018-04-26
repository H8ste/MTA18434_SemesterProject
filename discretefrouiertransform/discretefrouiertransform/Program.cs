using System;
using System.Numerics;


namespace discretefrouiertransform
{
    class Program
    {

        
        static void Main(string[] args)
        {
            int CalDelay = 0;
            DOAclass DOA = new DOAclass() ;
            
            //LOading two wave files
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
            while (true)
            {


                //Checks if there is anything in the signal that is high enough to activate the doa
                if (DOA.CheckTresholding(buffer1, 100))
                {
                    Console.WriteLine("Thresholding reached, running DOA...");
                    CalDelay = DOA.CrossCorrelation(buffer1, buffer2, buffer1.Length, 200);
                    Console.WriteLine("Best delay " + CalDelay);
                }
                else
                    Console.WriteLine("Thresholding not reached");

            }

                    /*AudioBuffers audioBuff = new AudioBuffers(8000,2);
                    var CalDelay = audioBuff.timeDelaySignalDOA(buffer1, buffer2,200);*/





        }
    }
}
