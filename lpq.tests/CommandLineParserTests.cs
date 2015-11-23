using System;
using NUnit.Framework;

namespace lpq.tests
{
    [TestFixture]
    public class CommandLineParserTests
    {
        [Test]
        public void ParseCommandLineTest()
        {
            var cmd = "-Smypc -Pprinter -l file.prn".Split(' ');
            var job = CommandLineParser.ParseCommandLine(cmd);

            Assert.That(job, Is.EqualTo(new Job {Server="mypc", Printer = "printer", Verbose = true}));
        }

        [Test]
        public void ParseCommandLineTestSpaces()
        {
            var cmd = "-S mypc -P printer file.prn".Split(' ');
            var job = CommandLineParser.ParseCommandLine(cmd);

            Assert.That(job, Is.EqualTo(new Job {Server="mypc", Printer = "printer", Verbose = false}));
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ParseCommandLineMissingServer()
        {
            var cmd = "-P printer file.prn".Split(' ');
            CommandLineParser.ParseCommandLine(cmd);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ParseCommandLineMissingPrinter()
        {
            var cmd = "-S server file.prn".Split(' ');
            CommandLineParser.ParseCommandLine(cmd);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ParseCommandLinePrinterShouldntHaveSpaces()
        {
            var cmd = new[] {"-Sserver", "-Pprinter name"};
            CommandLineParser.ParseCommandLine(cmd);
        }
    }
}
