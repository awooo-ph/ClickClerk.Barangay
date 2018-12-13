using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using ClickClerk.Barangay.Dialogs;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;
using MaterialDesignThemes.Wpf;

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
                _items.LiveFilteringProperties.Add(nameof(Tawo.Fullname));
                _items.IsLiveFiltering = true;
                _items.Filter = FilterTawo;
                return _items;
            }
        }

        private bool FilterTawo(object obj)
        {
            if (!(obj is Tawo t)) return false;
            if (string.IsNullOrEmpty(SearchKeyword)) return true;
            if (t.Fullname.ToLower().Contains(SearchKeyword.ToLower())) return true;

            return false;
        }

        protected override void OnSearch()
        {
            //foreach (Tawo item in Items)
            //{
                //item.Refresh(nameof(item.Fullname));
            //}
            Items.Refresh();
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

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand<Tawo>(async d =>
        {
            if (await MessageDialog.Show("Are you sure you want to delete this person?",
                $"{d.Fullname} will be removed from the database. Click DELETE to confirm action.",
                "DELETE", "CANCEL", PackIconKind.DeleteForever))
            {
               d.Delete();
            }
        }));
    }
}
