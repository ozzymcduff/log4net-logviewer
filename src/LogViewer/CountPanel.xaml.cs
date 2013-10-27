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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Drawing;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for CountPanel.xaml
    /// </summary>
    public partial class CountPanel : UserControl
    {
        public CountPanel()
        {
            //labelDebugCount.Content = string.Format("{0:#,#}  ", DebugCount)
            InitializeComponent();
            imageError.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, null);
            imageInfo.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, null);
            imageWarn.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle, Int32Rect.Empty, null);
            imageDebug.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle, Int32Rect.Empty, null);
        }
        public Orientation Orientation 
        {
            get { return countpanel.Orientation; }
            set { countpanel.Orientation = value; }
        }
    }
}
