using Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace WPFApp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly IQuitPlanService _quitPlanService = new QuitPlanService();
        private readonly ISmokingStatusService _smokingStatusService = new SmokingStatusService();

        private int _totalQuitDays;
        public int TotalQuitDays
        {
            get => _totalQuitDays;
            set
            {
                _totalQuitDays = value;
                OnPropertyChanged(nameof(TotalQuitDays));
            }
        }

        private decimal _moneySaved;
        public decimal MoneySaved
        {
            get => _moneySaved;
            set
            {
                _moneySaved = value;
                OnPropertyChanged(nameof(MoneySaved));
            }
        }

        public ICommand ViewAllNotificationsCommand { get; }

        public class CurrentPlanDashboardModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public double Progress { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
        private CurrentPlanDashboardModel _currentPlanDashboard;
        public CurrentPlanDashboardModel CurrentPlanDashboard
        {
            get => _currentPlanDashboard;
            set { _currentPlanDashboard = value; OnPropertyChanged(nameof(CurrentPlanDashboard)); }
        }
        public class JournalRecord
        {
            public DateTime Date { get; set; }
            public int Cigarettes { get; set; }
            public string HealthStatus { get; set; }

        }
        private ObservableCollection<JournalRecord> _recentJournals = new ObservableCollection<JournalRecord>();
        public ObservableCollection<JournalRecord> RecentJournals
        {
            get => _recentJournals;
            set { _recentJournals = value; OnPropertyChanged(nameof(RecentJournals)); }
        }    
        private void LoadCurrentPlanAndJournals()
        {
            var userId = AppSession.CurrentUser?.Id ?? 0;
            if (userId == 0) return;

            var plan = _quitPlanService.GetCurrentQuitPlanById(userId);
            if (plan != null)
            {
                var totalDays = (plan.TargetDate - plan.StartDate).Days;

                var allStatuses = _smokingStatusService.GetSmokingStatusesByUserId(userId);

                var successfulDays = allStatuses
                    .Where(s => s.RecordDate.Date >= plan.StartDate.Date &&
                                s.RecordDate.Date <= plan.TargetDate.Date &&
                                s.CigarettesPerDay <= plan.TargetCigarettesPerDay)
                    .Select(s => s.RecordDate.Date)
                    .Distinct()
                    .Count(); 

                double progress = totalDays > 0
                    ? Math.Min(100, Math.Max(0, (double)successfulDays / totalDays * 100))
                    : 0;
                CurrentPlanDashboard = new CurrentPlanDashboardModel
                {
                    Name = plan.Reason,
                    Description = $"Mục tiêu: {plan.MainGoal}, Số điếu mục tiêu mỗi ngày: {plan.TargetCigarettesPerDay}",
                    Progress = progress,
                    StartDate = plan.StartDate,
                    EndDate = plan.TargetDate
                };
            }
            else
            {
                CurrentPlanDashboard = new CurrentPlanDashboardModel
                {
                    Name = "Chưa có kế hoạch",
                    Description = "",
                    Progress = 0,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today
                };
            }
            var statuses = _smokingStatusService.GetSmokingStatusesByUserId(userId)
                .OrderByDescending(s => s.RecordDate)
                .Take(10)
                .ToList();
            RecentJournals = new ObservableCollection<JournalRecord>(
                statuses.Select(s => new JournalRecord
                {
                    Date = s.RecordDate,
                    Cigarettes = s.CigarettesPerDay,
                    HealthStatus = s.HealthStatus
                })
            );
        }

        public DashboardViewModel()
        {
            LoadCurrentPlanAndJournals();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
