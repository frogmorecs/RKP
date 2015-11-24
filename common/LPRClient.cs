using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace common
{
    public static class LPRClient
    {
        private const int LPRPort = 515;

        public static IEnumerable<string> QueryPrinter(LPQJob lpqJob)
        {
            using (var client = new TcpClient(lpqJob.Server, LPRPort))
            using (var stream = client.GetStream())
            using (var streamWriter = new StreamWriter(stream, Encoding.ASCII))
            using (var streamReader = new StreamReader(stream, Encoding.ASCII))
            {
                var code = lpqJob.Verbose ? '\x04' : '\x03';
                streamWriter.Write($"{code}{lpqJob.Printer} \n");
                streamWriter.Flush();

                while (!streamReader.EndOfStream)
                {
                    yield return streamReader.ReadLine();
                }
            }
        }
    }
}