using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClickClerk.Barangay.Dialogs;
using ClickClerk.Barangay.Tools;

namespace ClickClerk.Barangay.ViewModels.Pages
{
    class Cases : PageBase
    {
        private Cases()
        {
            CanSearch = true;
        }

        private static Cases _instance;
        public static Cases Instance => _instance ?? (_instance = new Cases());

        private ObservableCollection<Models.CaseFile> _items;
        public ObservableCollection<Models.CaseFile> Items => _items ?? (_items = Models.CaseFile.GetAll());

        private ICommand _addCommand;
        public ICommand AddCommand => _addCommand ?? (_addCommand = new DelegateCommand(async d =>
        {
            var res = await CaseDialog.Show();
            return;
            if (res == null) return;
            if (res.Data.Save() > 0)
            {
                
            }
        }));
    }
}
