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

            Console.ReadLine();

            while (true)
            {

            }
        }
        //static void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        //{
        //    Console.WriteLine(e.BytesRecorded + " samples recieved");

        //    //Console.WriteLine(e.BytesRecorded);
        //    //RUN EVERY 1600 SAMPLES RECORDED

            
        //    for (int index = 0; index < e.BytesRecorded; index += 2)
        //    {
        //        short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
        //        Console.WriteLine(sample);
                
        //        if (index < e.BytesRecorded / 2)
        //        {
        //            buffers[0][index] = sample;
        //        }

        //        if (index >= e.BytesRecorded / 2 / 2 && index < e.BytesRecorded / 2 + e.BytesRecorded / 2 / 2)
        //        {
        //            buffers[1][index - e.BytesRecorded / 2 / 2] = sample;
        //        }

        //        if (index >= e.BytesRecorded / 2)
        //        {
        //            buffers[2][index - e.BytesRecorded / 4 * 2] = sample;
        //        }
                


        //        //float sample32 = sample / 32768f;
        //    }
            
        //    for (int i = 0; i < buffers.Count; i++)
        //    {
        //        //ExampleUsePlanDirectly(buffers[i]);
        //        //Console.WriteLine("Done");
        //    }
        //    /*
            
        //    for (int i = 0; i < buffers.Count; i++)
        //    {
        //        Console.WriteLine(System.DateTime.Now.Second);
        //        SampleSegment soundSampleSegment = new SampleSegment(buffers[i], 8000);

        //        //soundSampleSegment.PrintInputArray();

        //        soundSampleSegment.DiscreteFourierTransform();

        //        //soundSampleSegment.PrintFreqArrays();

        //        //soundSampleSegment.MutiplyWithPhasor(0.05);

        //        soundSampleSegment.InverseFourierTransform();
        //        Console.WriteLine(System.DateTime.Now.Second);
        //        Console.WriteLine("calculated");


        //        //soundSampleSegment.PrintOutputArray();

        //    }
        //    */

        //    //Console.Clear();
            
        //    //for (int i = 0; i < buffers.Count; i++)
        //    //{
        //    //    Console.Write("Buffer " + i + ": ");
        //    //    Console.Write("[");
        //    //    for (int j = 0; j < buffers[i].Length; j++)
        //    //    {

        //    //        Console.Write(buffers[i][j]);
        //    //        Console.Write(", ");
        //    //        if (j == buffers[i].Length - 1)
        //    //        {
        //    //            Console.WriteLine("]");
        //    //        }
        //    //    }
        //    //}
            

        //}

       
        //FFT radix-2 algorithm
        //N chose is: N = 2^B

        //      f = (k/N) * f.s
    }
}