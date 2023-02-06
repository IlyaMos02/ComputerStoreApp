using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Reflection.PortableExecutable;
using Server._Repository;
using Newtonsoft.Json;
using Server.Model;
using System.Text;

namespace Server
{
    internal class Program
    {       
        private static byte[] _buffer = new byte[1024];
        private static List<Socket> _clientSockets = new List<Socket>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);

        private static string SQLConnection = "Data Source=(local);Initial Catalog=NetTechLab3;Integrated Security=True";
        static AdminRepository ARepository;
        static ProductRepository PRepository;

        private static int _threadSafeLocker = 0;
        private static bool Locker
        {
            get { return (Interlocked.CompareExchange(ref _threadSafeLocker, 1, 1) == 1); }
            set
            {
                if (value)
                    Interlocked.CompareExchange(ref _threadSafeLocker, 1, 0);
                else
                    Interlocked.CompareExchange(ref _threadSafeLocker, 0, 1);
            }
        }
    

        static void Main(string[] args)
        {           
            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = _serverSocket.EndAccept(AR);
            _clientSockets.Add(socket);
            Console.WriteLine("Client connected");
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int received = socket.EndReceive(AR);
            byte[] dataBuf = new byte[received];
            Array.Copy(_buffer, dataBuf, received);

            string text = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine("Text received: " + text);

            string response = string.Empty;

            if (!Locker)
                response = ReceiveQuerries(text, SQLConnection);
            else
                response = "locked";

            byte[] data = Encoding.ASCII.GetBytes(response);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static string ReceiveQuerries(string receive, string connection)
        {
            string response = string.Empty;

            if (receive.Contains("aut"))
            {
                ARepository = new AdminRepository(connection);
                string[] text = receive.Split(":");

                bool result = ARepository.GetAdmin(text[1], text[2]);
                response = "aut:" + text[1] + ":" + result.ToString();
                Console.WriteLine("Receive autorization.\nResponse: " + result.ToString());

                return response;
            }
            if (receive.Contains("getProd"))
            {
                PRepository = new ProductRepository(connection);

                var ProductList = PRepository.GetProducts();
                Console.WriteLine("Receive get products");

                response = JsonConvert.SerializeObject(ProductList);
                return response;
            }
            if (receive.Contains("search"))
            {
                PRepository = new ProductRepository(connection);
                string[] text = receive.Split(":");

                var ProductList = PRepository.GetProductsByValue(text[1]);
                Console.WriteLine("Receive search products");

                response = JsonConvert.SerializeObject(ProductList);
                return response;
            }
            if (receive.Contains("addProd"))
            {
                Locker = true;
                PRepository = new ProductRepository(connection);
                string[] text = receive.Split("|");

                var product = JsonConvert.DeserializeObject<Product>(text[1]);
                Console.WriteLine("Receive add product " + product.ToString());
                response = PRepository.AddProduct(product).ToString();

                Locker = false;
                return response;
            }
            if (receive.Contains("editProd"))
            {
                Locker = true;
                PRepository = new ProductRepository(connection);
                string[] text = receive.Split("|");

                var product = JsonConvert.DeserializeObject<Product>(text[1]);
                Console.WriteLine("Receive edit product " + product.ToString());
                response = PRepository.EditProduct(product).ToString();

                Locker = false;
                return response;
            }
            if (receive.Contains("deleteProd"))
            {
                Locker = true;
                PRepository = new ProductRepository(connection);
                string[] text = receive.Split(":");

                Console.WriteLine("Receive delete product with id " + text[1]);
                Locker = false;
                response = PRepository.DeleteProduct(Convert.ToInt32(text[1])).ToString();
            }
            return "0";
        }

        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }       
    }
}