using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using log4net.Core;
using log4net.Util;
using log4net;
using System.Reflection;

namespace LogViewer
{
    public class LogEntryParser
    {
        private static readonly DateTime _dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        class Names
        {
            public Object @event;
            public Object message;
            public Object properties;
            public Object data;
            public Object ndc;
            public Object name;
            public Object @value;
            public Object throwable;
            public Object exception;
            public Object timestamp;
            public Object thread;
            public Object domain;
            public Object level;
            public Object logger;
            public Object locationinfo;
            public Object @class;
            public Object method;
            public Object file;
            public Object line;
            public Object username;
        }

        public IEnumerable<LogEntry> Parse(Stream s)
        {
            var nt = new NameTable();
            var nsmgr = new XmlNamespaceManager(nt);
            nsmgr.AddNamespace("log4j", "http://jakarta.apache.org/log4j/");
            nsmgr.AddNamespace("log4net", "http://logging.apache.org/log4net/");
            // Create the XmlParserContext.
            var context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

            // Create the reader. 
            var settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                CheckCharacters = false,
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Ignore
            };
            var list = new List<LogEntry>();
            var encoding = Encoding.UTF8;
            var xmlreader = XmlReader.Create(new StreamReader(s, encoding), settings, context);

            Names names = new Names
            {
                @event = xmlreader.NameTable.Add("event"),
                message = xmlreader.NameTable.Add("message"),
                properties = xmlreader.NameTable.Add("properties"),
                data = xmlreader.NameTable.Add("data"),
                ndc = xmlreader.NameTable.Add("NDC"),
                name = xmlreader.NameTable.Add("name"),
                @value = xmlreader.NameTable.Add("value"),
                throwable = xmlreader.NameTable.Add("throwable"),
                exception = xmlreader.NameTable.Add("exception"),
                timestamp = xmlreader.NameTable.Add("timestamp"),
                thread = xmlreader.NameTable.Add("thread"),
                domain = xmlreader.NameTable.Add("domain"),
                level = xmlreader.NameTable.Add("level"),
                logger = xmlreader.NameTable.Add("logger"),
                locationinfo = xmlreader.NameTable.Add("locationInfo"),
                @class = xmlreader.NameTable.Add("class"),
                method = xmlreader.NameTable.Add("method"),
                file = xmlreader.NameTable.Add("file"),
                line = xmlreader.NameTable.Add("line"),
                username = xmlreader.NameTable.Add("username")
            };

            while (xmlreader.Read())
            {
                switch (xmlreader.NodeType)
                {
                    case XmlNodeType.Whitespace:
                        break;
                    case XmlNodeType.Element:
                        if (Object.ReferenceEquals(xmlreader.LocalName, names.@event))
                        {
                            LogEntry logentry = null;
                            logentry = new LogEntry();
                            if (xmlreader.HasAttributes)
                            {
                                while (xmlreader.MoveToNextAttribute())
                                {
                                    if (Object.ReferenceEquals(xmlreader.LocalName, names.timestamp))
                                    {
                                        var timestamp = xmlreader.Value;
                                        if (!string.IsNullOrEmpty(timestamp))
                                        {
                                            double dSeconds;
                                            logentry.Data.TimeStamp = Double.TryParse(timestamp, out dSeconds)
                                                                          ? _dt.AddMilliseconds(dSeconds).ToLocalTime()
                                                                          : DateTime.Parse(timestamp).ToLocalTime();
                                        }
                                    }
                                    else if (Object.ReferenceEquals(xmlreader.LocalName, names.thread))
                                    {
                                        logentry.Data.ThreadName = xmlreader.Value;
                                    }
                                    else if (Object.ReferenceEquals(xmlreader.LocalName, names.domain))
                                    {
                                        logentry.Data.Domain = xmlreader.Value;
                                    }
                                    else if (Object.ReferenceEquals(xmlreader.LocalName, names.level))
                                    {
                                        logentry.Data.Level = GetLevel(xmlreader.Value);
                                    }
                                    else if (Object.ReferenceEquals(xmlreader.LocalName, names.logger))
                                    {
                                        logentry.Data.LoggerName = xmlreader.Value;
                                    }
                                    else if (Object.ReferenceEquals(xmlreader.LocalName, names.username))
                                    {
                                        logentry.Data.UserName = xmlreader.Value;
                                    }
                                    else
                                    {
                                        throw new NotImplementedException(xmlreader.LocalName);
                                    }
                                }
                                xmlreader.MoveToElement();
                            }

                            EventChildren(xmlreader, names, logentry);
                            list.Add(logentry);
                        }
                        else
                        {
                            Console.WriteLine(xmlreader.NodeType + ", " + xmlreader.Name + ": " + xmlreader.Value);
                            throw new NotImplementedException();
                        }
                        break;
                    default:
                        Console.WriteLine(xmlreader.NodeType + ", " + xmlreader.Name + ": " + xmlreader.Value);
                        throw new NotImplementedException(xmlreader.NodeType.ToString());
                }
            }
            return list;
        }

        private Level GetLevel(string level)
        {
            return _log.Logger.Repository.LevelMap[level];
        }

