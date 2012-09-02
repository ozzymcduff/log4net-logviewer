using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LogViewer;
using System.IO;
using System.Threading;
namespace IntegrationTests
{
    [TestFixture]
    public class WatcherTests
    {
        private string _buffer =
             @"<log4net:event logger=""IntegrationTests.LogTests"" timestamp=""2011-03-23T21:39:31.3833441+01:00"" level=""ERROR"" thread=""7"" domain=""IsolatedAppDomainHost: IntegrationTests"" username=""AWESOMEMACHINE\Administrator""><log4net:message>msg</log4net:message><log4net:properties><log4net:data name=""log4net:HostName"" value=""AWESOMEMACHINE"" /></log4net:properties><log4net:exception>System.Exception: test</log4net:exception><log4net:locationInfo class=""IntegrationTests.LogTests"" method=""TestLog"" file=""C:\projects\LogViewer\IntegrationTests\LogTests.cs"" line=""19"" /></log4net:event>";

        [Test]
        public void Can_detect_changes_to_file()
        {
            var file = Path.Combine(".", "testfile1.xml");
            if (File.Exists(file)) { File.Delete(file); }
            File.WriteAllText(file, _buffer);
            var files = new List<LogEntry>();
            using (var watcher = new Watcher(new FileWithPosition(file))
            {
                logentry = l => { files.Add(l); }
            })
            {
                watcher.Init();
                Assert.That(files.Count, Is.EqualTo(1));
                File.AppendAllText(file, _buffer);
                Thread.Sleep(100/*750*3*/);
                Assert.That(files.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void Can_handle_rolling_log()
        {
            var file = Path.Combine(".", "testfile2.xml");
            if (File.Exists(file)) { File.Delete(file); }
            File.WriteAllText(file, _buffer);
            var outofbounds = 0;
            var files = new List<LogEntry>();
            using (var watcher = new Watcher(new FileWithPosition(file))
            {
                logentry = l => { files.Add(l); },
                outOfBounds = () => { outofbounds++; }
            })
            {
                watcher.Init();

                File.WriteAllText(file, "");
                Thread.Sleep(100/*750*3*/);
                Assert.That(outofbounds, Is.EqualTo(1));
            }
        }
    }
}
