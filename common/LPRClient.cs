using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace common
{
    public static class LPRClient
    {
        private const int LPRPort = 515;

        public static IEnumerable<string> QueryPrinter(Job job)
        {
            using (var client = new TcpClient(job.Server, LPRPort))
            using (var stream = client.GetStream())
            using (var streamWriter = new StreamWriter(stream, Encoding.ASCII))
            using (var streamReader = new StreamReader(stream, Encoding.ASCII))
            {
                var code = job.Verbose ? '\x04' : '\x03';
                streamWriter.Write($"{code}{job.Printer} \n");
                streamWriter.Flush();

                while (!streamReader.EndOfStream)
                {
                    yield return streamReader.ReadLine();
                }
            }
        }
    }
}