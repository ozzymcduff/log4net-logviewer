using System.ComponentModel;

namespace LogViewer.Infrastructure
{
    public class Observable<T> : INotifyPropertyChanged
    {
        T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (!Equals(value, _value))
                {
                    _value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
