using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace common
{
    public static class StreamExtensions
    {
        public static void Write(this Stream stream, string text)
        {
            var data = Encoding.ASCII.GetBytes(text);
            stream.Write(data, 0, data.Length);
        }
    }

    public interface ILPRClient
    {
        IEnumerable<string> QueryPrinter(LPQJob lpqJob);
        void PrintFile(LPRJob job);
    }

    public class LPRClient : ILPRClient
    {
        private const int LPRPort = 515;

        public IEnumerable<string> QueryPrinter(LPQJob lpqJob)
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

        public void PrintFile(LPRJob job)
        {
            using (var client = new TcpClient(job.Server, LPRPort))
            using (var stream = client.GetStream())
            {
                stream.Write($"\x02{job.Printer}\n");
                stream.Flush();

                CheckResult(stream);

                var controlFile = new StringBuilder();
                controlFile.Append($"H{Environment.MachineName}\n");
                controlFile.Append($"P{Environment.UserName}\n");
                controlFile.Append($"odfA001{Environment.MachineName}\n");
                controlFile.Append($"UdfA001{Environment.MachineName}\n");
                controlFile.Append($"N{job.Path}\n");

                stream.Write($"\x02{controlFile.Length} cfA001{Environment.MachineName}\n");
                stream.Flush();

                CheckResult(stream);

                stream.Write(controlFile.ToString());
                stream.WriteByte(0);
                stream.Flush();

                CheckResult(stream);

                var fileSize = new FileInfo(job.Path).Length;
                stream.Write($"\x03{fileSize} dfA001{Environment.MachineName}\n");
                stream.Flush();

                CheckResult(stream);

                var fileStream = new FileStream(job.Path, FileMode.Open);
                fileStream.CopyTo(stream);

                stream.WriteByte(0);
                stream.Flush();
            }
        }

        private static void CheckResult(NetworkStream stream)
        {
            var result = stream.ReadByte();
            if (result != 0)
            {
                throw new ApplicationException($"Unexpected response from server on receive job: {result}");
            }
        }
    }
}