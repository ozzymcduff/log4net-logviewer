using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LogViewer
{
    public class RecentFile
    {
        public int Number { get; private set; }
        public string Filepath { get; private set; }
       
        public string DisplayPath
        {
            get
            {
                return Path.Combine(
                    Path.GetDirectoryName(Filepath),
                    Path.GetFileNameWithoutExtension(Filepath));
            }
        }

        public RecentFile(int number, string filepath)
        {
            this.Number = number;
            this.Filepath = filepath;
        }
        public string DisplayName
        {
            get
            {
                /// <summary>
                /// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
                /// Default = "_{0}:  {2}"
                /// </summary>
                const string MenuItemFormatOneToNine = "_{0}:  {1}";

                /// <summary>
                /// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
                /// Default = "{0}:  {2}"
                /// </summary>
                const string MenuItemFormatTenPlus = "{0}:  {1}";
                const int MaxPathLength = 50;

                string format = (Number < 10 ? MenuItemFormatOneToNine : MenuItemFormatTenPlus);

                string shortPath = FileUtil.ShortenPathname(DisplayPath, MaxPathLength);

                return String.Format(format, Number, shortPath);
            }
        }
    }
}
