using System;
using lprshared;

namespace lpq
{
    public class LPQProgram
    {
        static int Main(string[] args)
        {
            try
            {
                var parser = new CommandLineParser<LPQJob>();
                var job = parser.ParseCommandLine(args);

                IPrintClient lprClient = new PrintClient();
                var task = lprClient.QueryPrinterAsync(job);

                foreach (var line in task.Result)
                {
                    Console.WriteLine(line);
                }
            }
            catch (ParserException)
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

                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 2;
            }

            return 0;
        }
    }
}
