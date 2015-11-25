using common;

namespace lpr
{
    class LPRProgram
    {
        static void Main(string[] args)
        {
            var parser = new CommandLineParser<LPRJob>();
            var job = parser.ParseCommandLine(args);

            ILPRClient lprClient = new LPRClient();
            lprClient.PrintFile(job);
        }
    }
}
