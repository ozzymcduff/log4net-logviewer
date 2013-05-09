using LogViewer;
using LogViewer.Infrastructure;
using LogViewer.Model;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using log4net.Core;

namespace IntegrationTests.LogViewerGui
{
    [TestFixture]
    public class FileLogEntryControllerTests : TestFixtureBase
    {
        public class RunSameThreadInvoker : IInvoker
        {
            public void Invoke(System.Action run)
            {
                run();
            }
        }
        public class Watcher : ILogFileWatcher
        {
            public Watcher()
            {
            }
            public void InvokeLogEntry(LogEntry entry) { LogEntry(entry); }
            public event System.Action<LogEntry> LogEntry;
            public void InvokeOutOfBounds() { OutOfBounds(); }
            public event System.Action OutOfBounds;

            public Action OnInit;
            public void Init()
            {
                OnInit();
            }
            public Action OnReset;
            public void Reset()
            {
                OnReset();
            }

            public void Dispose()
            {
            }
        }

        public class InMemoryPersist : IPersist
        {
            public InMemoryPersist()
            {
                _recent = new List<string>();
            }
            private readonly List<string> _recent;
            public List<string> RecentFiles()
            {
                return _recent;
            }

            public void InsertFile(string filepath)
            {
                _recent.Add(filepath);
            }

            public void RemoveFile(string filepath)
            {
                _recent.Remove(filepath);
            }
        }

        [Test]
        public void When_no_filename()
        {
            var c = new FileLogEntryController(new RunSameThreadInvoker(),
                (filename, parser) => new Watcher(),
                new InMemoryPersist());
            var entries = c.Entries.ToArray();
            Assert.That(entries.Count(), Is.EqualTo(0));
        }

        [Test]
        public void When_setting_filename_should_call_init_to_read()
        {
            var init = false;
            Watcher watcher = new Watcher()
                    {
                        OnInit = () => { init = true; },
                    };
            var c = new FileLogEntryController(new RunSameThreadInvoker(),
                (filename, parser) => watcher,
                new InMemoryPersist());
            c.FileName = "test";
            Assert.That(init);
            watcher.InvokeLogEntry(SampleLogEntry()); 
            Assert.That(c.Entries.Count(), Is.EqualTo(1));
        }

        [Test]
        public void When_getting_out_of_bounds_call_reset()
        {
            Watcher watcher = new Watcher()
            {
                OnInit = () => { },
            };
            var c = new FileLogEntryController(new RunSameThreadInvoker(),
                (filename, parser) => watcher,
                new InMemoryPersist());
            c.FileName = "test";
            watcher.InvokeLogEntry(SampleLogEntry());
            watcher.OnInit = Assert.Fail;
            var onreset = false;
            watcher.OnReset = () => { onreset = true; };
            watcher.InvokeOutOfBounds();
            Assert.That(onreset);
            watcher.InvokeLogEntry(SampleLogEntry());
            Assert.That(c.Entries.Count(), Is.EqualTo(1));
        }

        private static LogEntry SampleLogEntry()
        {
            return new LogEntry() { Data = new LoggingEventData() { Level = Level.Debug } };
        }
    }
}
