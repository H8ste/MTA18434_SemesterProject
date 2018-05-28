using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{
    [System.Serializable]
    public class NeuralNetwork
    {
        public int inputSize;
        public int hiddenDimension;
        public int hiddenSize;
        public int outputSize;

        private double[] inputLayerValue;
        private float[] outputLayerValue;
        private float[,] hiddenLayerValue;

        public float[,] inputLayerWeights;
        public float[,,] hiddenLayerWeights;
        public float[,] outputLayerWeights;

        public List<float> avgGradientList = new List<float>();

        private Random random;
        private float learningRate = .01f;

        //These should be serialized 
        private static int sumCount;
        private static float totalsum;
        private static float averageCost;

        public NeuralNetwork(int inputSize, int hiddenDimension, int hiddenSize, int outputSize)
        {
            this.inputSize = inputSize;
            this.hiddenDimension = hiddenDimension;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;

            random = new Random();

            InputLayerInit();
            HiddenLayerInit();
            OutputLayerInit();
        }

        public void SaveNetwork(string path)
        {
            string file = JsonConvert.SerializeObject(this);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, file);
        }

        public static NeuralNetwork LoadNetwork(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader r = new StreamReader(path))
                {
                    using (JsonReader reader = new JsonTextReader(r))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Console.WriteLine("Deserializing");
                        NeuralNetwork network = serializer.Deserialize<NeuralNetwork>(reader);
                        return network;
                    }
                }
            }
            else
            {
                return null;
                throw new FileNotFoundException();
            }
        }

        public void DataClassification(DataSample sample)
        {
            SigmoidActivation(sample);

            if (ReturnCost(CreateIdealVector(sample.label)) < .5)
            {
                Console.Write("Classified: ");
                float maxVal = outputLayerValue[0];
                int index = 0;

                for (int i = 0; i < outputSize; i++)
                {
                    if (outputLayerValue[i] > maxVal)
                    {
                        maxVal = outputLayerValue[i];
                        index = i;
                    }
                }

                switch (index)
                {
                    case 0:
                        Console.Write("Classified: Blank");
                        break;
                    case 1:
                        Console.Write("Classified: Noise");
                        break;
                    case 2:
                        Console.Write("Classified: Hej");
                        break;
                    case 3:
                        Console.Write("Classified: Siri");
                        break;
                }
            }
        }

        public void TrainNetwork(DataSample[] samples)
        {
            GradientWeightVector[] gradients = GradientBackPropArr(samples);

            //Average Gradient of input layer
            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    float sum = 0;

                    for (int k = 0; k < gradients.Length; k++)
                    {
                        sum += gradients[k].inputLayerWeights[i, j];
                    }

                    sum = sum / gradients.Length;

                    inputLayerWeights[i, j] = inputLayerWeights[i, j] - sum;
                }
            }

            //Average graident of hidden layer
            for (int i = 0; i < hiddenDimension - 1; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    for (int k = 0; k < hiddenSize; k++)
                    {
                        float sum = 0;

                        for (int f = 0; f < gradients.Length; f++)
                        {
                            sum += gradients[f].hiddenLayerWeights[i, j, k];
                        }

                        sum = sum / gradients.Length;

                        hiddenLayerWeights[i, j, k] = hiddenLayerWeights[i, j, k] - sum;
                    }
                }
            }

            //Average graident of output layer
            for (int i = 0; i < outputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    float sum = 0;

                    for (int k = 0; k < gradients.Length; k++)
                    {
                        sum += gradients[k].outputLayerWeights[i, j];
                    }

                    sum = sum / gradients.Length;

                    outputLayerWeights[i, j] = outputLayerWeights[i, j] - sum;
                }
            }

            
        }

        public void PrintAverageCost()
        {
            Console.WriteLine("Average Cost is: " + averageCost);
        }

        public void PrintInput()
        {
            for (int i = 0; i < inputSize; i++)
            {
                Console.WriteLine("Input value: " + i + " " + inputLayerValue[i]);
            }
        }

        public void PrintOutput()
        {
            for (int i = 0; i < outputSize; i++)
            {
                Console.WriteLine("Output value: " + i + " " + outputLayerValue[i]);
            }
        }

        public void PrintWeights()
        {
            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    Console.WriteLine("Input Weight: " + i + " " + j + " " + inputLayerWeights[i, j]);
                }
            }

            for (int i = 0; i < hiddenDimension - 1; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    for (int k = 0; k < hiddenSize; k++)
                    {
                        Console.WriteLine("Hidden Weight: " + i + " " + j + " " + k + " " + hiddenLayerWeights[i, j, k]);
                    }
                }
            }

            for (int i = 0; i < outputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    Console.WriteLine("Output Weight: " + i + " " + j + " " + outputLayerWeights[i, j]);
                }
            }
        }

        public void PrintNetworkActivations()
        {
            PrintInput();

            for (int i = 0; i < hiddenDimension; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    Console.WriteLine("Hidden value: " + i + " " + j + " " + hiddenLayerValue[i, j]);
                }
            }

            PrintOutput();
        }

        public void PrintAverageGradientList()
        {
            Console.WriteLine();
            for (int i = 0; i < avgGradientList.Count; i++)
            {
                Console.Write(avgGradientList[i] + " ");
            }
            Console.WriteLine();
        }

        private float ReturnCost(float[] idealVector)
        {
            float sum = 0;

            for (int i = 0; i < outputSize; i++)
            {
                sum += (outputLayerValue[i] - idealVector[i]) * (outputLayerValue[i] - idealVector[i]);
            }
            averageCost = AverageCost(sum);
            return sum;
        }

        private float AverageCost(float sum)
        {
            sumCount++;
            totalsum += sum;
            avgGradientList.Add(totalsum / sumCount);
            return totalsum / sumCount;
        }

        private void SigmoidActivation(DataSample sample)
        {
            if (sample.data.Length != inputSize)
            {
                Console.WriteLine("NOT the same size - " + sample.data.Length + " / " + inputSize);
                return;
            }

            inputLayerValue = sample.data;

            // First Layer Activation
            for (int i = 0; i < hiddenSize; i++)
            {
                float sum = 0;

                for (int j = 0; j < inputSize; j++)
                {
                    sum += (float)(inputLayerValue[j] * inputLayerWeights[j, i]);
                }

                hiddenLayerValue[0, i] = ActivationFunctions.Sigmoid(sum);
            }

            // Hidden Layer Activation
            for (int i = 1; i < hiddenDimension; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    float sum = 0;

                    for (int k = 0; k < hiddenSize; k++)
                    {
                        sum += (hiddenLayerValue[i - 1, k] * hiddenLayerWeights[i - 1, j, k]);
                    }

                    hiddenLayerValue[i, j] = ActivationFunctions.Sigmoid(sum);
                }
            }

            // Output Layer Activation
            for (int i = 0; i < outputSize; i++)
            {
                float sum = 0;

                for (int j = 0; j < hiddenSize; j++)
                {
                    sum += (hiddenLayerValue[hiddenDimension - 1, j] * outputLayerWeights[i, j]);
                }
                outputLayerValue[i] = ActivationFunctions.Sigmoid(sum);
            }

            
        }

        private GradientWeightVector[] GradientBackPropArr(DataSample[] samples)
        {
            GradientWeightVector[] gradients = new GradientWeightVector[samples.Length];

            for (int i = 0; i < samples.Length; i++)
            {
                gradients[i] = GradientBackProp(samples[i]);
            }
            return gradients;
        }

        private GradientWeightVector GradientBackProp(DataSample sample)
        {
            // Backprop Output Layer
            GradientWeightVector weightVector = new GradientWeightVector(this);
            float[] idealVector = CreateIdealVector(sample.label);
            float[] outputLayerDelta = new float[outputSize];
            float[,] hiddenLayerDelta = new float[hiddenDimension - 1, hiddenSize];
            float[] inputLayerDelta = new float[inputSize];

            SigmoidActivation(sample);
            Console.WriteLine("Label: " + sample.label);
            Console.WriteLine();
            PrintOutput();
            Console.WriteLine();
            Console.WriteLine("Cost after Activation is: " + ReturnCost(CreateIdealVector(sample.label)));
            PrintAverageCost();

            // Backpropagation for the outputlayer
            for (int i = 0; i < outputSize; i++)
            {
                outputLayerDelta[i] = -(idealVector[i] - outputLayerValue[i]) * outputLayerValue[i] *
                    (1 - outputLayerValue[i]);

                for (int j = 0; j < hiddenSize; j++)
                {
                    weightVector.outputLayerWeights[i, j] = learningRate * (outputLayerDelta[i] * hiddenLayerValue[hiddenDimension - 1, j]);
                }
            }

            // Finding delta and gradients for the last hidden layer
            for (int i = 0; i < hiddenSize; i++)
            {
                float sum = 0;

                for (int j = 0; j < outputSize; j++)
                {
                    for (int k = 0; k < hiddenSize; k++)
                    {
                        sum += outputLayerDelta[j] * outputLayerWeights[j, k];
                    }
                }

                hiddenLayerDelta[hiddenDimension - 2, i] = sum;

                for (int j = 0; j < hiddenSize; j++)
                {
                    weightVector.hiddenLayerWeights[hiddenDimension - 2, i, j] = learningRate * (hiddenLayerDelta[hiddenDimension - 2, i] *
                        hiddenLayerValue[hiddenDimension - 1, i] * (1 - hiddenLayerValue[hiddenDimension - 1, i]) *
                        hiddenLayerValue[hiddenDimension - 2, j]);
                }
            }

            for (int i = hiddenDimension - 2; i > 0; i--)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    float sum = 0;

                    for (int k = 0; k < hiddenSize; k++)
                    {
                        sum += hiddenLayerDelta[i, j] * hiddenLayerWeights[i, j, k];
                    }

                    hiddenLayerDelta[i, j] = sum;

                    for (int k = 0; k < hiddenSize; k++)
                    {
                        weightVector.hiddenLayerWeights[i, j, k] = learningRate * (hiddenLayerDelta[i, j] * hiddenLayerValue[i, j] *
                            (1 - hiddenLayerValue[i, j]) * hiddenLayerValue[i - 1, j]);
                    }
                }
            }

            // BackPropagation for the input layer
            for (int i = 0; i < inputSize; i++)
            {
                float sum = 0;

                for (int j = 0; j < hiddenSize; j++)
                {
                    sum += hiddenLayerDelta[0, j] * inputLayerWeights[i, j];
                }

                inputLayerDelta[i] = sum;

                for (int j = 0; j < hiddenSize; j++)
                {
                    weightVector.inputLayerWeights[i, j] = learningRate * (inputLayerDelta[i] * hiddenLayerValue[0, j] *
                        (1 - hiddenLayerValue[0, j]) * (float)inputLayerValue[i]);
                }
            }

            return weightVector;
        }

        private float[] CreateIdealVector(int label)
        {
            float[] idealVector = new float[outputSize];

            for (int i = 0; i < outputSize; i++)
            {
                if (i == label)
                {
                    idealVector[i] = 1;
                }
                else
                {
                    idealVector[i] = 0;
                }
            }
            return idealVector;
        }

        private void InputLayerInit()
        {
            inputLayerValue = new double[inputSize];
            inputLayerWeights = new float[inputSize, hiddenSize];

            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    inputLayerWeights[i, j] = (float)random.NextDouble() - .5f;
                }
            }
        }

        private void HiddenLayerInit()
        {
            hiddenLayerValue = new float[hiddenDimension, hiddenSize];
            hiddenLayerWeights = new float[hiddenDimension - 1, hiddenSize, hiddenSize];

            for (int i = 0; i < hiddenDimension - 1; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    for (int k = 0; k < hiddenSize; k++)
                    {
                        hiddenLayerWeights[i, j, k] = (float)random.NextDouble() - .5f;
                    }
                }
            }
        }

        private void OutputLayerInit()
        {
            outputLayerValue = new float[outputSize];
            outputLayerWeights = new float[outputSize, hiddenSize];

            for (int i = 0; i < outputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    outputLayerWeights[i, j] = (float)random.NextDouble() - .5f;
                }
            }
        }
    }

    [System.Serializable]
    public class GradientWeightVector
    {
        public float[,] inputLayerWeights;
        public float[,,] hiddenLayerWeights;
        public float[,] outputLayerWeights;

        public GradientWeightVector(NeuralNetwork network)
        {
            inputLayerWeights = new float[network.inputSize, network.hiddenSize];
            hiddenLayerWeights = new float[network.hiddenDimension - 1, network.hiddenSize, network.hiddenSize];
            outputLayerWeights = new float[network.outputSize, network.hiddenSize];
        }
    }
}
