using System;
using System.Collections.Generic;
using System.Linq;
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
using MaterialDesignThemes.Wpf;

namespace ClickClerk.Barangay.Dialogs
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : UserControl
    {
        public InputDialog()
        {
            InitializeComponent();
        }

        public async Task<string> Show()
        {
            var res = await DialogHost.Show(this, "InternalDialog");
            if (!(res is bool b)) return null;
            return b ? Input.Text : null;
        }

        public static async Task<string> Show(string title, string defaultValue, PackIconKind icon = PackIconKind.Alert)
        {
            return await Show(title, "", defaultValue, null);
        }

        public static async Task<string> Show(string title, string message, string defaultValue, string positive, string negative = null, PackIconKind icon = PackIconKind.Keyboard)
        {
            var dlg = new InputDialog();
            dlg.Title.Text = title;

            dlg.Input.Text = defaultValue;

            if (string.IsNullOrEmpty(message))
                dlg.Message.Visibility = Visibility.Collapsed;
            else
                dlg.Message.Text = message;

            if (!string.IsNullOrEmpty(positive))
                dlg.Positive.Content = positive;

            dlg.Icon.Kind = icon;

            if (!string.IsNullOrEmpty(negative))
            {
                dlg.Negative.Content = negative;
            }

            return await dlg.Show();
        }

        private void Input_OnLoaded(object sender, RoutedEventArgs e)
        {
            Input.SelectAll();
            Input.Focus();
        }
    }
}
