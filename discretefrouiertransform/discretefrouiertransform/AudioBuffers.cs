﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFTW.NET;
using NAudio.Wave;
//using WaveFileObject;

namespace discretefrouiertransform
{
    class AudioBuffers
    {
        private WaveFileWriter writer;
        private WaveFileWriter writerOriginal;
        public List<short[]> mics;
        private List<short[]> buffers;
        private List<double[]> shiftedBuffers;
        private double[] outputSignal;
        private int sampleRate;

        DOAclass DOA = new DOAclass();

        public short[] mic1;
        public short[] mic2;
        public short[] mic1Buff;
        public short[] mic2Buff;

        public List<short> aleppo;

        private List<double> combinedOutput;

        private int waveInDevices;
        private WaveInCapabilities deviceInfo;
        private WaveInEvent waveIn;
        private int channels;

        private bool firsttime;

        private int i = 0;
        private int duration = 0;
        private Boolean controlBool = false;


        public List<short[]> Buffers
        {
            get { return buffers; }
            private set { buffers = value; }
        }

        public List<double[]> ShiftedBuffers
        {
            get { return shiftedBuffers; }
            private set { shiftedBuffers = value; }
        }

        public double[] OutputSignal
        {
            get { return outputSignal; }
            private set { outputSignal = value; }
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
        public int Channels
        {
            get { return channels; }
            private set { channels = value; }
        }

        public AudioBuffers(int sampleRate, int channels)
        {
            mics = new List<short[]>();
            SampleRate = sampleRate;
            mics.Add(new short[SampleRate/10]);
            mics.Add(new short[SampleRate/10]);

            mic1 = new short[sampleRate / 10];
            mic2 = new short[sampleRate / 10];
            mic1Buff = new  short[8000];
            mic2Buff = new short[8000];

            aleppo = new List<short>();

            Channels = channels;
            firsttime = true;
            OutputSignal = new double[mics[0].Length];

            Buffers = new List<short[]>();
            ShiftedBuffers = new List<double[]>();
            combinedOutput = new List<double>();

            for (int j = 0; j < 5; j++)
            {
                Buffers.Add(new short[SampleRate / 10/2]);
                ShiftedBuffers.Add(new double[SampleRate/10 / 2]);
            }

            WaveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                Console.WriteLine("Device {0}: {1}, {2} channels",
                    waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }

            
            WaveInVar = new WaveInEvent();
            WaveInVar.DeviceNumber = 0; //Set to default
            WaveInVar.DataAvailable += waveIn_DataAvailable;
            WaveInVar.WaveFormat = new WaveFormat(SampleRate, 16, Channels);
            Console.WriteLine(WaveInVar.WaveFormat.AverageBytesPerSecond);
            

            /* writer = new WaveFileWriter("C:/Users/Nickl/Documents/audiosample/beamformed.wav",
                 WaveFormat.CreateCustomFormat(WaveInVar.WaveFormat.Encoding, SampleRate, 1,
                     WaveInVar.WaveFormat.AverageBytesPerSecond, (1 * WaveInVar.WaveFormat.BitsPerSample) / 16, 16));
             writerOriginal = new WaveFileWriter("C:/Users/Nickl/Documents/audiosample/original.wav",
                 WaveFormat.CreateCustomFormat(WaveInVar.WaveFormat.Encoding, SampleRate, 1,
                     WaveInVar.WaveFormat.AverageBytesPerSecond, (1 * WaveInVar.WaveFormat.BitsPerSample) / 16, 16)); */
        }

        public static double[] timeDelaySignal(double[] signalInput, double s)
        {
            //Console.WriteLine("Runs");

            double[] input = new double[signalInput.Length];
            for (int j = 0; j < signalInput.Length; j++)
            {
                input[j] = signalInput[j];
            }

            //Console.WriteLine("Runs");

            Complex[] output = new Complex[input.GetLength(input.Rank - 1) / 2 + 1];
            double[] inOut = new double[input.Length];

            using (var pinIn = new PinnedArray<double>(input))
            using (var pinOut = new PinnedArray<Complex>(output))
            using (var in1Out = new PinnedArray<double>(inOut))
            {

                //Console.WriteLine("fak");

                DFT.FFT(pinIn, pinOut);
                for (int j = 0; j < pinOut.Length; j++)
                {
                    double angle = ((2 * Math.PI) / pinOut.Length) * 8000 * j * 100 * s;
                    pinOut[j] = pinOut[j] * Complex.Exp(new Complex(0, -angle));
                }

                DFT.IFFT(pinOut, in1Out);
                for (int j = 0; j < inOut.Length; j++)
                {
                    inOut[j] = inOut[j] / input.Length;
                }

                //Console.WriteLine("pak");

                return inOut;
            }
        }

        public void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            //Console.WriteLine(e.BytesRecorded + " samples recieved");

            //Console.WriteLine(e.BytesRecorded/2);
            //RUN EVERY SAMPLING RATE / 10 * CHANNELS

            //originalSignal = new double[e.BytesRecorded / 2];
            int k = 0;
            //bool first = true;



            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                mics[(index / 2) % 2][(index / 2) - k] = sample;
                if ((index / 2) % 2 == 0)
                    k++;
            }


