using System.Windows.Data;
using ClickClerk.Barangay.Models;

namespace ClickClerk.Barangay.ViewModels.Pages
{
    class People:PageBase
    {
        private People()
        {
            Header = "PEOPLE";
            IsSelected = true;
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
    }
}
