using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{
    public class NeuralNetwork
    {
        public int inputSize;
        public int hiddenDimension;
        public int hiddenSize;
        public int outputSize;

        private float[] inputlayerValue;
        private float[] outputLayerValue;
        private float[,] hiddenLayerValue;

        private float[,] inputLayerWeights;
        private float[,,] hiddenLayerWeights;
        private float[,] outputLayerWeights;

        private Random random;

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

        public void DataClassification(DataSample sample)
        {
            SigmoidActivation(sample);
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

        public void PrintInput()
        {
            for (int i = 0; i < inputSize; i++)
            {
                Console.WriteLine("Input value: " + i + " " + inputlayerValue[i]);
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

        float ReturnCost(float[] idealVector)
        {
            float sum = 0;

            for (int i = 0; i < outputSize; i++)
            {
                sum += (outputLayerValue[i] - idealVector[i]) * (outputLayerValue[i] - idealVector[i]);
            }

            return sum;
        }

        private void SigmoidActivation(DataSample sample)
        {
            if (sample.data.Length != inputSize)
            {
                return;
            }

            inputlayerValue = sample.data;

            // First Layer Activation
            for (int i = 0; i < inputSize; i++)
            {
                float sum = 0;

                for (int j = 0; j < hiddenSize; j++)
                {
                    sum += (inputlayerValue[i] * inputLayerWeights[i, j]);
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

            Console.WriteLine("Cost after Activation is: " + ReturnCost(CreateIdealVector(sample.label)));
        }

        private GradientWeightVector[] GradientBackPropArr(DataSample[] samples)
        {
            GradientWeightVector[] gradients = new GradientWeightVector[samples.Length];

            for (int i = 0; i < samples.Length; i++)
            {
                SigmoidActivation(samples[i]);
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

            // Backpropagation for the outputlayer
            for (int i = 0; i < outputSize; i++)
            {
                outputLayerDelta[i] = -(idealVector[i] - outputLayerValue[i]) * outputLayerValue[i] *
                    (1 - outputLayerValue[i]);

                for (int j = 0; j < hiddenSize; j++)
                {
                    weightVector.outputLayerWeights[i, j] = outputLayerDelta[i] * hiddenLayerValue[hiddenDimension - 1, j];
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
                    weightVector.hiddenLayerWeights[hiddenDimension - 3, i, j] = hiddenLayerDelta[hiddenDimension - 2, i] *
                        hiddenLayerValue[hiddenDimension - 2, i] * (1 - hiddenLayerValue[hiddenDimension - 2, i]) *
                        hiddenLayerValue[hiddenDimension - 3, j];
                }
            }

            for (int i = hiddenDimension - 3; i > 0; i--)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    float sum = 0;

                    for (int k = 0; k < hiddenSize; k++)
                    {
                        sum += hiddenLayerDelta[i + 1, j] * hiddenLayerWeights[i, j, k];
                    }

                    hiddenLayerDelta[i, j] = sum;

                    for (int k = 0; k < hiddenSize; k++)
                    {
                        weightVector.hiddenLayerWeights[i - 1, j, k] = hiddenLayerDelta[i, j] * hiddenLayerValue[i, j] *
                            (1 - hiddenLayerValue[i, j]) * hiddenLayerValue[i - 1, j];
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
                    weightVector.inputLayerWeights[i, j] = inputLayerDelta[i] * hiddenLayerValue[0, j] *
                        (1 - hiddenLayerValue[0, j]) * inputlayerValue[i];
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
            inputlayerValue = new float[inputSize];
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
