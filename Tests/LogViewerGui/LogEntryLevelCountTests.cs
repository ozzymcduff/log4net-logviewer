using LogViewer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationTests
{
    [TestFixture]
    public class LogEntryLevelCountTests
    {
        [Test,Ignore]
        public void Test() 
        {
            var count1 = new LogEntryLevelCount(new []{ new KeyValuePair<string,int>("WARN",1), new KeyValuePair<string,int>("ERROR",2)});
            var count2 = new LogEntryLevelCount(new[] { new KeyValuePair<string, int>("WARN", 3) });
            Assert.That(count1 + count2, 
                Is.EqualTo(new LogEntryLevelCount(new []{ new KeyValuePair<string,int>("WARN",4), new KeyValuePair<string,int>("ERROR",2)})));
        }
    }
}
