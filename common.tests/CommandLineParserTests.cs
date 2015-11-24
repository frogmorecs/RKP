using System;
using lpq;
using NUnit.Framework;

namespace common.tests
{
    [TestFixture]
    public class CommandLineParserTests
    {
        [Test]
        public void ParseCommandLineTest()
        {
            var cmd = "-Smypc -Pprinter -l file.prn".Split(' ');
            var job = CommandLineParser<LPQJob>.ParseCommandLine(cmd);

            Assert.That(job, Is.EqualTo(new LPQJob {Server="mypc", Printer = "printer", Verbose = true}));
        }

        [Test]
        public void ParseCommandLineTestSpaces()
        {
            var cmd = "-S mypc -P printer file.prn".Split(' ');
            var job = CommandLineParser<LPQJob>.ParseCommandLine(cmd);

            Assert.That(job, Is.EqualTo(new LPQJob {Server="mypc", Printer = "printer", Verbose = false}));
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ParseCommandLineMissingServer()
        {
            var cmd = "-P printer file.prn".Split(' ');
            CommandLineParser<LPQJob>.ParseCommandLine(cmd);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ParseCommandLineMissingPrinter()
        {
            var cmd = "-S server file.prn".Split(' ');
            CommandLineParser<LPQJob>.ParseCommandLine(cmd);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ParseCommandLinePrinterShouldntHaveSpaces([Values('\x0B', '\x0C', '\x09', ' ')] char c)
        {
            var cmd = new[] {"-Sserver", $"-Pprinter{c}name"};
            CommandLineParser<LPQJob>.ParseCommandLine(cmd);
        }
    }
}
