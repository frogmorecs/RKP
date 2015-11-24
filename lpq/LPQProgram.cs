using System;
using common;

namespace lpq
{
    public class LPQProgram
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Displays the state of a remote lpd queue.");
                Console.WriteLine();
                Console.WriteLine("Usage: lpq -Sserver -Pprinter [-l]");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("      -S server   Name or ipaddress of the host providing lpd service");
                Console.WriteLine("      -P printer  Name of the print queue");
                Console.WriteLine("      -l          verbose output");
                Console.WriteLine();
                Console.WriteLine();

                return;
            }

            var job = CommandLineParser.ParseCommandLine<LPQJob>(args);
            var lines = LPRClient.QueryPrinter(job);

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

        }
    }
}
