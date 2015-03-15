using Xunit;
using LogViewer;
using System.IO;
using System.Threading;
using LogViewer.Infrastructure;
using System.Collections.Concurrent;
using System;

namespace IntegrationTests.FileChanges
{
	public class PollerTests
    {
        private string _buffer =
             @"<log4net:event logger=""IntegrationTests.LogTests"" timestamp=""2011-03-23T21:39:31.3833441+01:00"" level=""ERROR"" thread=""7"" domain=""IsolatedAppDomainHost: IntegrationTests"" username=""AWESOMEMACHINE\Administrator""><log4net:message>msg</log4net:message><log4net:properties><log4net:data name=""log4net:HostName"" value=""AWESOMEMACHINE"" /></log4net:properties><log4net:exception>System.Exception: test</log4net:exception><log4net:locationInfo class=""IntegrationTests.LogTests"" method=""TestLog"" file=""C:\projects\LogViewer\IntegrationTests\LogTests.cs"" line=""19"" /></log4net:event>";

        [Fact]
        public void Can_detect_changes_to_file()
        {
            var file = Path.Combine(".", "testfile3.xml");
            if (File.Exists(file)) { File.Delete(file); }
            File.WriteAllText(file, _buffer);
            var outofbounds = new ConcurrentStack<OutOfBoundsEvent>();
			var exceptionWhileReading = new ConcurrentStack<Exception>();
			var files = new ConcurrentQueue<LogEntry>();
            using (var watcher = new Poller<LogEntry>(new FileWithPosition(file), 30, new LogEntryParser()).Tap(w=>
            {
                w.LogEntry += l => { files.Enqueue(l); };
                w.OutOfBounds += () => { outofbounds.Push(new OutOfBoundsEvent()); };
				w.ExceptionOccurred += e => { exceptionWhileReading.Push(e); };
            }))
            {
                watcher.Init();
                Assert.Equal(1, files.Count);
                File.AppendAllText(file, _buffer);
                Thread.Sleep(100/*750*3*/);
                Assert.Equal(2, files.Count);
            }
        }

        [Fact]
        public void Can_handle_rolling_log()
        {
            var file = Path.Combine(".", "testfile4.xml");
            if (File.Exists(file)) { File.Delete(file); }
            File.WriteAllText(file, _buffer);
			var outofbounds = new ConcurrentStack<OutOfBoundsEvent>();
			var exceptionWhileReading = new ConcurrentStack<Exception>();
			var files = new ConcurrentQueue<LogEntry>();
            using (var watcher = new Poller<LogEntry>(new FileWithPosition(file), 30, new LogEntryParser()).Tap(w=>
            {
                w.LogEntry += l => { files.Enqueue(l); };
                w.OutOfBounds += () => { outofbounds.Push(new OutOfBoundsEvent()); };
				w.ExceptionOccurred += e => { exceptionWhileReading.Push(e); };
			}))
            {
                watcher.Init();

                File.WriteAllText(file, "");
                Thread.Sleep(100/*750*3*/);
                Assert.True(outofbounds.ToArray().Length >= 1, "outofbounds>=1");
            }
        }
    }
}
