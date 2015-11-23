using System;
using System.Linq;

namespace common
{
    public class CommandLineParser
    {
        public static Job ParseCommandLine(string[] args)
        {
            var job = new Job();

            for (int i = 0; i < args.Length; i++)
            {
                var currentArgument = args[i];

                if (currentArgument.StartsWith("-S"))
                {
                    job.Server = currentArgument.Length > 2 ? currentArgument.Substring(2) : args[++i];
                }
                else if (currentArgument.StartsWith("-P"))
                {
                    job.Printer = currentArgument.Length > 2 ? currentArgument.Substring(2) : args[++i];
                }
                else if (currentArgument.StartsWith("-l"))
                {
                    job.Verbose = true;
                }
            }

            if (job.Server == null)
            {
                throw new ArgumentException("Missing server parameter (-S)");
            }

            if (job.Printer == null)
            {
                throw new ArgumentException("Missing printer parameter (-P)");
            }

            if (new[] {" ", "\x08", "\x09", "\x0C"}.Any(c => job.Printer.Contains(c)))
            {
                throw new ArgumentException("Printer name cannot contain spaces.");
            }

            return job;
        }
    }
}