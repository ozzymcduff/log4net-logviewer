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
            InitializeComponent();
            System.Windows.Controls.Label l = this.label;
        }

        public String Label
        {
            get { return (String)this.GetValue(LabelAndText.LabelProperty); }
            set { this.SetValue(LabelAndText.LabelProperty, value); }
        }
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label", typeof(String), typeof(LabelAndText), 
          new FrameworkPropertyMetadata((object)string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
              new PropertyChangedCallback(LabelAndText.OnLabelChanged)));

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelAndText ctrl = (LabelAndText)d;
            ctrl.label.Content = (string)e.NewValue;
        }

        public String Text 
        {
            get { return (String)this.GetValue(LabelAndText.TextProperty); }
            set { this.SetValue(LabelAndText.TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          "Text", typeof(String), typeof(LabelAndText),
          new FrameworkPropertyMetadata((object)string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
              new PropertyChangedCallback(LabelAndText.OnTextChanged)));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LabelAndText ctrl = (LabelAndText)d;
            ctrl.text.Text = (string)e.NewValue;
        }
    }
}
