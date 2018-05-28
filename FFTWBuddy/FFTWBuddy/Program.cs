using FFTW.NET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Numerics;
using static FFTWBuddy.WaveFileObject;

namespace FFTWBuddy
{
    class Program
    {

        public static int sampleRate = 44100;
        public static int inputSize = 22050;

        static void Main(string[] args)
        {
            try
            {
                TcpClient tcpClient = new TcpClient("127.0.0.1", 10000);
                Console.WriteLine("Connected");

                StreamReader reader = new StreamReader(tcpClient.GetStream());
                StreamWriter writer = new StreamWriter(tcpClient.GetStream());
                string s = "";

                using (var timeDomain = new PinnedArray<double>(inputSize))
                using (var frequencyDomain = new FftwArrayComplex(DFT.GetComplexBufferSize(timeDomain.GetSize())))
                using (var fft = FftwPlanRC.Create(timeDomain, frequencyDomain, DftDirection.Forwards))
                {
                    while (!(s = reader.ReadLine()).Equals("Quit") || (s == null))
                    {
                        string result = "";

                        if (IsValidJson(s))
                        {
                            result = ProcessMessage(s, timeDomain, frequencyDomain, fft);
                        }

                        writer.WriteLine(result);
                        writer.Flush();
                        GC.Collect();
                    }
                    reader.Close();
                    writer.Close();
                    tcpClient.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private static string ProcessMessage(string s, PinnedArray<double> pin,FftwArrayComplex com, FftwPlanRC fft)
        {
            //Deserialize and then FFTW then return datasample as json string
            WFOTransporter wav = JsonConvert.DeserializeObject<WFOTransporter>(s);
            WaveFileObject obj = new WaveFileObject(wav);
            DataSample sample = new DataSample(FFT(obj, pin, com, fft));
            string json = JsonConvert.SerializeObject(sample);
            return json;
        }

        private static bool IsValidJson(string strInput)
        {
            // Removes whitespace in front of string, or in the back
            strInput = strInput.Trim();

            // Looks for brackets
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                // Try to parse json string, which throws exception if it cant. 
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        static double[] FFT(WaveFileObject obj, PinnedArray<double> pin, FftwArrayComplex com, FftwPlanRC fft)
        {
            Console.WriteLine("FFT");
            double[] magnitudes;
            Console.WriteLine(obj.soundData.Count);
            double[] input = new double[obj.soundData.Count + 20286];
            Array.Clear(input, 0, input.Length);
            obj.soundData.CopyTo(input, 0);

            switch (obj.header.channels)
            {
                case 1:
                    for (int i = 0; i < pin.Length; i++)
                    {
                        pin[i] = input[i];
                        //Console.Write(pin[i] + " : ");
                    }
                    break;

                case 2:
                    for (int i = 0; i < pin.Length; i++)
                    {
                        pin[i] = input[i + i];
                        //Console.WriteLine(pin[i]);
                    }
                    break;

                default:
                    break;
            }

            fft.Execute();

            magnitudes = new double[com.Length];
            for (int i = 0; i < 4000; i++)
            {
                magnitudes[i] = 10 * Math.Log10((com[i].Magnitude / inputSize) * (com[i].Magnitude / inputSize));
                /*
                if (10 * Math.Log10((com[i].Magnitude / inputSize) * (com[i].Magnitude / inputSize)) > 10)
                {
                    Console.WriteLine("Bin: " + i * sampleRate / com.Length + " " + 10 * Math.Log10((com[i].Magnitude / inputSize) * (com[i].Magnitude / inputSize)));
                }
                */
            }

            Console.WriteLine(com.Length);
            Console.WriteLine();

            Console.WriteLine("Returning magnitudes");
            return magnitudes;
        }
    }
}


