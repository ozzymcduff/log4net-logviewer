Log4net-logviewer
=================

Gui alternatives:
-----------------
- [Log2Console](http://log2console.codeplex.com/)
- Chainsaw, part of the log4j package

Note that you could use [LogTail.exe](./tree/master/LogTail) to reformat an xml and then use [LogParser](http://www.hanselman.com/blog/AnalyzeYourWebServerDataAndBeEmpoweredWithLogParserAndLogParserLizardGUI.aspx) on it.

I've spent a lot of time refactoring this project. Mostly the changes are to embrace the xaml way of life. I would guess that for most parts you would want to use Log2Console (a lot of positive comments on codeplex) or Chainsaw (I've used it for a while). The reason for using this project would mostly be for the command line tool.

Where does this come from?
--------------------------
Refactored version of [logviewer](http://www.codeproject.com/KB/cs/logviewer.aspx).

CLI usage
---------
I've also made a small console application [LogTail.exe](./tree/master/LogTail). Usage:

    -f|file={a filename}
The file to watch, monitor or 

    -l|lines={tail x lines}	
Display the last x lines. Defaults to 10 lines. 

    -y|layout={pattern layout syntax as defined in log4net.Layout.PatternLayout}
For example: 
    LogTail.exe logfile.xml -y="%date [%thread] %-5level %logger - %message%newline"

    -h|?|help
Display help


