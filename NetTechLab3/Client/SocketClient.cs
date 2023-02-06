using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetTechLab3.Client
{
    public class SocketClient
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static string ip = "192.168.0.111";
        public static string Send(string request)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(request);
            _clientSocket.Send(buffer);

            byte[] receiveBuffer = new byte[1024];
            int rec = _clientSocket.Receive(receiveBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receiveBuffer, data, rec);

            string receive = Encoding.ASCII.GetString(data);   
            
            return receive;
        }

        public static void LoopConnect()
        {
            int attempts = 0;

            while (!_clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(IPAddress.Parse(ip), 100);
                }
                catch (SocketException)
                {
                    
                }
            }
            MessageBox.Show("Connection attempts: " + attempts);
        }
    }
}
