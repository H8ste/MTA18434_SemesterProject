using System;
using System.Numerics;
using NAudio.Wave;


namespace discretefrouiertransform
{
    class Program
    {

        private static WaveFileWriter writer;
        private static WaveFileWriter writerOriginal;
        static void Main(string[] args)
        {
          
        int CalDelay = 0;
            DOAclass DOA = new DOAclass() ;
            AudioBuffers audioBuff = new AudioBuffers(8000, 2);

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
            /*
            bool runWhile = true;
           
            while (runWhile)
            {
                //   AudioBuffers.printBuffer(audioBuff.mic1, "Mic 1: ");
                //  AudioBuffers.printBuffer(audioBuff.mic2, "Mic 2: ");

                if (DOA.CheckTresholding(audioBuff.mic1, 200))
                {
                    // Console.WriteLine("Thresholding reached, running DOA...");
                    CalDelay = DOA.CrossCorrelation(audioBuff.mic1, audioBuff.mic2, audioBuff.mic1.Length, 200);
                    // Console.WriteLine("Best delay " + CalDelay);

                    short[] tempBuffMic1 = new short[8000];
                    short[] tempBuffMic2 = new short[8000];
                    short[] originalSignal = new short[8000];

                    

                    //tempBuff = audioBuff.mic2;
                    //tempBuff1 = audioBuff.mic1;
                    
                    for (int i = 0; i < 8000 ; i++)
                    {
                        tempBuffMic2[i] = audioBuff.mic2buff[i];
                        tempBuffMic1[i] = audioBuff.mic1[i];
                    }
                    

                    for (int i = 0; i < originalSignal.Length - 1; i++)
                    {
                        originalSignal[i] = (short) (tempBuffMic1[i] + tempBuffMic2[i]);
                    }
                    

                    // Console.WriteLine("Angle: " + calculateAngle(CalDelay));

                    //Console.WriteLine(CalDelay);
                    //AudioBuffers.printBuffer(tempBuffMic1, "tempBuffMic1");
                    //AudioBuffers.printBuffer(tempBuffMic2, "tempBuffMic2");
                    /*
                    short[] shiftedBuff = new short[8000];
                    shiftedBuff = ShiftSignal(tempBuffMic1, CalDelay);
                    //AudioBuffers.printBuffer(shiftedBuff, "Shifted:");

                    short[] beamformedBuff = new short[8000];

                    for (int i = 0; i < beamformedBuff.Length; i++)
                    {
                        beamformedBuff[i] = (short)(shiftedBuff[i] + tempBuffMic2[i]);
                    }

                   
                    AudioBuffers.printBuffer(beamformedBuff, "Beamformed: ");

                    
                    WaveInEvent wavw = new WaveInEvent();

                      writer = new WaveFileWriter("C:/Users/Heine/Desktop/Ny Mappe/beamformed.wav",
                        WaveFormat.CreateCustomFormat(wavw.WaveFormat.Encoding, 8000, 1,
                       wavw.WaveFormat.AverageBytesPerSecond, (1 * wavw.WaveFormat.BitsPerSample) / 16, 16));
                       writerOriginal = new WaveFileWriter("C:/Users/Heine/Desktop/Ny Mappe/fuck.wav",
                        WaveFormat.CreateCustomFormat(wavw.WaveFormat.Encoding, 8000, 1,
                            wavw.WaveFormat.AverageBytesPerSecond, (1 * wavw.WaveFormat.BitsPerSample) / 16, 16));

                    runWhile = false;
                   //
                   
        }
        */


            // Console.WriteLine("Thresholding not reached");


        

            while (true)
            {

            }



        }


    
   

        static public short[] ShiftSignal(short[] signal, int delay)
        {
            int j = 0;
            short[] tempSignal = new short[signal.Length];
            for (int i = 0; i < signal.Length; i++)
            {

                j = i + delay;
                if (j < 0 || j >= signal.Length)
                    continue;
                else
                    tempSignal[j] = signal[i];

        
            }
            return tempSignal;

        }

        static public Double calculateAngle(Double bestDelay)
        {
            Console.WriteLine(bestDelay);
            Double timeDiff = (1.0 / 8000.0) *  bestDelay;
            Console.WriteLine(timeDiff);
            Double angle = Math.Acos((343.0 * timeDiff) / 0.5);
            return RadianToDegree(angle);
        }

        static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

    }
}
