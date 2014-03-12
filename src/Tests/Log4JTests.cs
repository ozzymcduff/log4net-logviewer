using System;
using System.Linq;
using LogViewer;
using log4net;
using NUnit.Framework;
using System.IO;
using LogViewer.Infrastructure;

namespace IntegrationTests
{
    [TestFixture]
    public class Log4JTests
    {
        private static readonly string _buffer = @"<log4j:event logger=""IntegrationTests.LogTests"" timestamp=""1300902418948"" level=""ERROR"" thread=""7""><log4j:message>test</log4j:message><log4j:properties><log4j:data name=""log4net:UserName"" value=""AWESOMEMACHINE\Administrator"" /><log4j:data name=""log4jmachinename"" value=""AWESOMEMACHINE"" /><log4j:data name=""log4japp"" value=""IsolatedAppDomainHost: IntegrationTests"" /><log4j:data name=""log4net:HostName"" value=""AWESOMEMACHINE"" /></log4j:properties><log4j:locationInfo class=""IntegrationTests.LogTests"" method=""TestLog"" file=""C:\projects\LogViewer\IntegrationTests\LogTests.cs"" line=""18"" /></log4j:event>";

        [Ignore("Used to generate log file"), Test]
        public void TestLog()
        {
            var log = LogManager.GetLogger(typeof(Log4JTests));
            log.Error("test");
            log.Error("msg", new Exception("test"));
        }

        [Test]
        public void Parse2()
        {
            using (var s = new MemoryStream())
            using (var w = new StreamWriter(s))
            {

                var line =
                  @"<log4j:event logger=""IntegrationTests.LogTests"" timestamp=""1300909721869"" level=""ERROR"" thread=""7""><log4j:message>msg</log4j:message><log4j:properties><log4j:data name=""log4net:UserName"" value=""AWESOMEMACHINE\Administrator"" /><log4j:data name=""log4jmachinename"" value=""AWESOMEMACHINE"" /><log4j:data name=""log4japp"" value=""IsolatedAppDomainHost: IntegrationTests"" /><log4j:data name=""log4net:HostName"" value=""AWESOMEMACHINE"" /></log4j:properties><log4j:throwable>System.Exception: test</log4j:throwable><log4j:locationInfo class=""IntegrationTests.LogTests"" method=""TestLog"" file=""C:\projects\LogViewer\IntegrationTests\LogTests.cs"" line=""27"" /></log4j:event>";

                w.Write(line);
                w.Flush();
                s.Position = 0;
                var entry = new LogEntryParser().Parse(s).Single();

                Assert.That(entry.Data.ExceptionString, Is.EqualTo("System.Exception: test"));
            }
        }

        [Test]
        public void Parse3()
        {
            using (var s = new MemoryStream())
            using (var w = new StreamWriter(s))
            {

                var line =
                  @"<log4j:event 
logger=""IntegrationTests.LogTests"" 
timestamp=""1300909721869"" 
level=""ERROR"" thread=""7"">
<log4j:message>msg</log4j:message>
<log4j:properties>
	<log4j:data name=""log4net:UserName"" value=""AWESOMEMACHINE\Administrator"" />
	<log4j:data name=""log4jmachinename"" value=""AWESOMEMACHINE"" />
	<log4j:data name=""log4japp"" value=""IsolatedAppDomainHost: IntegrationTests"" />
	<log4j:data name=""log4net:HostName"" value=""AWESOMEMACHINE"" />
</log4j:properties>
<log4j:throwable>System.Exception: test</log4j:throwable>
<log4j:locationInfo 
	class=""IntegrationTests.LogTests"" method=""TestLog"" 
	file=""C:\projects\LogViewer\IntegrationTests\LogTests.cs"" 
	line=""27"" /></log4j:event>";

                w.Write(line);
                w.Flush();
                s.Position = 0;

                var entry = new LogEntryParser().Parse(s).Single();
                Assert.That(entry.Data.Level.Name, Is.EqualTo("ERROR"));
                Assert.That(entry.Data.UserName, Is.EqualTo(@"AWESOMEMACHINE\Administrator"));
                Assert.That(entry.MachineName, Is.EqualTo(@"AWESOMEMACHINE"));
                Assert.That(entry.HostName, Is.EqualTo(@"AWESOMEMACHINE"));
                Assert.That(entry.Data.Domain, Is.EqualTo(@"IsolatedAppDomainHost: IntegrationTests"));
                Assert.That(entry.Data.Message, Is.EqualTo("msg"));
                Assert.That(entry.Class(), Is.EqualTo("IntegrationTests.LogTests"));
                Assert.That(entry.Data.ExceptionString, Is.EqualTo("System.Exception: test"));
                Assert.That(entry.Method(), Is.EqualTo("TestLog"));
                Assert.That(entry.Line(), Is.EqualTo("27"));
                Assert.That(entry.File(), Is.EqualTo(@"C:\projects\LogViewer\IntegrationTests\LogTests.cs"));
            }
        }

