using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace common
{
    public interface ILPRClient
    {
        IEnumerable<string> QueryPrinter(LPQJob lpqJob);
        void PrintFile(LPRJob job);
    }

    public class LPRClient : ILPRClient
    {
        private const int LPRPort = 515;
        private static int _jobNumber;

        public IEnumerable<string> QueryPrinter(LPQJob lpqJob)
        {
            using (var client = new TcpClient(lpqJob.Server, LPRPort))
            using (var stream = client.GetStream())
            using (var streamReader = new StreamReader(stream, Encoding.ASCII))
            {
                var code = lpqJob.Verbose ? '\x04' : '\x03';
                stream.WriteASCII($"{code}{lpqJob.Printer} \n");

                while (!streamReader.EndOfStream)
                {
                    yield return streamReader.ReadLine();
                }
            }
        }

        public void PrintFile(LPRJob job)
        {
            var machineName = string.Join("", Environment.MachineName.Where(c => c > 32 && c < 128));
            var userName = string.Join("", Environment.UserName.Where(c => c > 32 && c < 128));

            _jobNumber = _jobNumber % 999 + 1;
            var jobIdentifier = $"{_jobNumber:D3}{machineName}";

            using (var client = new TcpClient(job.Server, LPRPort))
            using (var stream = client.GetStream())
            {
                if (job.SendDataFileFirst)
                {
                    // TODO Check this when we have a working server
                    WriteDataFile(job, stream, jobIdentifier);
                    WriteControlFile(job, stream, machineName, userName, jobIdentifier);
                }
                else
                {
                    WriteControlFile(job, stream, machineName, userName, jobIdentifier);
                    WriteDataFile(job, stream, jobIdentifier);
                }
            }
        }

        private static void WriteControlFile(LPRJob job, NetworkStream stream, string machineName, string userName, string jobIdentifier)
        {
            stream.WriteASCII($"\x02{job.Printer}\n");
            CheckResult(stream);

            var controlFile = new StringBuilder();
            controlFile.Append($"H{machineName}\n");
            controlFile.Append($"P{userName}\n");
            controlFile.Append($"{job.FileType}dfA{jobIdentifier}\n");
            controlFile.Append($"UdfA{jobIdentifier}\n");
            controlFile.Append($"N{job.Path}\n");

            stream.WriteASCII($"\x02{controlFile.Length} cfA{jobIdentifier}\n");
            CheckResult(stream);

            stream.WriteASCII(controlFile.ToString());
            stream.WriteByte(0);
            CheckResult(stream);
        }

        private static void WriteDataFile(LPRJob job, NetworkStream stream, string jobIdentifier)
        {
            var fileSize = new FileInfo(job.Path).Length;
            stream.WriteASCII($"\x03{fileSize} dfA{jobIdentifier}\n");
            CheckResult(stream);

            var fileStream = new FileStream(job.Path, FileMode.Open);
            fileStream.CopyTo(stream);
            stream.WriteByte(0);
            CheckResult(stream);
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