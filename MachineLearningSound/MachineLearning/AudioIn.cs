using FFTW.NET;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{
    class AudioIn
    {
        public WaveInEvent waveEvent;
        public int waveInDevices;

        public NeuralNetwork network = null;

        private int channels;
        private int bufferSize;
        private int sampleRate;

        private short[] audioDataTrue;
        private short[] audioDataOdd;
        double[] tempOdd;
        double[] tempTrue;

        PinnedArray<double> realIn;
        FftwArrayComplex comOut;
        FftwPlanRC fft;

        private int offsetTrue = 0;
        private int offsetOdd = 0;
        private int offsetInit = 0;

        public AudioIn(int sampleRate, int device, int bufferSize)
        {
            if (device > WaveIn.DeviceCount)
            {
                throw new Exception();
            }

            this.sampleRate = sampleRate;
            this.bufferSize = bufferSize;
            audioDataTrue = new short[bufferSize + 20286];
            tempOdd = new double[audioDataTrue.Length];
            tempTrue = new double[audioDataTrue.Length];

            realIn = new PinnedArray<double>(audioDataTrue.Length);
            comOut = new FftwArrayComplex(DFT.GetComplexBufferSize(realIn.GetSize()));
            fft = FftwPlanRC.Create(realIn, comOut, DftDirection.Forwards);

            waveInDevices = WaveIn.DeviceCount;
            waveEvent = new WaveInEvent();
            waveEvent.DeviceNumber = device;
            waveEvent.DataAvailable += OnWaveIn;
            channels = WaveIn.GetCapabilities(device).Channels;
            waveEvent.WaveFormat = new WaveFormat(sampleRate, 1);

            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/NetworkSave01.json";

            // Load Neural network
            if (File.Exists(path))
            {
                Console.WriteLine("exists");
                network = NeuralNetwork.LoadNetwork(path);
            }
            else
            {
                throw new FileNotFoundException();
            }

            //network.PrintWeights();

            waveEvent.StartRecording();
        }

        public void OnWaveIn(object sender, WaveInEventArgs e)
        {
            if (waveEvent == null)
                return;
            try
            {
                Console.WriteLine("Audio in");

                if (offsetTrue >= bufferSize)
                {
                    tempTrue = Array.ConvertAll(audioDataTrue, item => (double)item);
                    ComputeAndClassify(tempTrue);
                    offsetTrue = 0;
                }

                Buffer.BlockCopy(e.Buffer, 0, audioDataTrue, offsetTrue, e.Buffer.Length);
                offsetTrue += e.Buffer.Length;             
            }
            catch(Exception exe)
            {
                Console.WriteLine(exe);
                throw;
            }
        }

        private void ComputeAndClassify(double[] arr)
        {
            double[] magnitudes;

            for (int i = 0; i < realIn.Length; i++)
            {
                realIn[i] = arr[i];
            }

            Console.WriteLine("FFT");
            fft.Execute();

            magnitudes = new double[comOut.Length];
            for (int i = 0; i < 4000; i++)
            {
                magnitudes[i] = 10 * Math.Log10((comOut[i].Magnitude / bufferSize) * (comOut[i].Magnitude / bufferSize));

                if (10 * Math.Log10((comOut[i].Magnitude / bufferSize) * (comOut[i].Magnitude / bufferSize)) > 40)
                {
                    //Console.WriteLine("Bin: " + i * sampleRate / comOut.Length + " " + 10 * Math.Log10((comOut[i].Magnitude / bufferSize) * (comOut[i].Magnitude / bufferSize)));
                }
                
            }
            DataSample sample = new DataSample(magnitudes);

            Console.WriteLine("Classification");
            network.DataClassification(sample);
        }
    }
}