            splitInput(mics[0]);


            //printBuffer(mics[0], "ORIGINAL");
            // printBuffer(mics[0], "ORIGINAL SIGNAL - FIRSTMIC");
            //printBuffer(mics[1], "ORIGINAL SIGNAL - SECONDMIC");
            mic1 = mics[0];
            mic2 = mics[1];

            controlBool = DOA.CheckTresholding(mic1, 500);
            if (controlBool) { 
                for (int i = 0; i < 800; i++)
                {
                    aleppo.Add(mic1[i]);
                }

                if (aleppo.Count >= 8000)
                {
                    for (int i = 0; i < mic1Buff.Length; i++)
                    {
                        mic1Buff[i] = aleppo[i];
                        mic2Buff[i] = aleppo[i];
                    }
                    aleppo.Clear();

                    controlBool = false;

                    printBuffer(mic1Buff, "Allepp ");
                }

            }

            ////Console.Clear();

            //printBuffer(Buffers, "SEPERATED INPUT SIGNAL");
            for (int j = 0; j < Buffers.Count; j++)
            {
                //    //Console.WriteLine("Buffer: " + Buffers.Count);
                //    //Console.WriteLine("runs " + j);
                //(ShiftedBuffers[j] = timeDelaySignal(Buffers[j], 0.0030631);
            }

            ////Console.WriteLine("Stuck");
            /*
            //printBuffer(ShiftedBuffers, "SHIFTED SEPERATED INPUT SIGNAL");
            for (int j = 0; j < OutputSignal.Length; j++)
            {
                //double hann = 0.5 * (1 - Math.Cos(2 * Math.PI * j / (ShiftedBuffers[0].Length - 1)));

                if (firsttime != true && j < ShiftedBuffers[0].Length / 2)
                {
                    double hann1 = 0.5 * (1 - Math.Cos(2 * Math.PI * (j) / (ShiftedBuffers[0].Length - 1)));
                    double hann2 = 0.5 * (1 - Math.Cos(2 * Math.PI * (j + ShiftedBuffers[0].Length / 2) / (ShiftedBuffers[0].Length - 1)));
                    OutputSignal[j] = (hann1 * ShiftedBuffers[0][j]) + (hann2 * ShiftedBuffers[3 + (i - 1) % 2][j + ShiftedBuffers[3 + (i - 1) % 2].Length / 2]);
                }
                else if (firsttime == true && j < ShiftedBuffers[0].Length / 2)
                {
                    //double hann = 0.5 * (1 - Math.Cos(2 * Math.PI * j / (ShiftedBuffers[0].Length - 1)));
                    OutputSignal[j] = ShiftedBuffers[0][j];
                }
                else if (j >= ShiftedBuffers[0].Length / 2 && j < ShiftedBuffers[0].Length)
                {
                    double hann1 = 0.5 * (1 - Math.Cos(2 * Math.PI * j / (ShiftedBuffers[0].Length - 1)));
                    double hann2 = 0.5 * (1 - Math.Cos(2 * Math.PI * (j - ShiftedBuffers[0].Length / 2) / (ShiftedBuffers[0].Length - 1)));
                    OutputSignal[j] = (hann1 * ShiftedBuffers[0][j]) + (hann2 * ShiftedBuffers[1][j - ShiftedBuffers[0].Length / 2]);
                }
                else if (j >= ShiftedBuffers[0].Length && j < ShiftedBuffers[0].Length + ShiftedBuffers[0].Length / 2)
                {
                    double hann1 = 0.5 * (1 - Math.Cos(2 * Math.PI * (j - ShiftedBuffers[0].Length / 2) / (ShiftedBuffers[0].Length - 1)));
                    double hann2 = 0.5 * (1 - Math.Cos(2 * Math.PI * (j - ShiftedBuffers[0].Length) / (ShiftedBuffers[0].Length - 1)));


                    OutputSignal[j] = (hann1 * ShiftedBuffers[1][j - ShiftedBuffers[1].Length / 2]) +
                                      (hann2 * ShiftedBuffers[2][j - ShiftedBuffers[2].Length]);
                }
                else if (j >= ShiftedBuffers[0].Length + ShiftedBuffers[0].Length / 2)
                {
                    double hann1 = 0.5 * (1 - Math.Cos(2 * Math.PI * (j - (ShiftedBuffers[0].Length)) / (ShiftedBuffers[0].Length - 1)));
                    double hann2 = 0.5 * (1 - Math.Cos(2 * Math.PI * (j - (ShiftedBuffers[0].Length + ShiftedBuffers[0].Length / 2)) / (ShiftedBuffers[0].Length - 1)));

                    OutputSignal[j] = (hann1 * ShiftedBuffers[2][j - ShiftedBuffers[2].Length]) +
                                      (hann2 * ShiftedBuffers[3 + (i % 2)][j - (ShiftedBuffers[0].Length + ShiftedBuffers[0].Length / 2)]);
                } 
                
        }*/

