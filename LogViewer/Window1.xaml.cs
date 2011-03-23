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
using System.Data;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Interop;
using System.Reflection;
using Core;


namespace LogViewer
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    private string _FileName = string.Empty;
    private string FileName
    {
      get { return _FileName; }
      set
      {
        _FileName = value;
        RecentFileList.InsertFile(value);
      }
    }

    private List<LogEntry> _Entries = new List<LogEntry>();
    public List<LogEntry> Entries
    {
      get { return _Entries; }
      set { _Entries = value; }
    }

    public Window1()
    {
      InitializeComponent();
      listView1.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ListView1_HeaderClicked));
      RecentFileList.UseXmlPersister();
      RecentFileList.MenuClick += (s, e) => OpenFile(e.Filepath);

      imageError.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, null);
      imageInfo.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, null);
      imageWarn.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle, Int32Rect.Empty, null);
      imageDebug.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle, Int32Rect.Empty, null);

      Title = string.Format("LogViewer  v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
    }

    private void Clear()
    {
      textBoxLevel.Text = string.Empty;
      textBoxTimeStamp.Text = string.Empty;
      textBoxMachineName.Text = string.Empty;
      textBoxThread.Text = string.Empty;
      textBoxItem.Text = string.Empty;
      textBoxHostName.Text = string.Empty;
      textBoxUserName.Text = string.Empty;
      textBoxApp.Text = string.Empty;
      textBoxClass.Text = string.Empty;
      textBoxMethod.Text = string.Empty;
      textBoxLine.Text = string.Empty;
      textBoxfile.Text = string.Empty;
      textBoxMessage.Text = string.Empty;
      textBoxThrowable.Text = string.Empty;
    }

    private void OpenFile(string fileName)
    {
      FileName = fileName;
      LoadFile();
    }

    private void LoadFile()
    {
      textboxFileName.Text = FileName;
      _Entries.Clear();
      listView1.ItemsSource = null;

      DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
      string sXml = string.Empty;
      string sBuffer = string.Empty;
      int iIndex = 1;

      Clear();

      try
      {
        FileStream oFileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
        StreamReader oStreamReader = new StreamReader(oFileStream);
        var txt = oStreamReader.ReadToEnd();
        var parser = new LogEntryParser();
        #region Read File Buffer

        Entries = parser.Parse(txt).ToList();
        {

          {

         
            #region Show Counts
            ////////////////////////////////////////////////////////////////////////////////
            int ErrorCount =
            (
                from entry in Entries
                where entry.Level == "ERROR"
                select entry
            ).Count();

            if (ErrorCount > 0)
            {
              labelErrorCount.Content = string.Format("{0:#,#}  ", ErrorCount);
              labelErrorCount.Visibility = Visibility.Visible;
              imageError.Visibility = Visibility.Visible;
            }
            else
            {
              labelErrorCount.Visibility = Visibility.Hidden;
              imageError.Visibility = Visibility.Hidden;
            }

            int InfoCount =
            (
                from entry in Entries
                where entry.Level == "INFO"
                select entry
            ).Count();

            if (InfoCount > 0)
            {
              labelInfoCount.Content = string.Format("{0:#,#}  ", InfoCount);
              labelInfoCount.Visibility = Visibility.Visible;
              imageInfo.Visibility = Visibility.Visible;
            }
            else
            {
              labelInfoCount.Visibility = Visibility.Hidden;
              imageInfo.Visibility = Visibility.Hidden;
            }

            int WarnCount =
            (
                from entry in Entries
                where entry.Level == "WARN"
                select entry
            ).Count();

            if (WarnCount > 0)
            {
              labelWarnCount.Content = string.Format("{0:#,#}  ", WarnCount);
              labelWarnCount.Visibility = Visibility.Visible;
              imageWarn.Visibility = Visibility.Visible;
            }
            else
            {
              labelWarnCount.Visibility = Visibility.Hidden;
              imageWarn.Visibility = Visibility.Hidden;
            }

            int DebugCount =
            (
                from entry in Entries
                where entry.Level == "DEBUG"
                select entry
            ).Count();

            if (DebugCount > 0)
            {
              labelDebugCount.Content = string.Format("{0:#,#}  ", DebugCount);
              labelDebugCount.Visibility = Visibility.Visible;
              imageDebug.Visibility = Visibility.Visible;
            }
            else
            {
              labelDebugCount.Visibility = Visibility.Hidden;
              labelDebugCount.Visibility = Visibility.Hidden;
            }
            ////////////////////////////////////////////////////////////////////////////////
            #endregion
          }
        }
        ////////////////////////////////////////////////////////////////////////////////
        #endregion
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }

      this.listView1.ItemsSource = _Entries;
    }

    #region ListView Events
    ////////////////////////////////////////////////////////////////////////////////
    private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        Clear();
        LogEntry logentry = this.listView1.SelectedItem as LogEntry;

        this.image1.Source = ImageRepositoy.GetImage( logentry.Image);
        this.textBoxLevel.Text = logentry.Level;
        this.textBoxTimeStamp.Text = string.Format("{0} {1}", logentry.TimeStamp.ToShortDateString(), logentry.TimeStamp.ToShortTimeString());
        this.textBoxMachineName.Text = logentry.MachineName;
        this.textBoxThread.Text = logentry.Thread;
        this.textBoxItem.Text = logentry.Item.ToString();
        this.textBoxHostName.Text = logentry.HostName;
        this.textBoxUserName.Text = logentry.UserName;
        this.textBoxApp.Text = logentry.App;
        this.textBoxClass.Text = logentry.Class;
        this.textBoxMethod.Text = logentry.Method;
        this.textBoxLine.Text = logentry.Line;
        this.textBoxMessage.Text = logentry.Message;
        this.textBoxThrowable.Text = logentry.Throwable;
        this.textBoxfile.Text = logentry.File;
      }
      catch { }
    }

    private ListSortDirection _Direction = ListSortDirection.Descending;

    private void ListView1_HeaderClicked(object sender, RoutedEventArgs e)
    {
      GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
      ListView source = e.Source as ListView;
      try
      {
        ICollectionView dataView = CollectionViewSource.GetDefaultView(source.ItemsSource);
        dataView.SortDescriptions.Clear();
        _Direction = _Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        SortDescription description = new SortDescription(header.Content.ToString(), _Direction);
        dataView.SortDescriptions.Add(description);
        dataView.Refresh();
      }
      catch (Exception)
      {
      }
    }
    ////////////////////////////////////////////////////////////////////////////////
    #endregion

    #region DragDrop
    ////////////////////////////////////////////////////////////////////////////////
    private delegate void VoidDelegate();
    private void listView1_Drop(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        try
        {
          Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
          if (a != null)
          {
            FileName = a.GetValue(0).ToString();
            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new VoidDelegate(delegate { LoadFile(); })
                );
          }
        }
        catch (Exception ex)
        {
          MessageBox.Show("Error in Drag Drop: " + ex.Message);
        }
      }
    }

    ////////////////////////////////////////////////////////////////////////////////
    #endregion

    #region Menu Events
    ////////////////////////////////////////////////////////////////////////////////
    private void Window_Closing(object sender, CancelEventArgs e)
    {

    }

    private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
    {
      System.Windows.Forms.OpenFileDialog oOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
      if (oOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        FileName = oOpenFileDialog.FileName;
        LoadFile();
      }
    }

    private void MenuRefresh_Click(object sender, RoutedEventArgs e)
    {
      LoadFile();
      listView1.SelectedIndex = listView1.Items.Count - 1;
      if (listView1.Items.Count > 4)
      {
        listView1.SelectedIndex -= 3;
      }
      listView1.ScrollIntoView(listView1.SelectedItem);
      ListViewItem lvi = listView1.ItemContainerGenerator.ContainerFromIndex(listView1.SelectedIndex) as ListViewItem;
      lvi.BringIntoView();
      lvi.Focus();
    }

    private void MenuFileExit_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void MenuAbout_Click(object sender, RoutedEventArgs e)
    {
      About about = new About();
      about.ShowDialog();
    }

    private void MenuFilter_Click(object sender, RoutedEventArgs e)
    {
      Filter filter = new Filter(Entries);
      filter.ShowDialog();
      if (filter.DialogResult == true)
      {
        string level = filter.Level;
        string message = filter.Message;
        string username = filter.UserName;

        List<LogEntry> query = new List<LogEntry>();

        if (level.Length > 0)
        {
          var q =
          (
              from entry in Entries
              where entry.Level == level
              select entry
          ).ToList<LogEntry>();

          query.AddRange(q);
        }

        if (message.Length > 0)
        {
          var q =
          (
              from entry in Entries
              where entry.Message.Contains(message)
              select entry
          ).ToList<LogEntry>();
          query.AddRange(q);
        }

        if (username.Length > 0)
        {
          var q =
          (
              from entry in Entries
              where entry.UserName.Contains(username)
              select entry
          ).ToList<LogEntry>();
          query.AddRange(q);
        }

        this.listView1.ItemsSource = query.Count > 0 ? query : Entries;
      }
    }

    private int CurrentIndex = 0;
    private void Find(int Direction)
    {
      if (textBoxFind.Text.Length > 0)
      {
        if (Direction == 0)
        {
          for (int i = CurrentIndex + 1; i < listView1.Items.Count; i++)
          {
            LogEntry item = (LogEntry)listView1.Items[i];
            if (item.Message.Contains(textBoxFind.Text))
            {
              listView1.SelectedIndex = i;
              listView1.ScrollIntoView(listView1.SelectedItem);
              ListViewItem lvi = listView1.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
              lvi.BringIntoView();
              lvi.Focus();
              CurrentIndex = i;
              break;
            }
          }
        }
        else
        {
          for (int i = CurrentIndex - 1; i > 0 && i < listView1.Items.Count; i--)
          {
            LogEntry item = (LogEntry)listView1.Items[i];
            if (item.Message.Contains(textBoxFind.Text))
            {
              listView1.SelectedIndex = i;
              listView1.ScrollIntoView(listView1.SelectedItem);
              ListViewItem lvi = listView1.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
              lvi.BringIntoView();
              lvi.Focus();
              CurrentIndex = i;
              break;
            }
          }
        }
      }
    }

    private void buttonFindNext_Click(object sender, RoutedEventArgs e)
    {
      Find(0);
    }

    private void buttonFindPrevious_Click(object sender, RoutedEventArgs e)
    {
      Find(1);
    }

    private void textBoxFind_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        if (textBoxFind.Text.Length > 0)
        {
          Find(0);
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////////
    #endregion

  }
}
