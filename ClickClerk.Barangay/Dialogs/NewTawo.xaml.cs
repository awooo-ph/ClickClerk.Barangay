using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;
using JetBrains.Annotations;
using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;

namespace ClickClerk.Barangay.Dialogs
{
    /// <summary>
    /// Interaction logic for NewTawo.xaml
    /// </summary>
    public partial class NewTawo : UserControl,INotifyPropertyChanged
    {
        private NewTawo()
        {
            InitializeComponent();
            Data = new Tawo();
            Education = new ObservableCollection<Education>();
            DataContext = this;
        }

        private Tawo _Data;

        public Tawo Data
        {
            get => _Data;
            set
            {
                if (value == _Data) return;
                _Data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        private bool _IsNameValid;

        public bool IsNameValid
        {
            get => _IsNameValid;
            set
            {
                if (value == _IsNameValid) return;
                _IsNameValid = value;
                OnPropertyChanged(nameof(IsNameValid));
            }
        }

        private ObservableCollection<Education> _Education;

        public ObservableCollection<Education> Education
        {
            get => _Education;
            set
            {
                if (value == _Education) return;
                _Education = value;
                OnPropertyChanged(nameof(Education));
            }
        }
        
        public static async Task<NewTawo> Show()
        {
            var dlg = new NewTawo();
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
