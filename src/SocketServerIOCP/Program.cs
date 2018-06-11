using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServerIOCP
{
    class Program
    {
        static void Main(string[] args)
        {
            ushort port = 8010;
            Console.WriteLine("Creating 2000 initial client server with receive buffer size of 4096");
            Console.WriteLine("Creating 20 Accept Args");
            IPEndPoint server_ep = new IPEndPoint(IPAddress.Any, port);
            var userserver = new Server(2000, 4096, 20);

            Console.WriteLine("Setting delegated OnNewConnection");
            userserver.OnNewConnection = OnNewUserConnection;

            Console.WriteLine("Starting TCP Server with NoDelay option set on port " + port);
            userserver.Start(server_ep, SocketOptionName.NoDelay);

            Console.ReadLine();
        }

        static Connection OnNewUserConnection(Server serv, Socket socket)
        {
            var newconn = new HttpConnection(serv, socket);
            return newconn;
        }
    }
}
