using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace lpq
{
    public class LPQProgram
    {
        static void Main(string[] args)
        {
            var job = CommandLineParser.ParseCommandLine(args);

            using (var client = new TcpClient(job.Server, 515))
            using (var stream = client.GetStream())
            {
                var lines = LPR.QueryPrinter(stream, job);
                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
            }

        }
    }
}
