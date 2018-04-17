using System;
using System.Collections.Generic;
using System.Numerics;
using FFTW.NET;
using NAudio.Wave;


namespace discretefrouiertransform
{
    class Program
    {
        public static List<short[]> buffers = new List<short[]>();
        static void Main(string[] args)
        {
            AudioBuffers inputAudio = new AudioBuffers(8000, 1);

            //ExampleUsePlanDirectly();
            Console.WriteLine("Enter to start recording");
            Console.ReadLine();
            inputAudio.WaveInVar.StartRecording();


            while (true)
            {

            }
        }
       

       
        //FFT radix-2 algorithm
        //N chose is: N = 2^B

        //      f = (k/N) * f.s
    }
}