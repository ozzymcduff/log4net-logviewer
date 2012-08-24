using System;
using System.Linq;
using Core;
using log4net;
using NUnit.Framework;

namespace IntegrationTests
{
  [TestFixture]
  public class Log4JTests
  {
    private static readonly string _buffer = @"<log4j:event logger=""IntegrationTests.LogTests"" timestamp=""1300902418948"" level=""ERROR"" thread=""7""><log4j:message>test</log4j:message><log4j:properties><log4j:data name=""log4net:UserName"" value=""AWESOMEMACHINE\Administrator"" /><log4j:data name=""log4jmachinename"" value=""AWESOMEMACHINE"" /><log4j:data name=""log4japp"" value=""IsolatedAppDomainHost: IntegrationTests"" /><log4j:data name=""log4net:HostName"" value=""AWESOMEMACHINE"" /></log4j:properties><log4j:locationInfo class=""IntegrationTests.LogTests"" method=""TestLog"" file=""C:\projects\LogViewer\IntegrationTests\LogTests.cs"" line=""18"" /></log4j:event>";

    [Ignore("Used to generate log file"),Test]
    public void TestLog()
    {
      var log = LogManager.GetLogger(typeof(Log4JTests));
      log.Error("test");
      log.Error("msg", new Exception("test"));
    }

    [Test]
    public void Parse()
    {
      var entry = new LogEntryParser().Parse(_buffer).Single();
      Assert.That(entry.Level, Is.EqualTo("ERROR"));
      Assert.That(entry.UserName, Is.EqualTo(@"AWESOMEMACHINE\Administrator"));
      Assert.That(entry.MachineName, Is.EqualTo(@"AWESOMEMACHINE"));
      Assert.That(entry.HostName, Is.EqualTo(@"AWESOMEMACHINE"));
      Assert.That(entry.App, Is.EqualTo(@"IsolatedAppDomainHost: IntegrationTests"));
      Assert.That(entry.Message, Is.EqualTo("test"));
      Assert.That(entry.Class, Is.EqualTo("IntegrationTests.LogTests"));
      Assert.That(entry.Method, Is.EqualTo("TestLog"));
      Assert.That(entry.Line, Is.EqualTo("18"));
      Assert.That(entry.File, Is.EqualTo(@"C:\projects\LogViewer\IntegrationTests\LogTests.cs"));
    }
    [Test]
    public void Parse2()
    {
      var line =
        @"<log4j:event logger=""IntegrationTests.LogTests"" timestamp=""1300909721869"" level=""ERROR"" thread=""7""><log4j:message>msg</log4j:message><log4j:properties><log4j:data name=""log4net:UserName"" value=""AWESOMEMACHINE\Administrator"" /><log4j:data name=""log4jmachinename"" value=""AWESOMEMACHINE"" /><log4j:data name=""log4japp"" value=""IsolatedAppDomainHost: IntegrationTests"" /><log4j:data name=""log4net:HostName"" value=""AWESOMEMACHINE"" /></log4j:properties><log4j:throwable>System.Exception: test</log4j:throwable><log4j:locationInfo class=""IntegrationTests.LogTests"" method=""TestLog"" file=""C:\projects\LogViewer\IntegrationTests\LogTests.cs"" line=""27"" /></log4j:event>";
      var entry = new LogEntryParser().Parse(line).Single();

      Assert.That(entry.Throwable, Is.EqualTo("System.Exception: test"));
    }
  }
}
