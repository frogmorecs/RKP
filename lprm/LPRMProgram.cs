using System;
using lprshared;

namespace lprm
{
    internal class LPRMProgram
    {
        private static int Main(string[] args)
        {
            try
            {
                var parser = new CommandLineParser<LPRMJob>();
                var job = parser.ParseCommandLine(args);

                IPrintClient lprClient = new PrintClient();
                var task = lprClient.CancelAsync(job);
                task.Wait();
            }
            catch (ParserException)
            {
                Console.WriteLine("Displays the state of a remote lpd queue.");
                Console.WriteLine();
                Console.WriteLine("Usage: lprm -Sserver -Pprinter -Jid");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("      -S server   Name or ipaddress of the host providing lpd service");
                Console.WriteLine("      -P printer  Name of the print queue");
                Console.WriteLine("      -J          Id of the job");
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
