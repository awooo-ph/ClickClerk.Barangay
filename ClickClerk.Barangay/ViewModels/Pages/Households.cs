﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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
            //Messenger.Default.AddListener<HouseholdMember>(Messages.HouseHoldMemberDeleted, m =>
           // {
               
            //});
        }

        private static Households _instance;
        public static Households Instance => _instance ?? (_instance = new Households());

        private readonly ObservableCollection<Models.Household> _items = Models.Household.GetAll();
        private ListCollectionView _itemsView;
        public ListCollectionView Items
        {
            get
            {
                if (_itemsView != null) return _itemsView;
                _itemsView = new ListCollectionView(_items);
                _itemsView.Filter = Filter;
                return _itemsView;
            }
        }

        private bool Filter(object obj)
        {
            if (!(obj is Household h)) return false;
            if (string.IsNullOrEmpty(SearchKeyword)) return true;
            if (h.Incharge?.Fullname?.ToLower()?.Contains(SearchKeyword.ToLower())??false) return true;
            return false;
        }

        protected override void OnSearch()
        {
            _itemsView?.Refresh();
        }

        private ICommand _addCommand;
        public ICommand AddCommand => _addCommand ?? (_addCommand = new DelegateCommand(async d =>
        {
            var res = await HouseholdDialog.Show();
            
            if (res == null) return;
            if (res.Data.Save() > 0)
            {
                _items.Add(res.Data);
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
        }, d => MainViewModel.Instance.CurrentUser?.IsAdmin ?? false));

        private ICommand _removeMemberCommand;
        public ICommand RemoveMemberCommand => _removeMemberCommand ?? (_removeMemberCommand = new DelegateCommand<HouseholdMember>(
            d=>
            {
                d.Delete();
                _items.FirstOrDefault(x => x.Id == d.HouseholdId)?.Members.Remove(d);
                d.Tawo?.Update(nameof(Tawo.HasHousehold), false);
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
            //tawo.HasHousehold = true;
            //tawo.Save();
            if (tawo.Id > 0)
            {
                var member = HouseholdMember.GetByTawoId(tawo.Id) ?? new HouseholdMember()
                {
                    TawoId = tawo.Id,
                    HouseholdId = d.Id,
                    Position = dlg.Position
                };
                member.IsDeleted = false;
                member.Save();
                d.Members.Add(member);
            }
        }));

    }
}
