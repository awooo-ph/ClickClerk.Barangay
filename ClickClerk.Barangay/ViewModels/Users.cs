using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClickClerk.Barangay.Dialogs;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;
using MaterialDesignThemes.Wpf;

namespace ClickClerk.Barangay.ViewModels
{
    class Users:ViewModelBase
    {
        private Users() { }
        private static Users _instance;
        public static Users Instance => _instance ?? (_instance = new Users());


        private User _NewUser = new User();

        public User NewUser
        {
            get => _NewUser;
            set
            {
                if (value == _NewUser) return;
                _NewUser = value;
                OnPropertyChanged(nameof(NewUser));
            }
        }

        private ICommand _addCommand;
        public ICommand AddCommand => _addCommand ?? (_addCommand = new DelegateCommand(d =>
        {
            NewUser.Save();
            User.All.Add(NewUser);
            NewUser = new User();
        },d=>NewUser.IsValid));

        private ICommand _deleteCommand;

        public ICommand DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand<User>(async d =>
            {
                if (!await MessageDialog.Show("Confirm Delete", null, "DELETE USER", "CANCEL", PackIconKind.Delete)) return;
                d.Delete();
                User.All.Remove(d);
            }, d => d != null));
    }
}
