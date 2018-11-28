using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using ClickClerk.Barangay.Dialogs;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;

namespace ClickClerk.Barangay.ViewModels.Pages
{
    class People:PageBase
    {
        private People()
        {
            Header = "PEOPLE";
            IsSelected = true;
            CanSearch = true;
        }

        private static People _instnace;
        public static People Instance => _instnace ?? (_instnace = new People());

        private ListCollectionView _items;

        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = new ListCollectionView(Tawo.Cache);
                return _items;
            }
        }

        private ICommand _editEducationCommand;

        public ICommand EditEducationCommand =>
            _editEducationCommand ?? (_editEducationCommand = new DelegateCommand<Tawo>(async d =>
            {
                var educ = new ObservableCollection<Education>(d.Education);
                var res = await EducationDialog.Show(educ);
                if (!res) return;
                foreach (var education in educ)
                {
                    education.TawoId = d.Id;
                    education.Save();
                    if(!d.Education.Contains(education))
                        d.Education.Add(education);
                }
            }));

        private ICommand _addCommand;
        public ICommand AddCommand => _addCommand ?? (_addCommand = new DelegateCommand(async d =>
        {
            var res = await NewTawo.Show();
            if (res == null) return;
            if (res.Data.Save() > 0)
            {
                foreach (var education in res.Education)
                {
                    education.TawoId = res.Data.Id;
                    education.Save();
                    res.Data.Education.Add(education);
                }
            }
        }));
    }
}
