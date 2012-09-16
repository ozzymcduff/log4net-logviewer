Refactored http://www.codeproject.com/KB/cs/logviewer.aspx

You will need a Log4Net.config with Layout XmlLayoutSchemaLog4j:

<?xml version="1.0" encoding="utf-8" ?>

<log4net>
  <appender name="RollingFileAppender" type="log4net.Appender.RollingFileAppender">
    <file value="log.xml" />
    <appendToFile value="true" />
    <datePattern value="yyyyMMdd" />
    <rollingStyle value="Date" />
    <layout type="log4net.Layout.XmlLayoutSchemaLog4j">
      <locationInfo value="true" />
    </layout>
  </appender>
  <root>
    <level value="ALL" />
    <appender-ref ref="RollingFileAppender" />
  </root>
</log4net>

The format of the log files are something like:
<log4j:event logger="IntegrationTests.LogTests" timestamp="1300909721869" level="ERROR" thread="7">
  <log4j:message>msg</log4j:message>
  <log4j:properties>
    <log4j:data name="log4net:UserName" value="AWESOMEMACHINE\Administrator" />
    <log4j:data name="log4jmachinename" value="AWESOMEMACHINE" />
    <log4j:data name="log4japp" value="IsolatedAppDomainHost: IntegrationTests" />
    <log4j:data name="log4net:HostName" value="AWESOMEMACHINE" />
  </log4j:properties>
  <log4j:throwable>System.Exception: test</log4j:throwable>
  <log4j:locationInfo class="IntegrationTests.LogTests" method="TestLog" file="C:\projects\LogViewer\IntegrationTests\LogTests.cs" line="27" />
</log4j:event>

I've also made a small console application LogTail.exe. Usage:
-f|file={a filename}
  The file to watch, monitor or 

-l|lines={tail x lines}	
	Display the last x lines. Defaults to 10 lines. 

-y|layout={pattern layout syntax as defined in log4net.Layout.PatternLayout}
  For example: LogTail.exe logfile.xml -y="%date [%thread] %-5level %logger - %message%newline"

-h|?|help
	Display help

For instance to :
LogTail.exe logfile.xml
LogTail.exe -file=logfile.xml


