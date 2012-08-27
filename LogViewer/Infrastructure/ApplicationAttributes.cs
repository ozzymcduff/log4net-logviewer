using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LogViewer
{
    public class ApplicationAttributes
    {
        public  string Title { get; private set; }
        public  string CompanyName { get; private set; }
        public  string Copyright { get; private set; }
        public  string ProductName { get; private set; }

        public  string Version { get; private set; }
        private static string GetAttributeValue<T>(IEnumerable<object> attributes, Func<T,string> transform) 
        {
            var attr= attributes.FirstOrDefault(a => a.GetType() == typeof(T));
            if (null != attr) 
            {
                return transform((T)attr);
            }
            return string.Empty;
        }
        public static ApplicationAttributes Get()
        {
            var _Assembly = Assembly.GetEntryAssembly();
            object[] attributes = _Assembly.GetCustomAttributes(false);
            return new ApplicationAttributes()
            {
                Title= GetAttributeValue<AssemblyTitleAttribute>(attributes,a=>a.Title),
                CompanyName= GetAttributeValue<AssemblyCompanyAttribute>(attributes,a=>a.Company),
                Copyright= GetAttributeValue<AssemblyCopyrightAttribute>(attributes,a=>a.Copyright),
                ProductName= GetAttributeValue<AssemblyProductAttribute>(attributes,a=>a.Product),
                Version=_Assembly.GetName().Version.ToString()
            };
        }
    }
}
