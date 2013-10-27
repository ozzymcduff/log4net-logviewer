using System;
using System.Linq;
using NUnit.Framework;
using System.IO;
using LogViewer;
using log4net.Core;
using log4net.Layout;
using LogViewer.Infrastructure;

namespace IntegrationTests
{
    [TestFixture]
    public class Log4NetTests
    {
        private string _buffer =
          @"<log4net:event 
	logger=""IntegrationTests.LogTests"" 
	timestamp=""2011-03-23T21:39:31.3833441+01:00"" 
	level=""ERROR"" 
	thread=""7"" 
	domain=""IsolatedAppDomainHost: IntegrationTests"" 
username=""AWESOMEMACHINE\Administrator"">
<log4net:message>msg</log4net:message>
<log4net:properties>
	<log4net:data name=""log4net:HostName"" value=""AWESOMEMACHINE"" />
</log4net:properties>
<log4net:exception>System.Exception: test</log4net:exception>
<log4net:locationInfo class=""IntegrationTests.LogTests"" 
	method=""TestLog"" 
	file=""C:\projects\LogViewer\IntegrationTests\LogTests.cs"" line=""19"" /></log4net:event>";
        [Test]
        public void ParseStream()
        {
            using (var s = new MemoryStream())
            using (var w = new StreamWriter(s))
            {
                w.Write(_buffer);
                w.Flush();
                s.Position = 0;
                var entry = new LogEntryParser().Parse(s).Single();
                Assert.That(entry.Data.Level.Name, Is.EqualTo("ERROR"));
                Assert.That(entry.HostName, Is.EqualTo(@"AWESOMEMACHINE"));
                Assert.That(entry.Data.Domain, Is.EqualTo(@"IsolatedAppDomainHost: IntegrationTests"));
                Assert.That(entry.Data.Message, Is.EqualTo("msg"));
                Assert.That(entry.Class(), Is.EqualTo("IntegrationTests.LogTests"));
                Assert.That(entry.Method(), Is.EqualTo("TestLog"));
                Assert.That(entry.Line(), Is.EqualTo("19"));
                Assert.That(entry.File(), Is.EqualTo(@"C:\projects\LogViewer\IntegrationTests\LogTests.cs"));
            }
        }
        [Test]
        public void Parse3()
        {
            using (var s = new MemoryStream())
            using (var w = new StreamWriter(s))
            {

                w.Write(_buffer);
                w.Flush();
                s.Position = 0;

                var entry = new LogEntryParser().Parse(s).Single();
                Assert.That(entry.Data.Level.Name, Is.EqualTo("ERROR"));
                Assert.That(entry.HostName, Is.EqualTo(@"AWESOMEMACHINE"));
                Assert.That(entry.Data.Domain, Is.EqualTo(@"IsolatedAppDomainHost: IntegrationTests"));
                Assert.That(entry.Data.Message, Is.EqualTo("msg"));
                Assert.That(entry.Class(), Is.EqualTo("IntegrationTests.LogTests"));
                Assert.That(entry.Method(), Is.EqualTo("TestLog"));
                Assert.That(entry.Line(), Is.EqualTo("19"));
                Assert.That(entry.File(), Is.EqualTo(@"C:\projects\LogViewer\IntegrationTests\LogTests.cs"));
            }
        }
        [Test]
        public void ParseStreamAtPosition()
        {
            long p = 0;
            var path = Path.GetTempFileName();
            using (var s = new FileStream(path,
                FileMode.Truncate,
                FileAccess.ReadWrite))
            using (var w = new StreamWriter(s))
            {
                w.Write(_buffer);
                w.Flush();
                s.Position = 0;
                var entry = new LogEntryParser().Parse(s).Single();// read written entry
                p = s.Position;
            }
            using (var s = new FileStream(path,
                FileMode.Append,
                FileAccess.Write))
            using (var w = new StreamWriter(s))
            {
                w.Write(_buffer);
                w.Flush();
            }

            using (var s = FileUtil.OpenReadOnly(path, position: p))
            {
                var entry = new LogEntryParser().Parse(s).Single();
                Assert.That(entry.Data.Level.Name, Is.EqualTo("ERROR"));
                Assert.That(entry.HostName, Is.EqualTo(@"AWESOMEMACHINE"));
                Assert.That(entry.Data.Domain, Is.EqualTo(@"IsolatedAppDomainHost: IntegrationTests"));
                Assert.That(entry.Data.Message, Is.EqualTo("msg"));
                Assert.That(entry.Class(), Is.EqualTo("IntegrationTests.LogTests"));
                Assert.That(entry.Method(), Is.EqualTo("TestLog"));
                Assert.That(entry.Line(), Is.EqualTo("19"));
                Assert.That(entry.File(), Is.EqualTo(@"C:\projects\LogViewer\IntegrationTests\LogTests.cs"));
            }
        }
        [Test]
        public void ParseStreamAtPositionShouldThrowException()
        {
            long p = 0;
            var path = Path.GetTempFileName();
            using (var s = new FileStream(path,
                FileMode.Truncate,
                FileAccess.ReadWrite))
            using (var w = new StreamWriter(s))
            {
                w.Write(_buffer);
                w.Flush();
                s.Position = 0;
                var entry = new LogEntryParser().Parse(s).Single();// read written entry
                p = s.Position;
            }
            Assert.Throws<OutOfBoundsException>(() =>
            {
                using (var s = FileUtil.OpenReadOnly(path, position: p * 10))
                {
                    new LogEntryParser().Parse(s);
                }
            });
        }
        [Test]
        public void Test()
        {
            var layout = new PatternLayout("%date [%thread] %-5level %logger - %message%newline");

            var stringWriter = new StringWriter();
            layout.Format(stringWriter, new LoggingEvent(new LoggingEventData
                                                             {
                                                                 Level = Level.Error,
                                                                 Domain = "Domain",
                                                                 Message = "msg",
                                                                 ThreadName = "thread",
                                                                 LoggerName = "logger",
                                                                 TimeStamp = new DateTime(2001, 1, 1)
                                                             }));
            stringWriter.Flush();
            Assert.That(stringWriter.ToString(), Is.EqualTo("2001-01-01 00:00:00,000 [thread] ERROR logger - msg" + Environment.NewLine));
        }
    }
}