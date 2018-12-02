using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;
using JetBrains.Annotations;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;

namespace ClickClerk.Barangay.Dialogs
{
    /// <summary>
    /// Interaction logic for CaseDialog.xaml
    /// </summary>
    public partial class CaseDialog : INotifyPropertyChanged
    {
        public CaseDialog()
        {
            InitializeComponent();
            Data = new CaseFile(){DateFiled = DateTime.Now};
            DataContext = this;
        }

        private CaseFile _Data;

        public CaseFile Data
        {
            get => _Data;
            set
            {
                if (value == _Data) return;
                _Data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        private ListCollectionView _complainants;

        public ListCollectionView Complainants
        {
            get
            {
                if (_complainants != null) return _complainants;
                _complainants = new ListCollectionView(Tawo.Cache);
                _complainants.Filter = o =>
                {
                    if (!(o is Tawo t)) return false;
                    return !Data.Complainants.Contains(t) && !Data.Respondents.Contains(t);
                };
                return _complainants;
            }
        }

        private ListCollectionView _Respondents;

        public ListCollectionView Respondents
        {
            get
            {
                if (_Respondents != null) return _Respondents;
                _Respondents = new ListCollectionView(Tawo.Cache);
                _Respondents.Filter = o =>
                {
                    if (!(o is Tawo t)) return false;
                    return !Data.Respondents.Contains(t) && !Data.Complainants.Contains(t);
                };
                return _Respondents;
            }
        }

        public static async Task<CaseDialog> Show()
        {
            var dlg = new CaseDialog();
            var res = await DialogHost.Show(dlg, "ExternalDialog");
            if (res == null) return null;
            if(res is bool b && b)
                return dlg;
            return null;
        }

        private ICommand _addComplainantCommand;

        public ICommand AddComplainantCommand =>
            _addComplainantCommand ?? (_addComplainantCommand = new DelegateCommand(
                d =>
                {
                    if (Complainants.CurrentItem == null) return;
                    Data.Complainants.Add(Complainants.CurrentItem as Tawo);
                    Complainants.Refresh();
                    Respondents.Refresh();
                },d=> Complainants.CurrentItem!=null));

        private ICommand _addRespondentCommand;

        public ICommand AddRespondentCommand =>
            _addRespondentCommand ?? (_addRespondentCommand = new DelegateCommand(
                d =>
                {
                    if (Respondents.CurrentItem == null) return;
                    Data.Respondents.Add(Respondents.CurrentItem as Tawo);
                    Complainants.Refresh();
                    Respondents.Refresh();
                },d=> Respondents.CurrentItem!=null));

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Stepper_OnContinueNavigation(object sender, StepperNavigationEventArgs args)
        {
            if (args.NextStep == null)
            {
                DialogHost.CloseDialogCommand.Execute(true,this);
            }
        }

        private void Stepper_OnActiveStepChanged(object sender, ActiveStepChangedEventArgs args)
        {
            OnPropertyChanged(nameof(Data));
        }
    }
}
