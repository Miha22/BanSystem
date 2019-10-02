using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BanSystem
{
    class Client
    {
        private readonly string _address = "127.0.0.1";
        private IPEndPoint _ipPoint;
        privte Socket _socket;

        public Client()
        {
          _ipPoint = new IPEndPoint(IPAddress.Parse(_address), GlobalBan.Instance.Configuration.Instance.Socket_Port);
          _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
          // подключаемся к удаленному хосту
          _socket.Connect(ipPoint);
        }

        internal void Shutdown()
        {
          _socket.Shutdown(SocketShutdown.Both);
          _socket.Close();
        }

        internal void SendMessage(string message)
        {
          byte[] data = Encoding.Unicode.GetBytes(message);
          socket.Send(data);
        }

        internal static void Connect()
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(_address), GlobalBan.Instance.Configuration.Instance.Socket_Port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                string message = "";
                while (message != "!stop")
                {
                    //посылаем
                    Console.Write("Введите сообщение: ");
                    message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    socket.Send(data);


                    // получаем ответ
                    data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);
                    Console.WriteLine("ответ сервера: " + builder.ToString());

                    // закрываем сокет
                }
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
