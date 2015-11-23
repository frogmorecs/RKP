using System;
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

            var client = new TcpClient(job.Server, 515);
            var stream = client.GetStream();

            var streamWriter = new StreamWriter(stream, Encoding.ASCII);
            streamWriter.Write($"\x03{job.Printer} \n");
            streamWriter.Flush();

            var streamReader = new StreamReader(stream, Encoding.ASCII);
            while (!streamReader.EndOfStream)
            {
                Console.WriteLine(streamReader.ReadLine());
            }
        }
    }
}
