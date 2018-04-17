using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FFTW.NET;
using NAudio.Wave;

namespace discretefrouiertransform
{
    class AudioBuffers
    {
        private List<short[]> buffers;
        private List<double[]> shiftedBuffers;
        private double[] outputSignal;
        private int sampleRate;

        private int waveInDevices;
        private WaveInCapabilities deviceInfo;
        private WaveInEvent waveIn;
        private int channels;

        private bool firsttime;

        private int i = 0;


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
            SampleRate = sampleRate;
            Channels = channels;
            firsttime = true;
            OutputSignal = new double[SampleRate / 5 / 2];

            Buffers = new List<short[]>();
            ShiftedBuffers = new List<double[]>();
            for (int i = 0; i < 5; i++)
            {
                Buffers.Add(new short[SampleRate / 5 / 2]);
                ShiftedBuffers.Add(new double[SampleRate / 5 / 2]);
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
            WaveInVar.WaveFormat = new WaveFormat(SampleRate, Channels);
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
                //Console.WriteLine(pinIn.GetLength(input.Rank - 1) / 2 + 1);
                //Console.WriteLine(pinOut.GetLength(input.Rank - 1));

                //Console.WriteLine(input.Length);
                //Console.WriteLine(output.Length);

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

            //Console.WriteLine("Input");

            //for (int i = 0; i < input.Length; i++)
            //{
            //    Console.WriteLine(input[i]);
            //}

            //Console.WriteLine();
            //Console.WriteLine();

            //Console.WriteLine("output");
            //for (int i = 0; i < inOut.Length; i++)
            //    Console.WriteLine(inOut[i] / input.Length);



        }

        public void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            Console.WriteLine(e.BytesRecorded + " samples recieved");

            //Console.WriteLine(e.BytesRecorded);
            //RUN EVERY 1600 SAMPLES RECORDED


            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);


                if (!firsttime && index < e.BytesRecorded / 2 / 2)
                    Buffers[3 + (i - 1) % 2][index + Buffers[3 + i % 2].Length / 2] = sample;

                if (index < e.BytesRecorded / 2)
                    Buffers[0][index] = sample;

                if (index >= e.BytesRecorded / 2 / 2 && index < e.BytesRecorded / 2 + e.BytesRecorded / 2 / 2)
                    Buffers[1][index - e.BytesRecorded / 2 / 2] = sample;

                if (index >= e.BytesRecorded / 2)
                    Buffers[2][index - e.BytesRecorded / 2] = sample;

                if (index >= e.BytesRecorded / 2 + e.BytesRecorded / 2 / 2)
                    Buffers[3 + (i % 2)][index - e.BytesRecorded / 2 - e.BytesRecorded / 2 / 2] = sample;

            }




            Console.Clear();

            printBuffer(Buffers);
            
            for (int i = 0; i < buffers.Count; i++)
                ShiftedBuffers[i] = timeDelaySignal(Buffers[i], 0.000001);


            printBuffer(ShiftedBuffers);

            for (int j = 0; j < OutputSignal.Length; j++)
            {
                double hann = 0.5 * (1 - Math.Cos(2*Math.PI*j/(OutputSignal.Length-1)));

                if (!firsttime && j < e.BytesRecorded / 2 / 2)
                {
                    hann = 0.5 * (1 - Math.Cos(2 * Math.PI * ((((i-1)%2)*Buffers[0].Length/2) + j) / (OutputSignal.Length - 1)));
                    OutputSignal[j] = (hann * ShiftedBuffers[0][j]) + (hann * ShiftedBuffers[3 + (i - 1) % 2][j]);
                } else if (firsttime && j < e.BytesRecorded / 2 / 2)
                {
                    OutputSignal[j] = ShiftedBuffers[0][j];
                }
                else if (j >= e.BytesRecorded / 2 / 2 && j < e.BytesRecorded / 2)
                {
                    OutputSignal[j] = (hann * ShiftedBuffers[0][j]) + (hann * ShiftedBuffers[1][j]);
                }
                else if (j >= e.BytesRecorded / 2 && j < e.BytesRecorded / 2 + e.BytesRecorded / 2 / 2)
                {
                    //OutputSignal[j] = (hann)
                }
                else if (j < e.BytesRecorded / 2 + e.BytesRecorded / 2 / 2)
                {
                    
                }



                    //OutputSignal[i] = 

            }

            Console.ReadLine();

            firsttime = false;
            i++;

            /*


        //OLD AND SLOW FUNCTION DEPRICATED
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

        }

        private void printBuffer(List<short[]>input)
        {
            Console.WriteLine("ORIGINAL SIGNAL");
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
        private void printBuffer(List<double[]> input)
        {
            Console.WriteLine("SHIFTED SIGNAL");
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
    }


}
