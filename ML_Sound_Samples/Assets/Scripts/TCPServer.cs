using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient tcpClient;

    public void Initialize()
    {
        // Start TcpServer background thread 		
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    private void ListenForIncommingRequests()
    {
        try
        {	
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 10000);
            tcpListener.Start();
            Debug.Log("Server is listening");

            tcpClient = tcpListener.AcceptTcpClient();

            Debug.Log("Accepted");

            try
            {
                StreamReader reader = new StreamReader(tcpClient.GetStream());
                StreamWriter writer = new StreamWriter(tcpClient.GetStream());
                string s = "";

                while (!(s = reader.ReadLine()).Equals("Quit") || (s == null))
                {
                    HandleJsonMessage(s);
                    writer.WriteLine("From Server: Thank you for the contribution to the Animal Database");
                    writer.Flush();
                }

                // Closes the different objects
                reader.Close();
                writer.Close();
                tcpClient.Close();
            }
            catch (IOException e)
            {
                Debug.Log(e.Message);
                throw;
            }
            // Close client after exception
            finally
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
            }

        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    private void HandleJsonMessage(string s)
    {
        if (IsValidJson(s))
        {

        }
    }

    public void SendMsg(string message)
    {
        if (tcpClient == null)
        {
            return;
        }

        try
        {
            StreamWriter writer = new StreamWriter(tcpClient.GetStream());
            writer.WriteLine(message);
            writer.Flush();
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
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
                var obj = JsonUtility.FromJson<MonoBehaviour>(strInput);
                return true;
            }
            catch (Exception ex) //some other exception
            {
                Debug.Log(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}