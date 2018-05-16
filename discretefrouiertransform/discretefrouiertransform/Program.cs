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
            AudioBuffers audioBuff = new AudioBuffers(44100, 2);

            Console.WriteLine("Enter to start record");
            Console.ReadLine();

            audioBuff.WaveInVar.StartRecording();

            

            //LOading two wave files
            //WaveFileObject fileObject1 = new WaveFileObject("/Users/Lynge/Desktop/Audio 1HejSiriStoej.wav");
            //WaveFileObject fileObject2 = new WaveFileObject("/Users/Lynge/Desktop/Audio 2HejSiriStoej.wav");

            //int Samples = fileObject1.soundData.Count;
            /*

            double[] buffer1 = new double[Samples];
            double[] buffer2 = new double[Samples];

            for (int i = 0; i < Samples; i++)
            {
                buffer1[i] = fileObject1.soundData[i];
                buffer2[i] = fileObject2.soundData[i];
            }
            */



            while (true)
            {
             //   AudioBuffers.printBuffer(audioBuff.mic1, "Mic 1: ");
              //  AudioBuffers.printBuffer(audioBuff.mic2, "Mic 2: ");
                
                //Checks if there is anything in the signal that is high enough to activate the doa
                if (DOA.CheckTresholding(audioBuff.mic1, 500))
                {
                    //Console.WriteLine("Thresholding reached, running DOA...");
                    CalDelay = DOA.CrossCorrelation(audioBuff.mic1, audioBuff.mic2, audioBuff.mic1.Length, 200);
                   // Console.WriteLine(audioBuff.mic1.Length);
                    Console.WriteLine("Best delay " + CalDelay);
                }
               // else
                    //Console.WriteLine("Thresholding not reached");
                    

            }
            





        }
    }
}
