﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFTW.NET;
using NAudio.Wave;

namespace discretefrouiertransform
{
    class AudioBuffers
    {
        private WaveFileWriter writer;
        private WaveFileWriter writerOriginal1;
        private WaveFileWriter writerOriginal2;
        private WaveFileWriter combinedWriter;
        private string filePath;
        public List<short[]> mics;
        private List<short[]> buffers;
        private List<double[]> shiftedBuffers;
        private double[] outputSignal;
        private int sampleRate;


        public short[] file1;
        public short[] file2;

        private List<double> combinedOutput;

        private int waveInDevices;
        private WaveInCapabilities deviceInfo;
        private WaveInEvent waveIn;
        private int channels;

        private bool firsttime;

        private int i = 0;
        private int duration = 0;

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
            mics.Add(new short[SampleRate / 10]);
            mics.Add(new short[SampleRate / 10]);
            Channels = channels;
            firsttime = true;
            OutputSignal = new double[mics[0].Length];

            Buffers = new List<short[]>();
            ShiftedBuffers = new List<double[]>();
            combinedOutput = new List<double>();

            for (int j = 0; j < 5; j++)
            {
                Buffers.Add(new short[SampleRate / 10 / 2]);
                ShiftedBuffers.Add(new double[SampleRate / 10 / 2]);
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
            string beamformer = "C:/Users/Nickl/Aalborg Universitet/OneDrive - Aalborg Universitet/3rdtryBeamformer/onlyBeamformer/" + Program.filenameFILE + "_beamformer.wav";
            string micro1 = "C:/Users/Nickl/Aalborg Universitet/OneDrive - Aalborg Universitet/3rdtryBeamformer/" + Program.filenameFILE + "_micro1.wav";
            string micro2 = "C:/Users/Nickl/Aalborg Universitet/OneDrive - Aalborg Universitet/3rdtryBeamformer/" + Program.filenameFILE + "_micro2.wav";
            string combined = "C:/Users/Nickl/Aalborg Universitet/OneDrive - Aalborg Universitet/3rdtryBeamformer/" + Program.filenameFILE + "_combined.wav";



            writer = new WaveFileWriter(beamformer,
                WaveFormat.CreateCustomFormat(WaveInVar.WaveFormat.Encoding, SampleRate, 1,
                    WaveInVar.WaveFormat.AverageBytesPerSecond, (1 * WaveInVar.WaveFormat.BitsPerSample) / 16, 16));
            writerOriginal1 = new WaveFileWriter(micro1,
                WaveFormat.CreateCustomFormat(WaveInVar.WaveFormat.Encoding, SampleRate, 1,
                    WaveInVar.WaveFormat.AverageBytesPerSecond, (1 * WaveInVar.WaveFormat.BitsPerSample) / 16, 16));
            writerOriginal2 = new WaveFileWriter(micro2,
                WaveFormat.CreateCustomFormat(WaveInVar.WaveFormat.Encoding, SampleRate, 1,
                    WaveInVar.WaveFormat.AverageBytesPerSecond, (1 * WaveInVar.WaveFormat.BitsPerSample) / 16, 16));
            combinedWriter = new WaveFileWriter(combined,
                WaveFormat.CreateCustomFormat(WaveInVar.WaveFormat.Encoding, SampleRate, 1,
                    WaveInVar.WaveFormat.AverageBytesPerSecond, (1 * WaveInVar.WaveFormat.BitsPerSample) / 16, 16));
        }

        public double[] timeDelaySignal(short[] signalInput, double s)
        {

            double[] input = new double[signalInput.Length];
            for (int j = 0; j < signalInput.Length; j++)
            {
                input[j] = (double)signalInput[j];
            }

            Complex[] output = new Complex[input.GetLength(input.Rank - 1) / 2 + 1];
            double[] inOut = new double[input.Length];

            using (var pinIn = new PinnedArray<double>(input))
            using (var pinOut = new PinnedArray<Complex>(output))
            using (var in1Out = new PinnedArray<double>(inOut))
            {
                DFT.FFT(pinIn, pinOut);
                for (int j = 0; j < pinOut.Length; j++)
                {
                    double angle = ((2 * Math.PI) / pinOut.Length) * SampleRate * j * s;
                    pinOut[j] = pinOut[j] * Complex.Exp(new Complex(0, -angle));
                }

                DFT.IFFT(pinOut, in1Out);
                for (int j = 0; j < inOut.Length; j++)
                {
                    inOut[j] = inOut[j] / input.Length;
                }
                return inOut;
            }
        }

