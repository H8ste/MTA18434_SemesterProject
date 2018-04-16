﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFTW.NET;
using NAudio.Wave;

namespace discretefrouiertransform
{
    class AudioBuffers
    {
        private List<short[]> buffers;
        private int sampleRate;

        private int waveInDevices;
        private WaveInCapabilities deviceInfo;
        private WaveInEvent waveIn;
        private int channels;


        public List<short[]> Buffers
        {
            get { return buffers; }
            private set { buffers = value; }
        }

        public int SampleRate
        {
            get { return sampleRate; }
            private set { sampleRate = value; }
        }

        public int WaveInDevices
        {
            get { return waveInDevices; }
            private set { waveInDevices = value; }
        }

        public WaveInCapabilities DeviceInfo
        {
            get { return deviceInfo; }
            private set { deviceInfo = value; }
        }

        public WaveInEvent WaveInVar
        {
            get { return waveIn; }
            private set { waveIn = value; }
        }

        public AudioBuffers(int sampleRate, int channels)
        {
            List<short[]> buffers;
            for (int i = 0; i < 4; i++)
            {
                Buffers.Add(new short[sampleRate / 5]);
            }

            WaveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < WaveInDevices; waveInDevice++)
            {
                DeviceInfo = WaveIn.GetCapabilities(waveInDevice);
                Console.WriteLine("Device {0}: {1}, {2} channels",
                    WaveInDevices, DeviceInfo.ProductName, DeviceInfo.Channels);
            }

            WaveInVar = new WaveInEvent();
            WaveInVar.DeviceNumber = 0; //Set to default
            WaveInVar.DataAvailable += waveIn_DataAvailable;

        }

        public void ExampleUsePlanDirectly(short[] inputData)
        {
            // Use the same arrays for as many transformations as you like.
            // If you can use the same arrays for your transformations, this is faster than calling DFT.FFT / DFT.IFFT
            using (var timeDomain = new FftwArrayComplex(SampleRate / 5))
            using (var frequencyDomain = new FftwArrayComplex(timeDomain.GetSize()))
            using (var fft = FftwPlanC2C.Create(timeDomain, frequencyDomain, DftDirection.Forwards))
            using (var ifft = FftwPlanC2C.Create(frequencyDomain, timeDomain, DftDirection.Backwards))
            {
                // Set the input after the plan was created as the input may be overwritten
                // during planning
                for (int i = 0; i < timeDomain.Length; i++)
                {
                    double timeInSeconds = (double)i / SampleRate;
                    timeDomain[i] = inputData[i];
                }
                //timeDomain[i] = i % 10;
                long currentTime = System.DateTime.Now.Millisecond;
                // timeDomain -> frequencyDomain
                fft.Execute();

                for (int i = frequencyDomain.Length / 2; i < frequencyDomain.Length; i++)
                    frequencyDomain[i] = 0;

                for (int i = 0; i < frequencyDomain.Length / 2; i++)
                {
                    //Console.WriteLine(frequencyDomain[i]);
                }
                // frequencyDomain -> timeDomain
                ifft.Execute();
                Console.WriteLine("length: "+ ifft.Output.Length);
                Console.WriteLine(System.DateTime.Now.Millisecond - currentTime);


                // Do as many forwards and backwards transformations here as you like
            }
        }

        public void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            Console.WriteLine(e.BytesRecorded + " samples recieved");

            //Console.WriteLine(e.BytesRecorded);
            //RUN EVERY 1600 SAMPLES RECORDED


            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                Console.WriteLine(sample);

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
                //ExampleUsePlanDirectly(buffers[i]);
                //Console.WriteLine("Done");
            }
            /*
            
            for (int i = 0; i < buffers.Count; i++)
            {
                Console.WriteLine(System.DateTime.Now.Second);
                SampleSegment soundSampleSegment = new SampleSegment(buffers[i], 8000);

                //soundSampleSegment.PrintInputArray();

                soundSampleSegment.DiscreteFourierTransform();

                //soundSampleSegment.PrintFreqArrays();

                //soundSampleSegment.MutiplyWithPhasor(0.05);

                soundSampleSegment.InverseFourierTransform();
                Console.WriteLine(System.DateTime.Now.Second);
                Console.WriteLine("calculated");


                //soundSampleSegment.PrintOutputArray();

            }
            */

            //Console.Clear();

            //for (int i = 0; i < buffers.Count; i++)
            //{
            //    Console.Write("Buffer " + i + ": ");
            //    Console.Write("[");
            //    for (int j = 0; j < buffers[i].Length; j++)
            //    {

            //        Console.Write(buffers[i][j]);
            //        Console.Write(", ");
            //        if (j == buffers[i].Length - 1)
            //        {
            //            Console.WriteLine("]");
            //        }
            //    }
            //}


        }
    }
}
