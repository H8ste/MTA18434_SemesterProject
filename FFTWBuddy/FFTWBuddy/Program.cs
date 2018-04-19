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
        static double[] input;

        static void Main(string[] args)
        {
            try
            {
                TcpClient tcpClient = new TcpClient("127.0.0.1", 10000);
                Console.WriteLine("Connected");

                StreamReader reader = new StreamReader(tcpClient.GetStream());
                StreamWriter writer = new StreamWriter(tcpClient.GetStream());
                string s = "";

                while (!(s = reader.ReadLine()).Equals("Quit") || (s == null))
                {
                    string result = "";

                    if (IsValidJson(s))
                    {
                        result = ProcessMessage(s);
                    }

                    writer.WriteLine(result);
                    writer.Flush();
                    GC.Collect();
                }
                reader.Close();
                writer.Close();
                tcpClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static string ProcessMessage(string s)
        {
            //Deserialize and then FFTW then return datasample as json string
            WFOTransporter wav = JsonConvert.DeserializeObject<WFOTransporter>(s);
            WaveFileObject obj = new WaveFileObject(wav);
            DataSample sample = new DataSample(FFT(obj.soundData));
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

        static double[] FFT(IList list)
        {
            input = new double[list.Count];
            double[] magnitudes;
            list.CopyTo(input, 0);

            Console.WriteLine(list.Count);
            Console.WriteLine(input.Length);

            Console.WriteLine("Attempting FFT");

            using (var pinIn = new PinnedArray<double>(input))
            using (var comOut = new FftwArrayComplex(DFT.GetComplexBufferSize(pinIn.GetSize())))
            {
                DFT.FFT(pinIn, comOut);

                magnitudes = new double[comOut.Length];
                for (int i = 0; i < comOut.Length; i++)
                {
                    magnitudes[i] = comOut[i].Magnitude;
                }
            }

            Console.WriteLine("Returning magnitudes");
            return magnitudes;
        }     
    }
}


