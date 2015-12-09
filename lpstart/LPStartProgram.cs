using System;
using lprshared;

namespace lpstart
{
    internal class LPStartProgram
    {
        static int Main(string[] args)
        {
            try
            {
                var parser = new CommandLineParser<LPStartJob>();
                var job = parser.ParseCommandLine(args);

                IPrintClient lprClient = new PrintClient();
                var task = lprClient.StartAsync(job);
                task.Wait();
            }
            catch (ParserException)
            {
                Console.WriteLine("Start a remote lpd queue.");
                Console.WriteLine();
                Console.WriteLine("Usage: lpstart -Sserver -Pprinter");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("      -S server   Name or ipaddress of the host providing lpd service");
                Console.WriteLine("      -P printer  Name of the print queue");
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