        public void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {

            int k = 0;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                mics[(index / Channels) % Channels][(index / Channels) - k] = sample;
                if ((index / Channels) % Channels == 0)
                    k++;
            }


            splitInput(mics[0]);
            for (int j = 0; j < Buffers.Count; j++)
            {
                double AngleList = 50;
                double LenghtMic = 0.075;
                double reset = (1) * LenghtMic * Math.Cos(90 * Math.PI / 180) / 343;
                double shiftinseconds = (-1) * LenghtMic * Math.Cos(AngleList * Math.PI / 180) / 343;
                ShiftedBuffers[j] = timeDelaySignal(Buffers[j], shiftinseconds);
            }

            for (int j = 0; j < OutputSignal.Length; j++)
            {
   
                if (firsttime != true && j < ShiftedBuffers[0].Length / 2)
                {
                    double hann1 = 0.5 * (1 - Math.Cos(2 * Math.PI * (j) / (ShiftedBuffers[0].Length - 1)));
                    double hann2 = 0.5 * (1 - Math.Cos(2 * Math.PI * (j + ShiftedBuffers[0].Length / 2) / (ShiftedBuffers[0].Length - 1)));
                    OutputSignal[j] = (hann1 * ShiftedBuffers[0][j]) + (hann2 * ShiftedBuffers[3 + (i - 1) % 2][j + ShiftedBuffers[3 + (i - 1) % 2].Length / 2]);
                }
                else if (firsttime == true && j < ShiftedBuffers[0].Length / 2)
                {
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
            }

            i++;
            firsttime = false;
            double[] beamformedSignal = new double[mics[0].Length];
            for (int j = 0; j < beamformedSignal.Length; j++)
                beamformedSignal[j] = (mics[1][j] + OutputSignal[j])/2;
            for (int j = 0; j < beamformedSignal.Length; j++)
            {
                byte[] tempByteArr = BitConverter.GetBytes((short)beamformedSignal[j]);
                writer.Write(tempByteArr, 0, tempByteArr.Length);
                duration++;
            }
            
            for (int j = 0; j < OutputSignal.Length; j++)
            {
                byte[] tempByteArr = BitConverter.GetBytes((short)mics[0][j]);
                writerOriginal1.Write(tempByteArr, 0, tempByteArr.Length);
            }
            

            for (int j = 0; j < OutputSignal.Length; j++)
            {
                byte[] tempByteArr = BitConverter.GetBytes((short)mics[1][j]);
                writerOriginal2.Write(tempByteArr, 0, tempByteArr.Length);
            }
            


            for (int j = 0; j < OutputSignal.Length; j++)
            {
                double currentNumber = (mics[0][j] + mics[1][j])/2.0;
                byte[] tempByteArr = BitConverter.GetBytes((short)Math.Round(currentNumber));
                combinedWriter.Write(tempByteArr, 0, tempByteArr.Length);
            }
            

            if (duration > SampleRate*30)
            {
                Console.WriteLine("File written");
                writer.Close();
                writerOriginal1.Close();
                writerOriginal2.Close();
                combinedWriter.Close();
                Environment.Exit(0);
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
                
                Console.Write("\n" +"Buffer " + j + ": ");
                Console.Write("[");
                for (int k = 0; k < input[j].Length; k++)
                {

                    Console.Write(input[j][k]);
                    Console.Write(", ");
                    if (k == input[j].Length-1)
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
        private void printBuffer(double[] input, string text)
        {
            Console.WriteLine(text);
            Console.Write("[");
            for (int j = 0; j < input.Length; j++)
            {

                Console.Write(input[j]);
                Console.Write(", ");
                if (j == input.Length - 1)
                {
                    Console.WriteLine("]");
                }
            }

        }
        private void printBuffer(short[] input, string text)
        {
            Console.WriteLine(text);
            Console.Write("[");
            for (int j = 0; j < input.Length; j++)
            {

                Console.Write(input[j]);
                Console.Write(", ");
                if (j == input.Length - 1)
                {
                    Console.WriteLine("]");
                }
            }

        }
    }


}
