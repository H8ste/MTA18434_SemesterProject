using FFTW.NET;
using NAudio.Wave;
using System;
using System.Collections.Generic;
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

        public AudioIn(int sampleRate, int device)
        {
            if (device > WaveIn.DeviceCount)
            {
                throw new Exception();
            }

            waveInDevices = WaveIn.DeviceCount;
            waveEvent = new WaveInEvent();
            waveEvent.DeviceNumber = device;
            waveEvent.DataAvailable += OnWaveIn;
            channels = WaveIn.GetCapabilities(device).Channels;
            waveEvent.WaveFormat = new WaveFormat(sampleRate, 1);
            waveEvent.StartRecording();

            string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "NetworkSave01.json";

            // Load Neural network 
            network = NeuralNetwork.LoadNetwork(path);
        }

        public void OnWaveIn(object sender, WaveInEventArgs e)
        {
            if (waveEvent == null)
                return;
            try
            {
                short[] audioData = new short[e.Buffer.Length / 2];
                Buffer.BlockCopy(e.Buffer, 0, audioData, 0, e.Buffer.Length);

                // FFT HERE

                double[] magnitudes;

                using (var pinIn = new PinnedArray<double>(audioData))
                using (var comOut = new FftwArrayComplex(DFT.GetComplexBufferSize(pinIn.GetSize())))
                {
                    DFT.FFT(pinIn, comOut);

                    magnitudes = new double[comOut.Length];
                    for (int i = 0; i < comOut.Length; i++)
                    {
                        magnitudes[i] = comOut[i].Magnitude;
                    }
                }

                DataSample sample = new DataSample(magnitudes);
                network.DataClassification(sample);

            }
            catch(Exception exe)
            {
                Console.WriteLine(exe.Message);
            }
        }
    }
}
