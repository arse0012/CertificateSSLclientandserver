using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace EchoServer
{
    internal class Server
    {
        private int PORT;

        public Server(int port)
        {
            this.PORT = port;
        }

        public void Start()
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, PORT);
            serverSocket.Start();
            Console.WriteLine("Server started");

            using (TcpClient connectionSocket = serverSocket.AcceptTcpClient())
            using (Stream ns = connectionSocket.GetStream())
            {
                string serverCertificateFile = "c:/Users/arsen/Certificates/ServerSSL.pfx";
                bool clientCertifiateRequired = false;
                bool checkCertificateRevocation = true;
                SslProtocols enabledSSLProtocols = SslProtocols.Tls;

                X509Certificate serverCertificate = new X509Certificate2(serverCertificateFile, "mysecret");

                bool leaveInnerStreamOpen = false;
                SslStream sslStream = new SslStream(ns, leaveInnerStreamOpen);
                sslStream.AuthenticateAsServer(serverCertificate, clientCertifiateRequired,
                    enabledSSLProtocols, checkCertificateRevocation);

                using (StreamReader sr = new StreamReader(sslStream))
                using (StreamWriter sw = new StreamWriter(sslStream))
                {
                    Console.WriteLine("Server activated");
                    sw.AutoFlush = true; // enable automatic flushing

                    string message = sr.ReadLine(); // read string from client
                    string answer = "";
                    while (!string.IsNullOrEmpty(message))
                    {

                        Console.WriteLine("Client: " + message);
                        answer = message.ToUpper(); // convert string to upper case
                        sw.WriteLine(answer); // send back upper case string
                        message = sr.ReadLine();

                    }
                }
            }
        }
    }
}