using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickClerk.Barangay.Tools;

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
                if (value)
                {
                    Messenger.Default.Broadcast(Messages.PageChanged,this);
                }
            }
        }

        protected virtual void OnSearch() { }

        private string _SearchKeyword;

        public string SearchKeyword
        {
            get => _SearchKeyword;
            set
            {
                if (value == _SearchKeyword) return;
                _SearchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
                OnSearch();
            }
        }

        private bool _CanSearch;

        public bool CanSearch
        {
            get => _CanSearch;
            set
            {
                if (value == _CanSearch) return;
                _CanSearch = value;
                OnPropertyChanged(nameof(CanSearch));
            }
        }


    }
}
