using SimpleWebServer.MimeTypeMap;
using System;
using System.IO;
using System.Net.Sockets;

namespace SimpleWebServer.HTTP
{
    class HttpClient
    {
        private readonly Socket        _socket;
        private readonly NetworkStream _networkStream;
        private readonly StreamReader  _streamReader;
        private readonly MemoryStream  _memoryStream = new MemoryStream();
        private readonly string        _serverName = "Simple Demo HTTP Server";

        public const int    BUFFER_SIZE = 4096;

        // Root directory of your server
        // If you change this, update the build macro
        // because the build macro copy all the files in DefaultPage 
        // to this directory
        public const string ROOT_DIR = "www";
        public const string DEFAULT_404_PAGE = @"www\404.html";
        public const string DEFAULT_500_PAGE = @"www\500.html";

        #region HTTP Header Texts
        public const string HTTP_1_1_HEADER = @"HTTP/1.1 {0}
Server: {1}
Content-Length: {2}
Content-Type: {3}
Keep-Alive: Close

";
        #endregion HTTP Header Texts

        public HttpClient(Socket socket)
        {
            _socket = socket;
            _networkStream = new NetworkStream(socket, true);
            _streamReader = new StreamReader(_memoryStream);
        }

        public HttpClient(Socket socket, string ServerName): this(socket)
        {
            _serverName = ServerName;
        }

        public async void Run()
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            while(true)
            {
                int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length);

                // if there's no data, return and close connection
                if (bytesRead == 0)
                {
                    return;
                }

                _memoryStream.Seek(0, SeekOrigin.End);
                _memoryStream.Write(buffer, 0, bytesRead);

                bool done = ProcessHeader();
                if (done)
                {
                    break;
                }
            }
        }

        protected bool ProcessHeader()
        {
            while (true)
            {
                _memoryStream.Seek(0, SeekOrigin.Begin);
                var line = _streamReader.ReadLine();
                var data = line.Split(' ');

                var method  = data[0];
                var file    = data[1].TrimStart('/');
                var version = data[2];

                switch (method)
                {
                    case "GET":
                        if (String.IsNullOrWhiteSpace(file))
                        {
                            file = "index.html";
                        }

                        SendFile(ROOT_DIR + Path.DirectorySeparatorChar + file);

                        return true;
                }

                return false;
            }
        }

        private async void SendFile(string file)
        {
            byte[] data;
            string responseCode = "";
            string contentType  = "";

            try
            {
                if (File.Exists(file))
                {
                    data = File.ReadAllBytes(file);

                    contentType  = MimeTypeMap.MimeTypeMap.GetMimeType(Path.GetExtension(file).TrimStart('.'));
                    responseCode = HttpStatusDescription.Get(HttpStatusCode.OK);
                }
                else
                {
                    data = File.ReadAllBytes(DEFAULT_404_PAGE);

                    contentType  = MimeTypeMap.MimeTypeMap.GetMimeType("html");
                    responseCode = HttpStatusDescription.Get(HttpStatusCode.NotFound);
                }
            }
            catch(Exception e)
            {
                data = File.ReadAllBytes(DEFAULT_500_PAGE);

                Console.WriteLine("Exception Occurred!");
                Console.WriteLine(e.ToString());
                Console.WriteLine("\n");

                responseCode = HttpStatusDescription.Get(HttpStatusCode.InternalServerError);
            }

            string header   = String.Format(HTTP_1_1_HEADER, responseCode, _serverName, data.Length, contentType);
            var headerBytes = System.Text.Encoding.ASCII.GetBytes(header);

            await _networkStream.WriteAsync(headerBytes, 0, headerBytes.Length);
            await _networkStream.WriteAsync(data, 0, data.Length);
            await _networkStream.FlushAsync();

            _networkStream.Dispose();
        }
    }
}
