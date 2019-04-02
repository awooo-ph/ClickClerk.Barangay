using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using MaterialDesignThemes.Wpf;

namespace ClickClerk.Barangay.Dialogs
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginDialog : UserControl
    {
        private LoginDialog()
        {
            InitializeComponent();
        }

        public static async Task<NetworkCredential> Show()
        {
            var dlg = new LoginDialog();
            var res = await DialogHost.Show(dlg, "ExternalDialog");
            if (res == null) return null;

            return new NetworkCredential(dlg.Username.Text, dlg.Password.SecurePassword);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