        [Test]
        public void Parse_with_empty_ndc_should_not_crash_parser()
        {
            //<log4j:NDC></log4j:NDC>

            using (var s = new MemoryStream())
            using (var w = new StreamWriter(s))
            {

                var line =
                    @"<log4j:event 
logger=""IntegrationTests.LogTests"" 
timestamp=""1300909721869"" 
level=""ERROR"" thread=""7"">
<log4j:NDC></log4j:NDC>
<log4j:message>msg</log4j:message>
<log4j:properties>
	<log4j:data name=""log4net:UserName"" value=""AWESOMEMACHINE\Administrator"" />
	<log4j:data name=""log4jmachinename"" value=""AWESOMEMACHINE"" />
	<log4j:data name=""log4japp"" value=""IsolatedAppDomainHost: IntegrationTests"" />
	<log4j:data name=""log4net:HostName"" value=""AWESOMEMACHINE"" />
</log4j:properties>
<log4j:throwable>System.Exception: test</log4j:throwable>
<log4j:locationInfo 
	class=""IntegrationTests.LogTests"" method=""TestLog"" 
	file=""C:\projects\LogViewer\IntegrationTests\LogTests.cs"" 
	line=""27"" /></log4j:event>";

                w.Write(line);
                w.Flush();
                s.Position = 0;

                var entry = new LogEntryParser().Parse(s).Single();
                Assert.That(entry.Data.Level.Name, Is.EqualTo("ERROR"));
            }
        }

        [Test]
        public void Parse_with_text_element()
        {
            var line = @"<log4j:event logger=""installrelease"" timestamp=""1388750967872"" level=""ERROR"" thread=""""><log4j:NDC></log4j:NDC><log4j:message>Caught Errno::EACCES: Permission denied - filesomething
  C:/Ruby193/lib/ruby/gems/1.9.1/gems/someliblib/lib/libloblob/libloblob.rb:42:in `block in download'
  C:/Ruby193/lib/ruby/gems/1.9.1/gems/someliblib/lib/libloblob/libloblob.rb:41:in `each'
  C:/Ruby193/lib/ruby/gems/1.9.1/gems/someliblib/lib/libloblob/libloblob.rb:41:in `download'
  install_release.rb:92:in `block in get_latest!'</log4j:message><log4j:throwable>Caught Errno::EACCES: Permission denied - file
  C:/Ruby193/lib/ruby/gems/1.9.1/gems/someliblib/lib/libloblob/libloblob.rb:42:in `block in download'
  C:/Ruby193/lib/ruby/gems/1.9.1/gems/someliblib/lib/libloblob/libloblob.rb:41:in `each'
  C:/Ruby193/lib/ruby/gems/1.9.1/gems/someliblib/lib/libloblob/libloblob.rb:41:in `download'
  install_release.rb:92:in `block in get_latest!'</log4j:throwable><log4j:locationInfo class="""" method="""" file="""" line=""""/><log4j:properties></log4j:properties></log4j:event>";
            using (var s = new MemoryStream())
            using (var w = new StreamWriter(s))
            {
                w.Write(line);
                w.Flush();
                s.Position = 0;

                var entry = new LogEntryParser().Parse(s).Single();
                Assert.That(entry.Data.Level.Name, Is.EqualTo("ERROR"));
                Assert.That(entry.Data.Message, Is.StringContaining(@"Caught Errno::EACCES: Permission denied - filesomething"));
                Assert.That(entry.Data.Message, Is.StringContaining(@"install_release.rb:92:in `block in get_latest!'"));
            }
        }

        [Test]
        public void Parse()
        {
            using (var s = new MemoryStream())
            using (var w = new StreamWriter(s))
            {
                var lines = @"<log4j:event logger=""HomeController"" timestamp=""1361528145331"" level=""ERROR"" thread=""91"">
<log4j:message>error</log4j:message>
<log4j:properties>
    <log4j:data name=""log4net:UserName"" value=""SOMEDOMAIN\someuser"" />
    <log4j:data name=""log4net:Identity"" value=""44045"" />
    <log4j:data name=""log4jmachinename"" value=""somemachine"" />
    <log4j:data name=""log4japp"" value=""/LM/W3SVC/1"" />
    <log4j:data name=""log4net:HostName"" value=""SOMEHOST"" />
</log4j:properties>
<log4j:locationInfo class=""MyController"" method=""Log"" file=""C:\project\MyController.cs"" line=""99"" />
</log4j:event>
";

                w.Write(lines);
                w.Flush();
                s.Position = 0;

                var entry = new LogEntryParser().Parse(s).Single();

                Assert.That(entry.Identity, Is.EqualTo("44045"));
            }
        }

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
                Assert.That(entry.Data.UserName, Is.EqualTo(@"AWESOMEMACHINE\Administrator"));
                Assert.That(entry.MachineName, Is.EqualTo(@"AWESOMEMACHINE"));
                Assert.That(entry.HostName, Is.EqualTo(@"AWESOMEMACHINE"));
                Assert.That(entry.Data.Domain, Is.EqualTo(@"IsolatedAppDomainHost: IntegrationTests"));
                Assert.That(entry.Data.Message, Is.EqualTo("test"));
                Assert.That(entry.Class(), Is.EqualTo("IntegrationTests.LogTests"));
                Assert.That(entry.Method(), Is.EqualTo("TestLog"));
                Assert.That(entry.Line(), Is.EqualTo("18"));
                Assert.That(entry.File(), Is.EqualTo(@"C:\projects\LogViewer\IntegrationTests\LogTests.cs"));
            }
        }

        [Test]
        public void ParseFaulty()
        {
            using (var s = FileUtil.OpenReadOnly("test.xml"))
            {
                var entry = new LogEntryParser().Parse(s).Single();
                Assert.That(entry.Data.Message, Is.StringContaining("Translation for 'Detta ska jag g"));
                Assert.That(entry.Data.Message, Is.StringContaining("ra...' in module Todo for culture sv-SE does not exist."));

            }
        }
    }
}
