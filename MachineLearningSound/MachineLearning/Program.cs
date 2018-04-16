using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            int outputSize = DataSample.labelSize;
            int inputSize = 10;
            int width = 10;
            int height = 30;

            Random rand = new Random();
            NeuralNetwork nn = new NeuralNetwork(inputSize, width, height, outputSize);

            WaveFileObject waveFileObject = new WaveFileObject("C:/Users/Mikkel/Documents/Audio1Mathias.wav");

            short[][] soundChunks = waveFileObject.ToChunks(20);

            Spectrogram spec = new Spectrogram(DSProcess.STFT(soundChunks), waveFileObject.header.sampleRate);

            spec.PrintSpectrogram();


            /*
            DataSample[] samples = new DataSample[10];

            for (int i = 0; i < 10; i++)
            {
                float[] temp = new float[10];

                for (int j = 0; j < 5; j++)
                {
                    temp[j] = (50f * (float)rand.NextDouble()) + 100;
                }

                for (int j = 5; j < 10; j++)
                {
                    temp[j] = (50f * (float)rand.NextDouble()) - 100;

                }

                samples[i] = new DataSample(temp, Label.One);
            }

            for (int i = 0; i < 10; i++)
            {
                nn.TrainNetwork(samples);
            }

            nn.PrintOutput();

            float[] data = new float[10];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (float)(rand.NextDouble() + 1000) * 10;
            }

            DataSample sample = new DataSample(data, Label.Two);

            nn.DataClassification(sample);
            nn.PrintOutput();

            */
            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
    }
}
