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
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : UserControl
    {
        public MessageDialog()
        {
            InitializeComponent();
        }

        public async Task<bool> Show()
        {
            var res = await DialogHost.Show(this, "InternalDialog");
            if (res == null) return false;
            return res is bool b && b;
        }

        public static async Task<bool> Show(string title, string positive, PackIconKind icon = PackIconKind.Alert)
        {
            return await Show(title, "", positive);
        }

        public static async Task<bool> Show(string title, string message, string positive, string negative=null, PackIconKind icon = PackIconKind.Alert)
        {
            var dlg = new MessageDialog();
            dlg.Title.Text = title;
            dlg.Message.Text = message;
            dlg.Positive.Content = positive;
            dlg.Icon.Kind = icon;
            if (!string.IsNullOrEmpty(negative))
            {
                dlg.Negative.Content = negative;
            }
            else
            {
                dlg.Negative.Visibility = Visibility.Collapsed;
            }

            return await dlg.Show();
        }
    }
}
