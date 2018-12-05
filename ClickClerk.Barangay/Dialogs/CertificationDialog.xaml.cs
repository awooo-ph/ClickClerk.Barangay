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
    /// Interaction logic for CertificationDialog.xaml
    /// </summary>
    public partial class CertificationDialog : UserControl
    {
        public CertificationDialog()
        {
            InitializeComponent();
            Data = new Certificate{CertificateType = CertificateTypes.GoodMoral};
            DataContext = this;
        }

        private Certificate _Data;

        public Certificate Data
        {
            get => _Data;
            set
            {
                if (value == _Data) return;
                _Data = value;
                OnPropertyChanged(nameof(Data));
            }
        }
        
        public static async Task<CertificationDialog> Show(Tawo tawo)
        {
            var dlg = new CertificationDialog();
            dlg.Data.Tawo = tawo;
            dlg.Data.ControlNumber = Certificate.GetNextNumber();
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
                 
                }));


        private bool _PrintCertificate;

        public bool PrintCertificate
        {
            get => _PrintCertificate;
            set
            {
                if (value == _PrintCertificate) return;
                _PrintCertificate = value;
                OnPropertyChanged(nameof(PrintCertificate));
            }
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
