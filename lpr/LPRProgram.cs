using System;
using lprshared;

namespace lpr
{
    internal class LPRProgram
    {
        static int Main(string[] args)
        {
            try
            {
                var parser = new CommandLineParser<LPRJob>();
                var job = parser.ParseCommandLine(args);

                IPrintClient lprClient = new PrintClient();

                var task = lprClient.PrintFileAsync(job);
                task.Wait();
            }
            catch (ParserException)
            {
                Console.WriteLine("Sends a print job to a network printer");
                Console.WriteLine();
                Console.WriteLine("Usage: lpr -S server -P printer [-C class] [-J job] [-o option] [-x] [-d] filename");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("      -S server   Name or ipaddress of the host providing lpd service");
                Console.WriteLine("      -P printer  Name of the print queue");
                Console.WriteLine("      -C class    Job classification for use on the burst page");
                Console.WriteLine("      -J job      Job name to print on the burst page");
                Console.WriteLine("      -o option   Indicates type of the file (by default assumes a text file)");
                Console.WriteLine("                  Use \"-o l\" for binary (e.g. postscript) files");
                Console.WriteLine("      -x          Compatibility with SunOS 4.1.x and prior");
                Console.WriteLine("      -d          Send data file first");

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
