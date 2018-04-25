using FFTW.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        void NetworkTraining(int inputSize, int width, int height, int itterations)
        {
            int outputSize = DataSample.labelSize;

            string databasePath1 = "";
            string networkPath1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/NetworkSave01.json";
            NeuralNetwork network = null;

            if (File.Exists(networkPath1))
            {
                network = NeuralNetwork.LoadNetwork(networkPath1);

                if (network.inputSize != inputSize)
                {
                    throw new Exception();
                }

                if (network.hiddenDimension != width)
                {
                    throw new Exception();
                }

                if (network.hiddenSize != height)
                {
                    throw new Exception();
                }

            }
            else
            {
                network = new NeuralNetwork(inputSize, width, height, outputSize);
            }

            if (File.Exists(databasePath1))
            {
                SampleDatabase samples = new SampleDatabase(databasePath1);
                DataSample[] trainingSamples = new DataSample[10];
                Random rand = new Random();

                for (int i = 0; i < itterations; i++)
                {
                    // pick 10 samples
                    for (int j = 0; j < 10; j++)
                    {
                        int num = rand.Next(0, samples.database.Length);
                        trainingSamples[i] = new DataSample(samples.database[num].data, samples.database[num].label);
                    }

                    network.TrainNetwork(trainingSamples);
                }

                network.SaveNetwork(networkPath1);
            }
            else
            {
                throw new Exception();
            }
        }

        static void Example1D()
        {
            double[] input = new double[24];
            Complex[] output = new Complex[input.GetLength(input.Rank - 1) / 2 + 1];
            double[] inOut = new double[input.Length];         

            for (int i = 0; i < input.Length; i++)
                input[i] = Math.Sin(i * 2 * Math.PI * 128 / input.Length);

            using (var pinIn = new PinnedArray<double>(input))
            using (var pinOut = new PinnedArray<Complex>(output))
            using (var in1Out = new PinnedArray<double>(inOut))
            {
                Console.WriteLine(pinIn.GetLength(input.Rank - 1) / 2 + 1);
                Console.WriteLine(pinOut.GetLength(input.Rank - 1));

                Console.WriteLine(input.Length);
                Console.WriteLine(output.Length);

                DFT.FFT(pinIn, pinOut);
                DFT.IFFT(pinOut, in1Out);
            }

            Console.WriteLine("Input");

            for (int i = 0; i < input.Length; i++)
            {
                Console.WriteLine(Math.Sin(i * 2 * Math.PI * 128 / input.Length));
            }

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("output");
            for (int i = 0; i < inOut.Length; i++)
                Console.WriteLine(inOut[i] / input.Length);
        }

        static void Example2D()
        {
            using (var input = new AlignedArrayComplex(16, 64, 16))
            using (var output = new AlignedArrayComplex(16, input.GetSize()))
            {
                for (int row = 0; row < input.GetLength(0); row++)
                {
                    for (int col = 0; col < input.GetLength(1); col++)
                        input[row, col] = (double)row * col / input.Length;
                }

                DFT.FFT(input, output);
                DFT.IFFT(output, output);

                for (int row = 0; row < input.GetLength(0); row++)
                {
                    for (int col = 0; col < input.GetLength(1); col++)
                        Console.Write((output[row, col].Real / input[row, col].Real / input.Length).ToString("F2").PadLeft(6));
                    Console.WriteLine();
                }
            }
        }

        static void ExampleR2C()
        {
            double[] input = new double[97];

            var rand = new Random();

            for (int i = 0; i < input.Length; i++)
                input[i] = rand.NextDouble();

            using (var pinIn = new PinnedArray<double>(input))
            using (var output = new FftwArrayComplex(DFT.GetComplexBufferSize(pinIn.GetSize())))
            {
                DFT.FFT(pinIn, output);
                for (int i = 0; i < output.Length; i++)
                    Console.WriteLine(output[i]);
            }
        }

        static void ExampleUsePlanDirectly()
        {
            // Use the same arrays for as many transformations as you like.
            // If you can use the same arrays for your transformations, this is faster than calling DFT.FFT / DFT.IFFT
            using (var timeDomain = new FftwArrayComplex(253))
            using (var frequencyDomain = new FftwArrayComplex(timeDomain.GetSize()))
            using (var fft = FftwPlanC2C.Create(timeDomain, frequencyDomain, DftDirection.Forwards))
            using (var ifft = FftwPlanC2C.Create(frequencyDomain, timeDomain, DftDirection.Backwards))
            {
                // Set the input after the plan was created as the input may be overwritten
                // during planning
                for (int i = 0; i < timeDomain.Length; i++)
                    timeDomain[i] = i % 10;

                // timeDomain -> frequencyDomain
                fft.Execute();

                for (int i = frequencyDomain.Length / 2; i < frequencyDomain.Length; i++)
                    frequencyDomain[i] = 0;

                // frequencyDomain -> timeDomain
                ifft.Execute();

                // Do as many forwards and backwards transformations here as you like
            }
        }
    }
}
