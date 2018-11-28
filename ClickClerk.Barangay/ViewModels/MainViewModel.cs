using System.Collections.Generic;
using System.Windows.Input;
using ClickClerk.Barangay.Tools;
using ClickClerk.Barangay.ViewModels.Pages;

namespace ClickClerk.Barangay.ViewModels
{
    class MainViewModel:ViewModelBase
    {
        private MainViewModel()
        {
            Messenger.Default.AddListener<PageBase>(Messages.PageChanged, page => { CurrentScreen = page; });
        }

        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ?? (_instance = new MainViewModel());

        private PageBase _CurrentScreen;

        public PageBase CurrentScreen
        {
            get => _CurrentScreen;
            set
            {
                if (value == _CurrentScreen) return;
                _CurrentScreen = value;
                OnPropertyChanged(nameof(CurrentScreen));
            }
        }

        private ViewModelBase _RightDrawer;

        public ViewModelBase RightDrawer
        {
            get => _RightDrawer;
            set
            {
                if (value == _RightDrawer) return;
                _RightDrawer = value;
                OnPropertyChanged(nameof(RightDrawer));
                IsDrawerOpen = value!=null;
            }
        }

        private bool _IsDrawerOpen;

        public bool IsDrawerOpen
        {
            get => _IsDrawerOpen;
            set
            {
                if (value == _IsDrawerOpen) return;
                _IsDrawerOpen = value;
                OnPropertyChanged(nameof(IsDrawerOpen));
            }
        }

        private ICommand _showReligionsCommand;

        public ICommand ShowReligionsCommand => _showReligionsCommand ?? (_showReligionsCommand = new DelegateCommand(
                                                    d => { RightDrawer=new Religions(); }));

        private ICommand _showSchoolsCommand;

        public ICommand ShowSchoolsCommand =>
            _showSchoolsCommand ?? (_showSchoolsCommand = new DelegateCommand(d =>
            {
                RightDrawer = new Schools();
            }));

    }
}
