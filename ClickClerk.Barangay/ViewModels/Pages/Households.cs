using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClickClerk.Barangay.Dialogs;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;
using MaterialDesignThemes.Wpf;

namespace ClickClerk.Barangay.ViewModels.Pages
{
    class Households:PageBase
    {
        private Households()
        {
            CanSearch = true;
        }

        private static Households _instance;
        public static Households Instance => _instance ?? (_instance = new Households());

        private ObservableCollection<Models.Household> _items;
        public ObservableCollection<Models.Household> Items => _items ?? (_items = Models.Household.GetAll());

        private ICommand _addCommand;
        public ICommand AddCommand => _addCommand ?? (_addCommand = new DelegateCommand(async d =>
        {
            var res = await HouseholdDialog.Show();
            
            if (res == null) return;
            if (res.Data.Save() > 0)
            {
                Items.Add(res.Data);
            }
        }));

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand<Household>(async d =>
        {
            if (await MessageDialog.Show("Are you sure you want to delete this household?",
                $"Household number {d.Number} will be removed from the database. Click DELETE to confirm action.",
                "DELETE", "CANCEL", PackIconKind.DeleteForever))
            {
                d.Delete();
            }
        }));

        private ICommand _addMemberCommand;

        public ICommand AddMemberCommand => _addMemberCommand ?? (_addMemberCommand = new DelegateCommand<Household>(
        async d =>
        {
            var dlg = await TawoFinder.Show(o =>
            {
                if (!(o is Tawo t)) return false;
                return !t.HasHousehold;
            });

            if (dlg == null) return;
            var tawo = dlg.UseSearch ? (Tawo) dlg.People.CurrentItem : dlg.NewTawo;
            tawo.HasHousehold = true;
            tawo.Save();
            if (tawo.Id > 0)
            {
                var member = new HouseholdMember()
                {
                    TawoId = tawo.Id,
                    HouseholdId = d.Id,
                    Position = dlg.Position
                };
                member.Save();
                d.Members.Add(member);
            }
        }));
    }
}
