using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StringBuilder sbText = new StringBuilder();
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                object[] attributes = assembly.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    Type type = attribute.GetType();
                    if (type == typeof(AssemblyTitleAttribute))
                    {
                        AssemblyTitleAttribute title = (AssemblyTitleAttribute)attribute;
                        labelAssemblyName.Content = title.Title;
                    }
                    if (type == typeof(AssemblyFileVersionAttribute))
                    {
                        AssemblyFileVersionAttribute version = (AssemblyFileVersionAttribute)attribute;
                        labelAssemblyVersion.Content = version.Version;
                    }
                    if (type == typeof(AssemblyCopyrightAttribute))
                    {
                        AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)attribute;
                        sbText.AppendFormat("{0}\r", copyright.Copyright);
                    }
                    if (type == typeof(AssemblyCompanyAttribute))
                    {
                        AssemblyCompanyAttribute company = (AssemblyCompanyAttribute)attribute;
                        sbText.AppendFormat("{0}\r",company.Company);
                    }
                    if (type == typeof(AssemblyDescriptionAttribute))
                    {
                        AssemblyDescriptionAttribute description = (AssemblyDescriptionAttribute)attribute;
                        sbText.AppendFormat("{0}\r", description.Description);
                    }
                }
                labelAssembly.Content = sbText.ToString();
            }
            string text =
@"<log4net>
  <appender name=""RollingFileAppender"" type=""log4net.Appender.RollingFileAppender"">
    <file type=""log4net.Util.PatternString"" value=""c:\log\log.xml"" />
    <appendToFile value=""true"" />
    <datePattern value=""yyyyMMdd"" />
    <rollingStyle value=""Date"" />
    <layout type=""log4net.Layout.XmlLayoutSchemaLog4j"">
      <locationInfo value=""true"" />
    </layout>
  </appender>
  <root>
    <level value=""DEBUG"" />
    <appender-ref ref=""RollingFileAppender"" />
  </root>
</log4net>";
           
            this.RichTextBox1.AppendText(text);
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }        
    }
}
