using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace common
{
    public interface ILPRClient
    {
        IEnumerable<string> QueryPrinter(LPQJob job);
        Task<List<string>> QueryPrinterAsync(LPQJob lpqJob);

        void PrintFile(LPRJob job);
        Task PrintFileAsync(LPRJob job);
    }

    public class LPRClient : ILPRClient
    {
        private class ConnectInfo<T>
        {
            public TcpClient Client { get; set; }
            public T Job { get; set; }
        }

        private const int LPRPort = 515;
        private static int _jobNumber;

        public IEnumerable<string> QueryPrinter(LPQJob job)
        {
            var connectionInfo = new ConnectInfo<LPQJob>
            {
                Job = job,
                Client = new TcpClient(job.Server, LPRPort)
            };

            return QueryPrinter(connectionInfo);
        }

        public Task<List<string>> QueryPrinterAsync(LPQJob job)
        {
            var client = new TcpClient();
            var connectInfo = new ConnectInfo<LPQJob>
            {
                Client = client,
                Job = job,
            };

            return Task.Factory.FromAsync(client.BeginConnect, client.EndConnect, job.Server, LPRPort, connectInfo)
                               .ContinueWith(OnConnectLPQ);
        }

        private static List<string> OnConnectLPQ(IAsyncResult result)
        {
            return QueryPrinter((ConnectInfo<LPQJob>) result.AsyncState).ToList();
        }

        private static IEnumerable<string> QueryPrinter(ConnectInfo<LPQJob> connectInfo)
        {
            using (var client = connectInfo.Client)
            using (var stream = client.GetStream())
            using (var streamReader = new StreamReader(stream, Encoding.ASCII))
            {
                var code = connectInfo.Job.Verbose ? '\x04' : '\x03';
                stream.WriteASCII($"{code}{connectInfo.Job.Printer} \n");

                while (!streamReader.EndOfStream)
                {
                    yield return streamReader.ReadLine();
                }
            }
        }

        public void PrintFile(LPRJob job)
        {
            PrintFileAsync(job).Wait(); // TODO Make synchronous
        }

        public Task PrintFileAsync(LPRJob job)
        {
            var client = new TcpClient();
            var connectInfo = new ConnectInfo<LPRJob>
            {
                Client = client,
                Job = job,
            };

            return Task.Factory.FromAsync(client.BeginConnect, client.EndConnect, job.Server, LPRPort, connectInfo)
                               .ContinueWith(OnConnectLPR);
        }

        private static void OnConnectLPR(IAsyncResult result)
        {
            var connectInfo = (ConnectInfo<LPRJob>) result.AsyncState;

            using (var client = connectInfo.Client)
            using (var stream = client.GetStream())
            {
                var machineName = string.Join("", Environment.MachineName.Where(c => c > 32 && c < 128));
                var userName = string.Join("", Environment.UserName.Where(c => c > 32 && c < 128));

                var newJobNumber = GetNextJobNumber();

                var jobIdentifier = $"{newJobNumber:D3}{machineName}";

                stream.WriteASCII($"\x02{connectInfo.Job.Printer}\n");
                CheckResult(stream);

                if (connectInfo.Job.SendDataFileFirst)
                {
                    WriteDataFile(connectInfo.Job, stream, jobIdentifier);
                    WriteControlFile(connectInfo.Job, stream, machineName, userName, jobIdentifier);
                }
                else
                {
                    WriteControlFile(connectInfo.Job, stream, machineName, userName, jobIdentifier);
                    WriteDataFile(connectInfo.Job, stream, jobIdentifier);
                }
            }
        }

        private static void WriteControlFile(LPRJob job, NetworkStream stream, string machineName, string userName, string jobIdentifier)
        {
            var controlFile = new StringBuilder();
            controlFile.Append($"H{machineName}\n");
            controlFile.Append($"P{userName}\n");
            controlFile.Append($"{job.FileType}dfA{jobIdentifier}\n");
            controlFile.Append($"UdfA{jobIdentifier}\n");
            controlFile.Append($"N{job.Path}\n");

            if (job.Class != null)
            {
                controlFile.Append($"C{job.Class}\n");
            }
            if (job.JobName != null)
            {
                controlFile.Append($"J{job.JobName}\n");
            }

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

        private static int GetNextJobNumber()
        {
            return Interlocked.Increment(ref _jobNumber) % 999 + 1;
        }
    }
}