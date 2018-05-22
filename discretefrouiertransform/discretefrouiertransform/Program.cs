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