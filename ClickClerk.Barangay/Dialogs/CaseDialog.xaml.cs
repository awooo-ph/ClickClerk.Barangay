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
            Data = new CaseFile();
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
                    return !Data.Complainants.Contains(t);
                };
                return _complainants;
            }
        }

        public static async Task<CaseDialog> Show()
        {
            var dlg = new CaseDialog();
            var res = await DialogHost.Show(dlg, "ExternalDialog");
            if (res == null) return null;
            return dlg;
        }

        private ICommand _addComplainantCommand;

        public ICommand AddComplainantCommand =>
            _addComplainantCommand ?? (_addComplainantCommand = new DelegateCommand(
                d =>
                {
                    if (Complainants.CurrentItem == null) return;
                    Data.Complainants.Add(Complainants.CurrentItem as Tawo);
                    Complainants.Refresh();
                },d=> Complainants.CurrentItem!=null));

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
