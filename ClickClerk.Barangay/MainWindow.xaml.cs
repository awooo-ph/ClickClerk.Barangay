using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using ClickClerk.Barangay.ViewModels;

namespace ClickClerk.Barangay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Core.Context = SynchronizationContext.Current;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if(_dialogHostLoaded) MainViewModel.Instance.Login();
        }

        private bool _dialogHostLoaded = false;
        private void ExternalDialogHostLoaded(object sender, RoutedEventArgs e)
        {
            _dialogHostLoaded = true;
        }
    }
}
