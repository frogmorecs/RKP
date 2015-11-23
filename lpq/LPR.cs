using System.Collections.Generic;
using System.IO;
using System.Text;

namespace lpq
{
    public class LPR
    {
        public static IEnumerable<string> QueryPrinter(Stream stream, Job job)
        {
            using (var streamWriter = new StreamWriter(stream, Encoding.ASCII))
            using (var streamReader = new StreamReader(stream, Encoding.ASCII))
            {
                streamWriter.Write($"\x03{job.Printer} \n");
                streamWriter.Flush();

                while (!streamReader.EndOfStream)
                {
                    yield return streamReader.ReadLine();
                }
            }
        }
    }
}