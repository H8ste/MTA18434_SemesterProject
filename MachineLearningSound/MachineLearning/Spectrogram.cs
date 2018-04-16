using System;
using System.Numerics;

namespace MachineLearning
{
    public class Spectrogram
    {
        public double[,] spectrogram;
        long sampleRate;

        public Spectrogram(Complex[][] complexes, long sampleRate)
        {
            this.sampleRate = sampleRate;
            spectrogram = new double[complexes.Length, complexes[0].Length];

            Console.WriteLine(complexes.Length);

            for (int x = 0; x < complexes.Length; x++)
            {
                for (int y = 0; y < complexes[0].Length; y++)
                {
                    //spectrogram[x, y] = 20 *Math.Log(Math.Sqrt((complexes[x][y].Real * complexes[x][y].Real) + (complexes[x][y].Imaginary * complexes[x][y].Imaginary)));
                    //spectrogram[x, y] = Math.Sqrt( complexes[x][y].Magnitude * complexes[x][y].Magnitude );
                    spectrogram[x, y] = complexes[x][y].Magnitude;
                }
            }
        }

        public void PrintSpectrogram()
        {
            for (int i = 0; i < spectrogram.GetLength(0); i++)
            {
                for (int j = 0; j < spectrogram.GetLength(1); j++)
                {
                    Console.WriteLine("index " + i + " , " + "Frequency: " + (double)j / spectrogram.GetLength(1) * sampleRate + " Db: " + spectrogram[i,j]);       
                }
            }
        }
    }
}
