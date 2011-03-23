using System.Linq;
using Core;
using NUnit.Framework;

namespace IntegrationTests
{
  [TestFixture]
  public class Log4NetTests
  {
    private string _buffer =
      @"<log4net:event logger=""IntegrationTests.LogTests"" timestamp=""2011-03-23T21:39:31.3833441+01:00"" level=""ERROR"" thread=""7"" domain=""IsolatedAppDomainHost: IntegrationTests"" username=""AWESOMEMACHINE\Administrator""><log4net:message>msg</log4net:message><log4net:properties><log4net:data name=""log4net:HostName"" value=""AWESOMEMACHINE"" /></log4net:properties><log4net:exception>System.Exception: test</log4net:exception><log4net:locationInfo class=""IntegrationTests.LogTests"" method=""TestLog"" file=""C:\projects\LogViewer\IntegrationTests\LogTests.cs"" line=""19"" /></log4net:event>";
    [Test]
    public void Parse()
    {
      var entry = new LogEntryParser().Parse(_buffer).Single();
      Assert.That(entry.Level, Is.EqualTo("ERROR"));
      Assert.That(entry.HostName, Is.EqualTo(@"AWESOMEMACHINE"));
      Assert.That(entry.App, Is.EqualTo(@"IsolatedAppDomainHost: IntegrationTests"));
      Assert.That(entry.Message, Is.EqualTo("msg"));
      Assert.That(entry.Class, Is.EqualTo("IntegrationTests.LogTests"));
      Assert.That(entry.Method, Is.EqualTo("TestLog"));
      Assert.That(entry.Line, Is.EqualTo("19"));
      Assert.That(entry.File, Is.EqualTo(@"C:\projects\LogViewer\IntegrationTests\LogTests.cs"));
    }
  }
}