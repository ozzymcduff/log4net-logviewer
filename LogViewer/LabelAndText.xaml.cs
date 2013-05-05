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

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for LabelAndText.xaml
    /// </summary>
    public partial class LabelAndText : UserControl
    {
        public LabelAndText()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        public String Label
        {
            get { return (String)this.GetValue(LabelProperty); }
            set { this.SetValue(LabelProperty, value); }
        }
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label", typeof(String), typeof(LabelAndText), new PropertyMetadata("Label"));

        public String Text 
        {
            get { return (String)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          "Text",typeof(String), typeof(LabelAndText), new PropertyMetadata("Text"));
    }
}
