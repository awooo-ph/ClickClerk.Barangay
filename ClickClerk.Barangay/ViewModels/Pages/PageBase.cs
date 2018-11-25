using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickClerk.Barangay.ViewModels.Pages
{
    abstract class PageBase:ViewModelBase
    {
        private string _Header;

        public string Header
        {
            get => _Header;
            set
            {
                if (value == _Header) return;
                _Header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        private bool _IsSelected;

        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (value == _IsSelected) return;
                _IsSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

    }
}
