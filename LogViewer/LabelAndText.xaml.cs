using System;
using System.Windows;
using System.Windows.Controls;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for LabelAndText.xaml
    /// </summary>
    public partial class LabelAndText
    {

        public LabelAndText()
        {
            InitializeComponent();
        }

        public String Label
        {
            get { return (String)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label", typeof(String), typeof(LabelAndText), 
          new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
              OnLabelChanged));

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (LabelAndText)d;
            ctrl.label.Content = e.NewValue;
        }

        public String Text 
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          "Text", typeof(String), typeof(LabelAndText),
          new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
              OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (LabelAndText)d;
            ctrl.text.Text = (string)e.NewValue;
        }
    }
}
