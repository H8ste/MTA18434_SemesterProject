using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Sockets;

namespace FFTWBuddy
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                TcpClient tcpClient = new TcpClient("127.0.0.1", 10000);
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
            //Deserialize and then FFTW

            //Serialize to json string again and return
            return null;
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
    }
}
