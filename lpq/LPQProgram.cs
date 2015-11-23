using System;
using common;

namespace lpq
{
    public class LPQProgram
    {
        static void Main(string[] args)
        {
            var job = CommandLineParser.ParseCommandLine(args);
            var lines = LPRClient.QueryPrinter(job);

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

        }
    }
}
