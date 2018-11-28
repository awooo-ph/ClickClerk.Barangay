using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;

namespace ClickClerk.Barangay.ViewModels
{
    class Religions:ViewModelBase
    {
        private ListCollectionView _items;

        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = new ListCollectionView(Religion.All);
                _items.Filter = o =>(o is Religion r && r.Id>0);
                return _items;
            }
        }

        private string _Fullname;

        public string Fullname
        {
            get => _Fullname;
            set
            {
                if (value == _Fullname) return;
                _Fullname = value;
                OnPropertyChanged(nameof(Fullname));
            }
        }

        private ICommand _addReligionCommand;

        public ICommand AddReligionCommand =>
            _addReligionCommand ?? (_addReligionCommand = new DelegateCommand(d =>
                {
                    if (string.IsNullOrEmpty(Fullname)) return;
                    var religion = new Religion()
                    {
                        Fullname = Fullname
                    };
                    religion.Save();
                    Fullname = "";
                },
                d=>!string.IsNullOrEmpty(Fullname)));
    }
}
