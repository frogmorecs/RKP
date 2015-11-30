using System;
using NUnit.Framework;

namespace common.tests
{
    [TestFixture]
    public class CommandLineParserTests
    {
        private CommandLineParser<LPQJob> _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new CommandLineParser<LPQJob>();
        }
        [Test]
        public void ParseCommandLineTest()
        {
            var cmd = "-Smypc -Pprinter -l file.prn".Split(' ');
            var job = _parser.ParseCommandLine(cmd);

            Assert.That(job, Is.EqualTo(new LPQJob {Server="mypc", Printer = "printer", Verbose = true}));
        }

        [Test]
        public void ParseCommandLineTestSpaces()
        {
            var cmd = "-S mypc -P printer file.prn".Split(' ');
            var job = _parser.ParseCommandLine(cmd);

            Assert.That(job, Is.EqualTo(new LPQJob {Server="mypc", Printer = "printer", Verbose = false}));
        }

        [Test]
        [ExpectedException(typeof (ParserException))]
        public void ParseCommandLineMissingServer()
        {
            var cmd = "-P printer file.prn".Split(' ');
            _parser.ParseCommandLine(cmd);
        }

        [Test]
        [ExpectedException(typeof (ParserException))]
        public void ParseCommandLineMissingPrinter()
        {
            var cmd = "-S server file.prn".Split(' ');
            _parser.ParseCommandLine(cmd);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ParseCommandLinePrinterShouldntHaveSpaces([Values('\x0B', '\x0C', '\x09', ' ')] char c)
        {
            var cmd = new[] {"-Sserver", $"-Pprinter{c}name"};
            _parser.ParseCommandLine(cmd);
        }

        [Test]
        public void ParseCommandLineWithDefaultParameter()
        {
            var parser = new CommandLineParser<LPRJob>();

            var cmd = "-S mypc -P printer file.prn".Split(' ');
            var job = parser.ParseCommandLine(cmd);

            Assert.That(job, Is.EqualTo(new LPRJob { Server = "mypc", Printer = "printer", FileType = "f", Path = "file.prn"}));
        }

        [Test]
        public void ParseCommandLineFileType()
        {
            var parser = new CommandLineParser<LPRJob>();

            var cmd = "-S mypc -P printer file.prn -o l".Split(' ');
            var job = parser.ParseCommandLine(cmd);

            Assert.That(job, Is.EqualTo(new LPRJob { Server = "mypc", Printer = "printer", FileType = "l", Path = "file.prn"}));
        }
    }
}