        private void EventChildren(XmlReader xmlreader, Names names, LogEntry logentry)
        {
            while (xmlreader.Read())
            {
                switch (xmlreader.NodeType)
                {
                    case XmlNodeType.Whitespace:
                        break;
                    case XmlNodeType.Element:
                        if (Object.ReferenceEquals(xmlreader.LocalName, names.message))
                        {
                            logentry.Data.Message = xmlreader.ReadInnerXml();
                        }
                        else if (Object.ReferenceEquals(xmlreader.LocalName, names.throwable)
                                || Object.ReferenceEquals(xmlreader.LocalName, names.exception))
                        {
                            logentry.Data.ExceptionString = xmlreader.ReadInnerXml();
                        }
                        else if (Object.ReferenceEquals(xmlreader.LocalName, names.properties))
                        {
                            ReadProperties(xmlreader, names, logentry);
                        }
                        else if (Object.ReferenceEquals(xmlreader.LocalName, names.locationinfo))
                        {
                            ReadLocationInfo(xmlreader, names, logentry);
                        }
                        else if (Object.ReferenceEquals(xmlreader.LocalName, names.data))
                        {
                            ReadDataAttributes(xmlreader, names, logentry);
                        }
                        else if (Object.ReferenceEquals(xmlreader.LocalName, names.ndc))
                        {
                            ReadNDC(xmlreader, names, logentry);
                        }
                        else
                        {
                            throw new NotImplementedException("1! " + xmlreader.Name);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (Object.ReferenceEquals(xmlreader.LocalName, names.@event))
                        {
                            return;
                        }
                        else if (Object.ReferenceEquals(xmlreader.LocalName, names.message)
                              || Object.ReferenceEquals(xmlreader.LocalName, names.throwable)
                              || Object.ReferenceEquals(xmlreader.LocalName, names.exception)
                              || Object.ReferenceEquals(xmlreader.LocalName, names.properties)
                          )
                        {
                            //Ignore it
                        }
                        else
                        {
                            throw new NotImplementedException("3! " + xmlreader.Name);
                        }
                        break;
                    default:
                        throw new NotImplementedException("2! " + xmlreader.NodeType);
                    //break;
                }
            }
        }

        private void ReadLocationInfo(XmlReader xmlreader, Names names, LogEntry logentry)
        {
            var className = string.Empty;
            var methodName = string.Empty;
            var fileName = string.Empty;
            var lineNumber = string.Empty;
            if (xmlreader.HasAttributes)
            {
                while (xmlreader.MoveToNextAttribute())
                {
                    if (Object.ReferenceEquals(xmlreader.LocalName, names.@class))
                    {
                        className = xmlreader.Value;
                    }
                    else if (Object.ReferenceEquals(xmlreader.LocalName, names.method))
                    {
                        methodName = xmlreader.Value;
                    }
                    else if (Object.ReferenceEquals(xmlreader.LocalName, names.file))
                    {
                        fileName = xmlreader.Value;
                    }
                    else if (Object.ReferenceEquals(xmlreader.LocalName, names.line))
                    {
                        lineNumber = xmlreader.Value;
                    }
                    else
                    {
                        throw new NotImplementedException(xmlreader.LocalName);
                    }
                }
                logentry.Data.LocationInfo = new LocationInfo(className, methodName, fileName, lineNumber);
                xmlreader.MoveToElement();
            }
        }

        private void ReadProperties(XmlReader xmlreader, Names names, LogEntry logentry)
        {
            while (xmlreader.Read())
            {
                switch (xmlreader.NodeType)
                {
                    case XmlNodeType.Whitespace:
                        break;
                    case XmlNodeType.Element:
                        if (Object.ReferenceEquals(xmlreader.LocalName, names.data))
                        {
                            ReadDataAttributes(xmlreader, names, logentry);
                        }
                        else
                        {
                            throw new NotImplementedException("!");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (Object.ReferenceEquals(xmlreader.LocalName, names.data))
                        {

                        }
                        else if (Object.ReferenceEquals(xmlreader.LocalName, names.properties))
                        {
                            return;
                        }
                        else
                        {
                            throw new NotImplementedException("! " + xmlreader.Name);
                        }
                        break;
                    default:
                        throw new NotImplementedException("! " + xmlreader.NodeType);
                }
            }
        }

        private void ReadDataAttributes(XmlReader xmlreader, Names names, LogEntry logentry)
        {
            var name = string.Empty;
            var val = string.Empty;
            if (xmlreader.HasAttributes)
            {
                while (xmlreader.MoveToNextAttribute())
                {
                    if (Object.ReferenceEquals(xmlreader.LocalName, names.name))
                    {
                        name = xmlreader.Value;
                    }
                    else if (Object.ReferenceEquals(xmlreader.LocalName, names.value))
                    {
                        val = xmlreader.Value;
                    }
                    else
                    {
                        throw new NotImplementedException(xmlreader.LocalName);
                    }
                }

                // move back to the element node that contains
                // the attributes we just traversed
                xmlreader.MoveToElement();
                if (logentry.Data.Properties == null)
                {
                    logentry.Data.Properties = new PropertiesDictionary();
                }
                logentry.Data.Properties[name] = val;
                switch (name)
                {
                    case ("log4net:UserName"):
                        logentry.Data.UserName = val;
                        break;
                    case ("log4japp"):
                        logentry.Data.Domain = val;
                        break;
                }
            }

        }

        private void ReadNDC(XmlReader xmlreader, Names names, LogEntry logentry)
        {
            if (xmlreader.Read())
            {
                switch (xmlreader.NodeType)
                {
                    case XmlNodeType.Whitespace:
                        break;
                    case XmlNodeType.Element:
                        if (Object.ReferenceEquals(xmlreader.LocalName, names.ndc))
                        {
                        }
                        else
                        {
                            throw new NotImplementedException("!");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (Object.ReferenceEquals(xmlreader.LocalName, names.ndc))
                        {
                            return;
                        }
                        throw new NotImplementedException("! " + xmlreader.Name);
                    default:
                        throw new NotImplementedException("! " + xmlreader.NodeType);
                }
            }
        }
    }
}
