using BusinessObjects;
using Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using WPFApp.ViewModels;

namespace WPFApp.Views
{
    public class SmokingStatusViewModel : INotifyPropertyChanged
    {
        private readonly ISmokingStatusRepository _smokingStatusRepository;

        public ObservableCollection<SmokingStatus> HistoryRecords { get; set; }

        private int _cigarettesPerDay;
        private string _healthStatus;
        private DateTime _recordDate = DateTime.Today;

        public int CigarettesPerDay
        {
            get => _cigarettesPerDay;
            set
            {
                _cigarettesPerDay = value;
                OnPropertyChanged(nameof(CigarettesPerDay));
            }
        }

        public string HealthStatus
        {
            get => _healthStatus;
            set
            {
                _healthStatus = value;
                OnPropertyChanged(nameof(HealthStatus));
            }
        }

        public DateTime RecordDate
        {
            get => _recordDate;
            set
            {
                _recordDate = value;
                OnPropertyChanged(nameof(RecordDate));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand ExportCommand { get; }

        public SmokingStatusViewModel()
        {
            _smokingStatusRepository = new SmokingStatusRepository();
            HistoryRecords = new ObservableCollection<SmokingStatus>();

            SaveCommand = new RelayCommand(_ => SaveSmokingStatus(), _ => CanSaveSmokingStatus());
            ExportCommand = new RelayCommand(_ => ShowChartPopup());

            LoadSmokingHistory();
        }

        private void LoadSmokingHistory()
        {
            try
            {
                var history = _smokingStatusRepository.GetSmokingStatusesByUserId(AppSession.CurrentUser.Id);
                HistoryRecords.Clear();

                var sortedHistory = history.OrderByDescending(x => x.RecordDate);
                foreach (var record in sortedHistory)
                {
                    HistoryRecords.Add(record);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveSmokingStatus()
        {
            return CigarettesPerDay > 0 && !string.IsNullOrWhiteSpace(HealthStatus);
        }

        private void SaveSmokingStatus()
        {
            try
            {
                if (!CanSaveSmokingStatus())
                {
                    MessageBox.Show("Vui lòng kiểm tra lại thông tin nhập liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool result = _smokingStatusRepository.AddSmokingStatus(
                    AppSession.CurrentUser.Id,
                    CigarettesPerDay,
                    HealthStatus,
                    RecordDate
                );

                if (result)
                {
                    MessageBox.Show("Thêm thông tin hút thuốc thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    CigarettesPerDay = 0;
                    HealthStatus = string.Empty;
                    RecordDate = DateTime.Today;

                    LoadSmokingHistory();
                }
                else
                {
                    MessageBox.Show("Đã xảy ra lỗi khi thêm thông tin hút thuốc. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowChartPopup()
        {
            var chartWindow = new WPFApp.Views.SmokingChartView(HistoryRecords);
            chartWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
