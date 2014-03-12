using IntegrationTests.LogViewerGui;
using LogViewer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationTests
{
    [TestFixture]
    public class LogEntryLevelCountTests : TestFixtureBase
    {
        [Test]
        public void Can_equals()
        {
            Assert.That(LCount(Kv("ERROR", 1), Kv("WARN", 1)),
                Is.EqualTo(LCount(Kv("WARN", 1), Kv("ERROR", 1))));
            Assert.That(LCount(Kv("WARN", 1), Kv("ERROR", 1)),
                Is.EqualTo(LCount(Kv("WARN", 1), Kv("ERROR", 1))));
            Assert.That(LCount(Kv("WARN", 2), Kv("ERROR", 1)),
                Is.EqualTo(LCount(Kv("WARN", 2), Kv("ERROR", 1))));
            Assert.That(LCount(Kv("WARN", 2), Kv("ERROR", 1)),
                Is.Not.EqualTo(LCount(Kv("ERROR", 1))));
            Assert.That(LCount(Kv("ERROR", 1)),
                Is.Not.EqualTo(LCount(Kv("WARN", 2), Kv("ERROR", 1))));
        }

        [Test]
        public void Can_add()
        {
            var count1 = LCount(Kv("WARN", 1), Kv("ERROR", 2));
            var count2 = LCount(Kv("WARN", 3));
            Assert.That(count1 + count2,
                Is.EqualTo(LCount(Kv("WARN", 4), Kv("ERROR", 2))));
        }

        [Test]
        public void Can_remove()
        {
            var count1 = LCount(Kv("WARN", 1), Kv("ERROR", 2));
            var count2 = LCount(Kv("ERROR", 1));
            Assert.That(count1 - count2,
                Is.EqualTo(LCount(Kv("WARN", 1), Kv("ERROR", 1))));
        }
    }
}