            //for (int j = 0; j < OutputSignal.Length; j++)
            //{
            //    if (j < OutputSignal.Length / 2)
            //    {
            //        OutputSignal[j] = ShiftedBuffers[0][j];
            //    }
            //    else
            //    {
            //        OutputSignal[j] = ShiftedBuffers[2][j - Buffers[0].Length];
            //    }
            //}

            i++;
            firsttime = false;

            //printBuffer(mics[0], "ORIGINAL SIGNAL");
            //printBuffer(OutputSignal, "FINALSHIFTED SIGNAL");

            /*double[] beamformedSignal = new double[mics[0].Length];
            for (int j = 0; j < beamformedSignal.Length; j++)
                beamformedSignal[j] = mics[1][j] + OutputSignal[j];

            //printBuffer(beamformedSignal, "BEAMFORMED SIGNAL");
            //for (int j = 0; j < mics[0].Length; j++)
            //{
            //    combinedOutput.Add(beamformedSignal[j]);
            //}
            //List<byte> byteArr = new List<byte>();

            for (int j = 0; j < beamformedSignal.Length; j++)
            {
                byte[] tempByteArr = BitConverter.GetBytes((short)beamformedSignal[j]);
                //Console.WriteLine("Starting to Append file");
                writer.Write(tempByteArr, 0, tempByteArr.Length);
                //Console.WriteLine("File Appended");
                duration++;
                //for (int l = 0; l < 2; l++)
                //{
                //    byteArr.Add(tempByteArr[l]);
                //}
            }
            for (int j = 0; j < OutputSignal.Length; j++)
            {
                byte[] tempByteArr = BitConverter.GetBytes((short)mics[0][j]);
                //Console.WriteLine("Starting to Append file");
                writerOriginal.Write(tempByteArr, 0, tempByteArr.Length);
                //Console.WriteLine("File Appended");
                //for (int l = 0; l < 2; l++)
                //{
                //    byteArr.Add(tempByteArr[l]);
                //}
            }

    */








            //WaveFileObject.WriteWaveFile(byteArr.ToArray(), "C:/Users/Nickl/Documents/testing.wav", SampleRate);
            if (duration > SampleRate * 30)
            {
                Console.WriteLine("File written");
                Console.ReadLine();
            }

        }

        private void splitInput(short[] input)
        {
            for (int index = 0; index < input.Length; index++)
            {
                if (!firsttime && index < input.Length / 2 / 2)
                    Buffers[3 + (i - 1) % 2][index + Buffers[3 + i % 2].Length / 2] = input[index];

                if (index < input.Length / 2)
                    Buffers[0][index] = input[index];

                if (index >= input.Length / 2 / 2 && index < input.Length / 2 + input.Length / 2 / 2)
                    Buffers[1][index - input.Length / 2 / 2] = input[index];

                if (index >= input.Length / 2)
                    Buffers[2][index - input.Length / 2] = input[index];

                if (index >= input.Length / 2 + input.Length / 2 / 2)
                    Buffers[3 + (i % 2)][index - input.Length / 2 - input.Length / 2 / 2] = input[index];
            }
        }

        private void printBuffer(List<short[]> input, string text)
        {
            Console.WriteLine(text);
            for (int j = 0; j < input.Count; j++)
            {

                Console.Write("\n" + "Buffer " + j + ": ");
                Console.Write("[");
                for (int k = 0; k < input[j].Length; k++)
                {

                    Console.Write(input[j][k]);
                    Console.Write(", ");
                    if (k == input[j].Length - 1)
                    {
                        Console.WriteLine("]");
                    }
                }
            }
        }

   
        private void printBuffer(List<double[]> input, string text)
        {
            Console.WriteLine(text);
            for (int i = 0; i < input.Count; i++)
            {
                Console.Write("Buffer " + i + ": ");
                Console.Write("[");
                for (int j = 0; j < input[i].Length; j++)
                {

                    Console.Write(input[i][j]);
                    Console.Write(", ");
                    if (j == input[i].Length - 1)
                    {
                        Console.WriteLine("]");
                    }
                }
            }
        }
        public static void printBuffer(double[] input, string text)
        {
            Console.Write(text + "= [");

            for (int j = 0; j < input.Length; j++)
            {

                Console.Write(input[j]);
                Console.Write(" ");
                if (j == input.Length - 1)
                {
                    Console.WriteLine("]");
                }
            }

        }
        public static void printBuffer(short[] input, string text)
        {
            Console.Write(text + "=");
            Console.Write("[");
            for (int j = 0; j < input.Length; j++)
            {

                Console.Write(input[j]);
                Console.Write(" ");
                if (j == input.Length - 1)
                {
                    Console.WriteLine("]");
                }
            }

        }
    }


}
