using BusinessObjects;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFApp.ViewModels
{
    public class SmokingChartViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SmokingStatus> _smokingData;
        public ObservableCollection<SmokingStatus> SmokingData
        {
            get => _smokingData;
            set
            {
                _smokingData = value;
                OnPropertyChanged();
            }
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public SmokingChartViewModel(IEnumerable<SmokingStatus> data)
        {
            SmokingData = new ObservableCollection<SmokingStatus>(data);
            SeriesCollection = new SeriesCollection();
            Formatter = value => value.ToString("N0");
        }
      
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
