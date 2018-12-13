using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;
using JetBrains.Annotations;
using MaterialDesignThemes.Wpf;

namespace ClickClerk.Barangay.Dialogs
{
    /// <summary>
    /// Interaction logic for TawoFinder.xaml
    /// </summary>
    public partial class TawoFinder : INotifyPropertyChanged
    {
        private TawoFinder()
        {
            InitializeComponent();
            NewTawo = new Tawo();
            Education = new ObservableCollection<Education>();
            DataContext = this;

            Messenger.Default.AddListener<Tawo>(Messages.TawoNameChanged, t =>
            {
                OnPropertyChanged(nameof(HasSelected));
            });
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Tawo _NewTawo;

        public Tawo NewTawo
        {
            get => _NewTawo;
            set
            {
                _NewTawo = value;
                OnPropertyChanged(nameof(NewTawo));
            }
        }


        private string _SearchKeyword;

        public string SearchKeyword
        {
            get => _SearchKeyword;
            set
            {
                if (value == _SearchKeyword) return;
                _SearchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
                People.Refresh();
            }
        }

        private ListCollectionView _people;

        public ListCollectionView People
        {
            get
            {
                if (_people != null) return _people;
                _people = new ListCollectionView(Tawo.Cache);
                _people.Filter = FilterPeople;
                _people.CurrentChanged += (sender, args) =>
                {
                    OnPropertyChanged(nameof(HasSelected));
                };
                _people.SortDescriptions.Add(new SortDescription(nameof(Tawo.Fullname),ListSortDirection.Ascending));
                return _people;
            }
        }

        public List<string> Names => Tawo.Cache.Select(x => x.Fullname).ToList();

        private bool FilterPeople(object obj)
        {
            if (!(obj is Tawo tawo)) return false;
            if (Filter!=null && !Filter(obj)) return false;
            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                return tawo.Fullname.ToUpper().Contains(SearchKeyword.ToUpper());
            }
            return true;
        }

        private string _Position = "PARENT";

        public string Position
        {
            get => _Position;
            set
            {
                if (value == _Position) return;
                _Position = value;
                OnPropertyChanged(nameof(Position));
            }
        }
        
        private bool _UseSearch = true;

        public bool UseSearch
        {
            get => _UseSearch;
            set
            {
                if (value == _UseSearch) return;
                _UseSearch = value;
                OnPropertyChanged(nameof(UseSearch));
                OnPropertyChanged(nameof(HasSelected));
            }
        }

        public bool HasSelected
        {
            get
            {
                if (UseSearch) return People.CurrentItem != null;
                return NewTawo.IsNameValid;
            }
        }

        private Visibility _relationVisibility = Visibility.Visible;

        public Visibility RelationVisibility
        {
            get => _relationVisibility;
            set
            {
                if (value == _relationVisibility) return;
                _relationVisibility = value;
                OnPropertyChanged(nameof(RelationVisibility));
            }
        }



        private ObservableCollection<Education> _Education;

        public ObservableCollection<Education> Education
        {
            get => _Education;
            set
            {
                if (value == _Education) return;
                _Education = value;
                OnPropertyChanged(nameof(Education));
            }
        }
        
        public Predicate<object> Filter { get; set; }

        public static async Task<TawoFinder> Show(Predicate<object> filter)
        {
            var dlg = new TawoFinder
            {
                Filter = filter
            };
            var res = await DialogHost.Show(dlg, "InternalDialog");
            if (res == null) return null;
            if (!(res is bool b && b)) return null;
            return dlg;
        }

        public static async Task<Tawo> Show()
        {
            var dlg = new TawoFinder();
            dlg.RelationVisibility = Visibility.Collapsed;
            var res = await DialogHost.Show(dlg, "InternalDialog");
            if (res == null) return null;
            if (!(res is bool b && b)) return null;
            if (dlg.UseSearch)
                return dlg.People.CurrentItem as Tawo;
            return dlg.NewTawo;
        }
    }
}
