using System;

namespace lpq
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
                throw new ApplicationException("Missing server parameter (-S)");
            }

            if (job.Printer == null)
            {
                throw new ApplicationException("Missing printer parameter (-P)");
            }

            return job;
        }
    }
}