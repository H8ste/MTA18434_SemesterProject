using System;
using System.Collections.Generic;
using System.Numerics;
using FFTW.NET;
using NAudio.Wave;


namespace discretefrouiertransform
{
    class Program
    {
        public static string filenameFILE;
        public static List<short[]> buffers = new List<short[]>();
        static void Main(string[] args)
        {
            
        

            //ExampleUsePlanDirectly();
            Console.WriteLine("Enter to start recording");
            Console.WriteLine();
        
            filenameFILE = Console.ReadLine();
            AudioBuffers inputAudio = new AudioBuffers(8000, 2);
            Console.Write("Number of samples: ");
            //short[] file1 = inputAudio.readAndWrite("C:/Users/Heine/Desktop/rec/beamformertest/beamformeronly/" + "0_800hz_beamformer.wav");
            //short[] file2 = inputAudio.readAndWrite("C:/Users/Heine/Desktop/rec/beamformertest/beamformeronly/" + "0_1600hz_beamformer.wav");

            //inputAudio.BeamAndWrite(file1, file2);

            // C:\Users\Heine\Desktop\rec\beamformertest\beamformeronly

            inputAudio.WaveInVar.StartRecording();
            Console.WriteLine("Recording " + filenameFILE);

            while (true)
            {

            }
        }
       

       
        //FFT radix-2 algorithm
        //N chose is: N = 2^B

        //      f = (k/N) * f.s
    }
}