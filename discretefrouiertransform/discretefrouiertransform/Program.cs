using System;
using System.Collections.Generic;
using System.Numerics;
using NAudio.Wave;


namespace discretefrouiertransform
{
    class Program
    {
        public static List<short[]> buffers = new List<short[]>();
        static void Main(string[] args)
        {
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                Console.WriteLine("Device {0}: {1}, {2} channels",
                    waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }

            WaveInEvent waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += waveIn_DataAvailable;
            int sampleRate = 8000; // 8 kHz
            int channels = 1; // mono
            waveIn.WaveFormat = new WaveFormat(sampleRate, channels);
            waveIn.StartRecording();

            buffers.Add(new short[800]);
            buffers.Add(new short[800]);
            buffers.Add(new short[800]);

            /*

            int sampleRate = 44100;
            double[] buffer = new double[512];
            double amplitude = 0.7;
            double frequency1 = 250;
            double frequency2 = 752;

            for (int i = 0; i < buffer.Length; i++)
            {
                double timeInSeconds = (double)i / sampleRate;
                buffer[i] = 22 * Math.Sin(2 * Math.PI * frequency1 * timeInSeconds) + (57) * Math.Sin(2 * Math.PI * frequency2 * timeInSeconds);
            }
            
            SampleSegment soundSampleSegment = new SampleSegment(buffer, sampleRate);

            soundSampleSegment.PrintInputArray();

            soundSampleSegment.DiscreteFourierTransform();

            soundSampleSegment.PrintFreqArrays();

            soundSampleSegment.MutiplyWithPhasor(0.05);

            soundSampleSegment.InverseFourierTransform();

            soundSampleSegment.PrintOutputArray();

            */

            Console.ReadLine();
        }
        static void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            Console.WriteLine(e.BytesRecorded);
            //RUN EVERY 1600 SAMPLES RECORDED
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);

                if (index < e.BytesRecorded / 2)
                {
                    buffers[0][index] = sample;
                }

                if (index >= e.BytesRecorded / 2 / 2 && index < e.BytesRecorded / 2 + e.BytesRecorded / 2 / 2)
                {
                    buffers[1][index - e.BytesRecorded / 2 / 2] = sample;
                }

                if (index >= e.BytesRecorded / 2)
                {
                    buffers[2][index - e.BytesRecorded / 4 * 2] = sample;
                }

                //float sample32 = sample / 32768f;
            }

            for (int i = 0; i < buffers.Count; i++)
            {
                SampleSegment soundSampleSegment = new SampleSegment(buffers[i], 8000);

                //soundSampleSegment.PrintInputArray();

                soundSampleSegment.DiscreteFourierTransform();

                soundSampleSegment.PrintFreqArrays();

                //soundSampleSegment.MutiplyWithPhasor(0.05);

                soundSampleSegment.InverseFourierTransform();

                //soundSampleSegment.PrintOutputArray();
            }
            //Console.Clear();
            /*
            for (int i = 0; i < buffers.Count; i++)
            {
                Console.Write("Buffer " + i + ": ");
                Console.Write("[");
                for (int j = 0; j < buffers[i].Length; j++)
                {

                    Console.Write(buffers[i][j]);
                    Console.Write(", ");
                    if (j == buffers[i].Length - 1)
                    {
                        Console.WriteLine("]");
                    }
                }
            }
            */

        }
        //FFT radix-2 algorithm
        //N chose is: N = 2^B

        //      f = (k/N) * f.s
    }
}