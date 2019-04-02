using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClickClerk.Barangay.Dialogs;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;
using MaterialDesignThemes.Wpf;
using Xceed.Words.NET;

namespace ClickClerk.Barangay.ViewModels.Pages
{
    class Cases : PageBase
    {
        private Cases()
        {
            CanSearch = true;
        }

        private static Cases _instance;
        public static Cases Instance => _instance ?? (_instance = new Cases());

        private ObservableCollection<Models.CaseFile> _items;
        public ObservableCollection<Models.CaseFile> Items => _items ?? (_items = new ObservableCollection<CaseFile>(Models.CaseFile.GetAll().Take(74)));

        private ICommand _addCommand;
        public ICommand AddCommand => _addCommand ?? (_addCommand = new DelegateCommand(async d =>
        {
            var res = await CaseDialog.Show();
            
            if (res == null) return;
            if (res.Data.Save() > 0)
            {
                foreach (var tawo in res.Data.Complainants)
                {
                    var complainant = new CaseComplainant()
                    {
                        CaseId = res.Data.Id,
                        TawoId = tawo.Id
                    };
                    complainant.Save();
                }

                foreach (var tawo in res.Data.Respondents)
                {
                    new CaseRespondent()
                    {
                        CaseId = res.Data.Id,
                        TawoId = tawo.Id
                    }.Save();
                }
                
                Items.Add(res.Data);
            }

        }));

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand<CaseFile>(async d =>
        {
            if (await MessageDialog.Show("Are you sure you want to delete this case?",
                $"Case number {d.CaseNumber} will be removed from the database. Click DELETE to confirm action.",
                "DELETE", "CANCEL", PackIconKind.DeleteForever))
            {
                d.Delete();
            }
        }, d => MainViewModel.Instance.CurrentUser?.IsAdmin ?? false));
    }
}
