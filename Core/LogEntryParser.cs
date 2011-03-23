using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using LogViewer;

namespace Core
{
  public class LogEntryParser
  {
    public IEnumerable<LogEntry> Parse(string buffer)
    {
      var entries = new List<LogEntry>();
      DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
      int iIndex = 1;

      var doc = XDocument.Parse("<doc xmlns:log4j=\"http://jakarta.apache.org/log4j/\" xmlns:log4net=\"http://logging.apache.org/log4net/\">" + buffer + "</doc>");
      foreach (var xElement in doc.Root.Elements())
      {
        LogEntry logentry = new LogEntry();

        logentry.Item = iIndex;

        string timestamp = xElement.Attribute("timestamp").Value;
        double dSeconds;
        if (Double.TryParse(timestamp, out dSeconds))
        {
          logentry.TimeStamp = dt.AddMilliseconds(dSeconds).ToLocalTime();
        }
        else
        {
          logentry.TimeStamp = DateTime.Parse(timestamp).ToLocalTime();
        }
        logentry.Thread = xElement.Attribute("thread").Value;

        if (null != xElement.Attribute("domain"))
          logentry.App = xElement.Attribute("domain").Value;

        #region get level

        ////////////////////////////////////////////////////////////////////////////////
        logentry.Level = xElement.Attribute("level").Value;
        logentry.Image = ParseImageType(logentry.Level);
        ////////////////////////////////////////////////////////////////////////////////

        #endregion

        #region read xml

        var elements = xElement.Elements();
        foreach (var element in elements)
        {
          switch (element.Name.LocalName)
          {
            case ("message"):
              {
                logentry.Message = element.Value;
                break;
              }
            case "properties":
              {
                var properties = element.Elements();

                foreach (var property in properties)
                  switch (property.Attribute("name").Value)
                  {
                    case ("log4jmachinename"):
                      logentry.MachineName = property.Attribute("value").Value;
                      break;
                    case ("log4net:HostName"):
                      logentry.HostName = property.Attribute("value").Value;
                      break;
                    case ("log4net:UserName"):
                      logentry.UserName = property.Attribute("value").Value;
                      break;
                    case ("log4japp"):
                      logentry.App = property.Attribute("value").Value;
                      break;
                    default:
                      throw new NotImplementedException(property.Attribute("name").Value);
                  }

                break;
              }

            case "throwable":
            case "exception":
              {
                logentry.Throwable = element.Value;
                break;
              }
            case ("locationInfo"):
              {
                logentry.Class = element.Attribute("class").Value;
                logentry.Method = element.Attribute("method").Value;
                logentry.File = element.Attribute("file").Value;
                logentry.Line = element.Attribute("line").Value;
                break;
              }
            default:
              throw new NotImplementedException(element.Name.LocalName);
          }

        }

        #endregion

        entries.Add(logentry);
      }
      return entries;
    }

    public static LogEntry.ImageType ParseImageType(string level)
    {
      switch (level)
      {
        case "ERROR":
          return LogEntry.ImageType.Error;
        case "INFO":
          return LogEntry.ImageType.Info;
        case "DEBUG":
          return LogEntry.ImageType.Debug;
        case "WARN":
          return LogEntry.ImageType.Warn;
        case "FATAL":
          return LogEntry.ImageType.Fatal;
        default:
          return LogEntry.ImageType.Custom;
      }
    }
  }
}
