using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Input;
using ClickClerk.Barangay.Dialogs;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;
using ClickClerk.Barangay.ViewModels.Pages;
using MaterialDesignThemes.Wpf;

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

        private ICommand _showSettingsCommand;

        public ICommand ShowSettingsCommand =>
            _showSettingsCommand ?? (_showSettingsCommand = new DelegateCommand(d =>
                {
                    RightDrawer = Settings.Instance;
                },d=>CurrentUser?.IsAdmin??false));

        private bool _isAuthenticating = false;
        public async void Login()
        {
            if (_isAuthenticating) return;
            _isAuthenticating = true;
            while (!IsAuthenticated)
            {
                var credential = await LoginDialog.Show();
                var user = User.All.FirstOrDefault(x => x.Username.ToLower() == credential.UserName);

                if (user==null && User.GetRows() == 0)
                {
                    user = new User()
                    {
                        Fullname = "Administrator",
                        IsAdmin = true,
                        Username = credential.UserName,
                        Password = credential.Password
                    };
                }

                
                if (user == null || string.IsNullOrEmpty(credential.Password))
                {
                    await MessageDialog.Show("Authentication Failed","INVALID USERNAME AND PASSWORD!", "OKAY",null, PackIconKind.Key);
                    continue;
                }

                if(string.IsNullOrEmpty(user.Password)) user.Update(nameof(User.Password),credential.Password);

                if (user.Password != credential.Password)
                {
                    await MessageDialog.Show("Authentication Failed", "INVALID USERNAME AND PASSWORD!", "OKAY", null, PackIconKind.Key);
                    continue;
                }

                user.Save();

                CurrentUser = user;
                break;
            }

            _isAuthenticating = false;
        }



        private User _CurrentUser;

        public User CurrentUser
        {
            get => _CurrentUser;
            set
            {
                _CurrentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }

        public bool IsAuthenticated => CurrentUser != null;

        private ICommand _logoutCommand;
        public ICommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new DelegateCommand(d =>
         {
             CurrentUser = null;
             Login();
         }));

    }
}
