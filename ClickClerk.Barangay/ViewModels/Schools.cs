using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;

namespace ClickClerk.Barangay.ViewModels
{
    class Schools:ViewModelBase
    {
        private ListCollectionView _items;

        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = new ListCollectionView(School.All);
                _items.LiveFilteringProperties.Add(nameof(School.Id));
                _items.Filter = o =>(o is School r && r.Id>0);
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

        private ICommand _addSchoolCommand;

        public ICommand AddSchoolCommand =>
            _addSchoolCommand ?? (_addSchoolCommand = new DelegateCommand(d =>
                {
                    if (string.IsNullOrEmpty(Fullname)) return;
                    var school = new School()
                    {
                        Fullname = Fullname
                    };
                    school.Save();
                    Fullname = "";
                },
                d=>!string.IsNullOrEmpty(Fullname)));
    }
}
