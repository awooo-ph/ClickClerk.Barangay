using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using ClickClerk.Barangay.Models;
using JetBrains.Annotations;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;

namespace ClickClerk.Barangay.Dialogs
{
    /// <summary>
    /// Interaction logic for HouseholdDialog.xaml
    /// </summary>
    public partial class HouseholdDialog : INotifyPropertyChanged
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
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
