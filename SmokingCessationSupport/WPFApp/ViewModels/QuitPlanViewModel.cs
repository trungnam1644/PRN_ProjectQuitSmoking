using BusinessObjects;
using Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WPFApp.ViewModels;

namespace WPFApp.Views
{
    public class QuitPlanViewModel : INotifyPropertyChanged
    {
        private readonly IQuitPlanService iQuitPlanService;
        private readonly ISmokingStatusService _smokingStatusService = new SmokingStatusService();
        private readonly IQuitPlanService _quitPlanService = new QuitPlanService();

        private string _newPlanReason;
        private DateTime _newPlanStartDate = DateTime.Today;
        private DateTime _newPlanTargetDate = DateTime.Today.AddMonths(1);
        private QuitPlan _currentPlan;
        private double _planProgress;
        private bool _hasCurrentPlan;
        private string _mainGoal;
        private int _targetCigarettesPerDay;

        public ObservableCollection<QuitPlanStage> StageTemplates { get; set; }
        public ObservableCollection<QuitPlanStage> SelectedStages { get; set; }
        public ObservableCollection<string> CurrentStages { get; set; } = new ObservableCollection<string>();

        public string MainGoal
        {
            get => _mainGoal;
            set
            {
                _mainGoal = value;
                OnPropertyChanged(nameof(MainGoal));
            }
        }

        public int TargetCigarettesPerDay
        {
            get => _targetCigarettesPerDay;
            set
            {
                _targetCigarettesPerDay = value;
                OnPropertyChanged(nameof(TargetCigarettesPerDay));
            }
        }

        public string NewPlanReason
        {
            get => _newPlanReason;
            set
            {
                _newPlanReason = value;
                OnPropertyChanged(nameof(NewPlanReason));
            }
        }

        public DateTime NewPlanStartDate
        {
            get => _newPlanStartDate;
            set
            {
                _newPlanStartDate = value;
                OnPropertyChanged(nameof(NewPlanStartDate));
            }
        }

        public DateTime NewPlanTargetDate
        {
            get => _newPlanTargetDate;
            set
            {
                _newPlanTargetDate = value;
                OnPropertyChanged(nameof(NewPlanTargetDate));
            }
        }

        public QuitPlan CurrentPlan
        {
            get => _currentPlan;
            set
            {
                _currentPlan = value;
                OnPropertyChanged(nameof(CurrentPlan));
                HasCurrentPlan = _currentPlan != null;
            }
        }

        public double PlanProgress
        {
            get => Math.Round(_planProgress, 2);
            set
            {
                _planProgress = value;
                OnPropertyChanged(nameof(PlanProgress));
            }
        }

        public bool HasCurrentPlan
        {
            get => _hasCurrentPlan;
            set
            {
                if (_hasCurrentPlan != value)
                {
                    _hasCurrentPlan = value;
                    OnPropertyChanged(nameof(HasCurrentPlan));
                }
            }
        }

        public ICommand CreatePlanCommand { get; }

        public QuitPlanViewModel()
        {
            StageTemplates = new ObservableCollection<QuitPlanStage>();
            SelectedStages = new ObservableCollection<QuitPlanStage>();
            iQuitPlanService = new QuitPlanService();

            CreatePlanCommand = new RelayCommand(obj => CreateQuitPlan());

            LoadCurrentPlan();
        }
                     
        private void LoadCurrentPlan()
        {
            var totalDays = 0;
            var completedDays = 0;
            CurrentPlan = iQuitPlanService.GetCurrentQuitPlanById(AppSession.CurrentUser.Id);
            HasCurrentPlan = CurrentPlan != null;
            PlanProgress = CalculatePlanProgress();
        }

        public void CreateQuitPlan()
        {
            if (string.IsNullOrEmpty(NewPlanReason))
            {
                MessageBox.Show("Vui lòng nhập lý do cai thuốc!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(MainGoal))
            {
                MessageBox.Show("Vui lòng nhập mục tiêu chính!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewPlanStartDate >= NewPlanTargetDate)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TargetCigarettesPerDay < 0)
            {
                MessageBox.Show("Số điếu hút mục tiêu phải lớn hơn hoặc bằng 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentPlan != null)
            {
                var totalDays = (CurrentPlan.TargetDate - CurrentPlan.StartDate).Days;
                var completedDays = (DateTime.Today - CurrentPlan.StartDate).Days;
                PlanProgress = CalculatePlanProgress();
                if (PlanProgress != 100)
                {
                    MessageBox.Show("Bạn đã có một kế hoạch cai thuốc hiện tại. Vui lòng hoàn thành kế hoạch này trước khi tạo kế hoạch mới.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var newPlan = new QuitPlan
            {
                UserId = AppSession.CurrentUser.Id,
                Reason = NewPlanReason,
                StartDate = NewPlanStartDate,
                TargetDate = NewPlanTargetDate,
                MainGoal = MainGoal,
                TargetCigarettesPerDay = TargetCigarettesPerDay
            };

            if (iQuitPlanService.AddQuitPlan(newPlan))
            {
                MessageBox.Show("Kế hoạch cai thuốc đã được tạo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                CurrentPlan = newPlan;
                PlanProgress = 0;
                NewPlanReason = "";
                MainGoal = "";
                TargetCigarettesPerDay = 0;
                NewPlanStartDate = DateTime.Today;
                NewPlanTargetDate = DateTime.Today.AddMonths(1);
            }
            else
            {
                MessageBox.Show("Không thể tạo kế hoạch cai thuốc. Vui lòng thử lại sau.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private double CalculatePlanProgress()
        {
            var progress = 0.0;
            var userId = AppSession.CurrentUser?.Id ?? 0;
            var plan = _quitPlanService.GetCurrentQuitPlanById(userId);
            if (plan != null)
            {
                var totalDayss = (plan.TargetDate - plan.StartDate).Days;

                var allStatuses = _smokingStatusService.GetSmokingStatusesByUserId(userId);

                var successfulDays = allStatuses
                    .Where(s => s.RecordDate.Date >= plan.StartDate.Date &&
                                s.RecordDate.Date <= plan.TargetDate.Date &&
                                s.CigarettesPerDay <= plan.TargetCigarettesPerDay)
                    .Select(s => s.RecordDate.Date)
                    .Distinct()
                    .Count();

               progress = totalDayss > 0
                    ? Math.Min(100, Math.Max(0, (double)successfulDays / totalDayss * 100))
                    : 0;              
            }
            return progress;
        }

    }
}