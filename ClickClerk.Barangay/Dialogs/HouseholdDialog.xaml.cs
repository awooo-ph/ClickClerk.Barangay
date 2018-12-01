using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using JetBrains.Annotations;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;

namespace ClickClerk.Barangay.Dialogs
{
    /// <summary>
    /// Interaction logic for HouseholdDialog.xaml
    /// </summary>
    public partial class HouseholdDialog : UserControl,INotifyPropertyChanged
    {
        private HouseholdDialog()
        {
            InitializeComponent();
            Data = new Household();
            DataContext = this;
        }

        private Household _Data;

        public Household Data
        {
            get => _Data;
            set
            {
                if (value == _Data) return;
                _Data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        private ListCollectionView _people;

        public ListCollectionView People
        {
            get
            {
                if (_people != null) return _people;
                _people = new ListCollectionView(Tawo.Cache);
                _people.Filter = o =>
                {
                    if (!(o is Tawo t)) return false;
                    return !t.HasHousehold;
                };
                return _people;
            }
        }
        
        public static async Task<HouseholdDialog> Show()
        {
            var dlg = new HouseholdDialog();
            var res = await DialogHost.Show(dlg, "ExternalDialog");
            if (res == null) return null;
            return dlg;
        }

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
