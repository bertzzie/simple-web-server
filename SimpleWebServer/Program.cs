using System;

using CommandLine;
using CommandLine.Text;

namespace SimpleWebServer
{
    # region Program Options
    class ProgramOptions
    {
        [Option('p', "port", Required = false,
            HelpText = "Nomor port untuk server",
            DefaultValue = 8080)]
        public int port { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("Simple Web Server", "1.0"),
                Copyright = new CopyrightInfo("Alex Xandra Albert Sim", 2015),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("Lisensi: bebas digunakan, tetapi ingat program hanya untuk demo.");
            help.AddPreOptionsLine("Penulis kode tidak bertanggung jawab atas kerusakan akibat penggunaan program.");
            help.AddOptions(this);

            return help;
        }
    }
    #endregion Program Options

    class Program
    {
        static void Main(string[] args)
        {
            int port;

            var options = new ProgramOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                port = options.port;
                Console.WriteLine("The Port Used is {0}", port);
            }
        }
    }
}
