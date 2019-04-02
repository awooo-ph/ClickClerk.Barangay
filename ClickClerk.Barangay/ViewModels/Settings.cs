using Jot;
using Jot.DefaultInitializer;
using Jot.Storage;
using Jot.Triggers;

namespace ClickClerk.Barangay.ViewModels
{
    class Settings:ViewModelBase
    {
        public static StateTracker Tracker { get; } = new StateTracker(new JsonFileStoreFactory(), new DesktopPersistTrigger());

        private Settings()
        {
            Tracker.Configure(this)
                .Apply();
        }

        private static Settings _instance;
        public static Settings Instance => _instance ?? (_instance = new Settings());

        private string _Captain;
        [Trackable]
        public string Captain
        {
            get => _Captain;
            set
            {
                if (value == _Captain) return;
                _Captain = value;
                OnPropertyChanged(nameof(Captain));
            }
        }

        private string _Secretary;
        [Trackable]
        public string Secretary
        {
            get => _Secretary;
            set
            {
                if (value == _Secretary) return;
                _Secretary = value;
                OnPropertyChanged(nameof(Secretary));
            }
        }


    }
}
