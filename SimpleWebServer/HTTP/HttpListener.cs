using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleWebServer.HTTP
{
    class HttpListener
    {
        private readonly int         _port;
        private readonly TcpListener _tcpListener;
        private readonly Task        _listenerTask;

        public HttpListener(int port)
        {
            _port = port;

            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _tcpListener.Start();

            _listenerTask = Task.Factory.StartNew(() => ListenLoop());
        }

        private async void ListenLoop()
        {
            while (true)
            {
                var socket = await _tcpListener.AcceptSocketAsync();
                if (socket == null)
                {
                    break;
                }

                var server = new HttpServer(socket);

                await Task.Factory.StartNew(server.Run);
            }
        }
    }
}
