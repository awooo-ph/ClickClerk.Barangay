using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ClickClerk.Barangay.Dialogs;
using ClickClerk.Barangay.Models;
using ClickClerk.Barangay.Tools;
using MaterialDesignThemes.Wpf;
using Spire.Doc;
using Xceed.Words.NET;

namespace ClickClerk.Barangay.ViewModels.Pages
{
    class Certificates:PageBase
    {
        private Certificates()
        {
            CanSearch = true;
            _items = Certificate.GetAll();
        }

        private static Certificates _instance;
        public static Certificates Instance => _instance ?? (_instance = new Certificates());

        private readonly ObservableCollection<Certificate> _items;
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
            if (!(obj is Certificate h)) return false;
            if (string.IsNullOrEmpty(SearchKeyword)) return true;
            if (h.Tawo?.Fullname?.ToLower()?.Contains(SearchKeyword.ToLower()) ?? false) return true;
            return false;
        }

        protected override void OnSearch()
        {
            _itemsView?.Refresh();
        }

        private ICommand _printCommand;
        public ICommand PrintCommand => _printCommand ?? (_printCommand = new DelegateCommand<Certificate>(async certificate =>
        {
            //var selection = _items.Where(x => x.IsSelected).ToList();
            //if (selection.Count == 0)
            //{
            //    await MessageDialog.Show("Select Certification to Print",
            //        "You must select at least one certification to print.\nUse SHIFT or CTRL keys to select multiple items.", "OKAY");
            //    return;
            //}

            //foreach (var certificate in selection)
            //{
                var path = GetCertificatePath(certificate);
                if (!File.Exists(path)) await GenerateCertificate(certificate);

                Process.Start(new ProcessStartInfo(path)
                    {
                        //Verb = "Print",
                        //CreateNoWindow = true,
                        //WindowStyle = ProcessWindowStyle.Hidden
                    });

          

            //}
        },d=>d!=null));

        private ICommand _addCommand;
        public ICommand AddCommand => _addCommand ?? (_addCommand = new DelegateCommand(async d =>
        {
            var tawo = await TawoFinder.Show();
            if (tawo == null) return;

            var res = await CertificationDialog.Show(tawo);
            
            if (res == null) return;

            if (res.Certificates.SelectedItems.Count == 0) return;

            res.Data.TawoId = tawo.Save();
            var cn = Certificate.GetNextNumber();
            foreach (CertificateTypes certificate in res.Certificates.SelectedItems)
            {
                var cert = new Certificate()
                {
                    ControlNumber = cn,
                    CertificateType = certificate,
                    BarangayCaptain = Settings.Instance.Captain,
                    Secretary = Settings.Instance.Secretary,
                    Tawo = res.Data.Tawo,
                    CtcIssued = res.Data.CtcIssued,
                    CtcNumber = res.Data.CtcNumber,
                    CtcPlace = res.Data.CtcPlace,
                    CustomTemplate = res.Data.CustomTemplate,
                    DateIssued = res.Data.DateIssued,
                    OrNumber = res.Data.OrNumber,
                    Purpose = res.Data.Purpose,
                    Remarks = res.Data.Remarks,
                };
                cert.Save();
                _items.Add(cert);
                var path = await GenerateCertificate(cert);
                if (res.PrintCertificate)
                    try
                    {
                        var info = new ProcessStartInfo(path);
                        //info.Verb = "Print";
                       // info.CreateNoWindow = true;
                      //  info.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(info);
                    }
                    catch (Exception e)
                    {
                        //
                    }
            }
        }));

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand<Certificate>(async d =>
        {
            if (await MessageDialog.Show("Are you sure you want to delete this certification?",
                $"Certification number {d.ControlNumber} will be removed from the database. Click DELETE to confirm action.",
                "DELETE", "CANCEL", PackIconKind.DeleteForever))
            {
                d.Delete();
            }
        }, d => MainViewModel.Instance.CurrentUser?.IsAdmin ?? false));

        private string ToNth(int number)
        {
            if (number == 1) return "1ST";
            if (number == 2) return "2ND";
            if (number == 3) return "3RD";
            return $"{number}TH";
        }

        private string GetCertificatePath(Certificate certificate)
        {
            var path = Path.Combine(".", "Certifications");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return Path.Combine(path, $"{certificate.ControlNumber} [{certificate.CertificateType}] {certificate.Tawo.Fullname}.pdf");
        }

        public async Task<string> GenerateCertificate(Certificate certificate)
        {
            byte[] resource;
            switch (certificate.CertificateType)
            {
                case CertificateTypes.GoodMoral:
                    resource = Properties.Resources.Residence;
                    break;
                case CertificateTypes.SoloParent:
                    resource = Properties.Resources.SoloParent;
                    break;
                case CertificateTypes.PWD:
                    resource = Properties.Resources.PWD;
                    break;
                case CertificateTypes.SSS:
                    resource = Properties.Resources.SSS;
                    break;
                case CertificateTypes.Death:
                    resource = Properties.Resources.DeathCertificate;
                    break;
                case CertificateTypes.Indigent:
                    resource = Properties.Resources.Indigency;
                    break;
                default:
                    resource = Properties.Resources.Residence;
                    break;
            }

            var path = GetCertificatePath(certificate);
            
            using(var stream = new MemoryStream(resource))
            using (var doc = new Document(stream))
            {
                //doc.AddPasswordProtection(EditRestrictions.readOnly, "awooo");
                doc.Replace("[CN]", certificate.ControlNumber.ToUpper() ?? "-", true, true);
                doc.Replace("[DATE]", certificate.DateIssued.ToString("d").ToUpper(), true, true);
                doc.Replace("[MONTH]", certificate.DateIssued.ToString("MMMM").ToUpper(), true, true);
                doc.Replace("[DAY]",ToNth(certificate.DateIssued.Day), true, true);
                doc.Replace("[YEAR]",certificate.DateIssued.Year.ToString(), true, true);
                doc.Replace("[NAME]",certificate.Tawo.Fullname.ToUpper(), true, true);
                doc.Replace("[AGE]",certificate.Tawo.Age.ToString()??"", true, true);
                doc.Replace("[BIRHTDATE]",certificate.Tawo.BirthDate?.ToString("D")??"", true, true);
                doc.Replace("[ADDRESS]",certificate.Tawo.Address??"", true, true);
                doc.Replace("[CTC#]",certificate.ControlNumber??"", true, true);
                doc.Replace("[CTC_DATE]",certificate.CtcIssued.ToString("D"), true, true);
                doc.Replace("[CTC_PLACE]",certificate.CtcPlace??"", true, true);
                doc.Replace("[OR_NUMBER]",certificate.OrNumber??"", true, true);
                doc.Replace("[PURPOSE]",certificate.Purpose??"", true, true);
                doc.Replace("[REMARKS]",certificate.Remarks??"", true, true);
                doc.Replace("[CAPTAIN]",certificate.BarangayCaptain??"", true, true);
                doc.Replace("[SECRETARY]",certificate.Secretary??"", true, true);

                if (certificate.CertificateType == CertificateTypes.SSS)
                {
                    //if (string.IsNullOrEmpty(certificate.Tawo.IncomeSource))
                        while (string.IsNullOrEmpty(certificate.Tawo.IncomeSource))
                        {
                            var s = await InputDialog.Show("Income Source",
                                $"Enter {certificate.Tawo.Fullname}'s source of income.", "", "ACCEPT","CANCEL");
                            certificate.Tawo.IncomeSource = s;
                            if(!string.IsNullOrEmpty(s)) break;
                        }

                    if ((certificate.Tawo.IncomeMonthly ?? 0) == 0)
                    {
                        var dd = 0.0;
                        while (true)
                        {
                            var date = await InputDialog.Show("Monthly Income",
                                $"Enter {certificate.Tawo.Fullname}'s monthly income:", "", "ACCEPT","CANCEL");
                            if (double.TryParse(date, out dd)) break;
                        }

                        certificate.Tawo.IncomeMonthly = dd;
                    }

                    if (certificate.Tawo.IncomeStart == null)
                    {
                        DateTime d = DateTime.Now;
                        while (true)
                        {
                            var date = await InputDialog.Show("Income Started",
                                $"When did {certificate.Tawo.Fullname} started {certificate.Tawo.IncomeSource}?", "",
                                "ACCEPT", "CANCEL");
                            if (DateTime.TryParse(date, out d)) break;
                        }
                        certificate.Tawo.IncomeStart = d;
                    }
                    certificate.Tawo.Save();

                    doc.Replace("[INCOME_SOURCE]", certificate.Tawo.IncomeSource ?? "_______", true, true);
                    doc.Replace("[INCOME_START]", certificate.Tawo.IncomeStart?.ToString("d") ?? "_______", true, true);
                    var income = certificate.Tawo.IncomeMonthly ?? 0;
                    doc.Replace("[MONTHLY_INCOME]", income.ToString("#,##0.00"), true, true);
                } else if (certificate.CertificateType == CertificateTypes.Death)
                {
                    if (certificate.Tawo.DeathDate == null)
                    {
                        DateTime d = DateTime.Now;
                        while (true)
                        {
                            var date = await InputDialog.Show("Date of Death",
                                $"When did {certificate.Tawo.Fullname} died?", "",
                                "ACCEPT", "CANCEL");
                            if (DateTime.TryParse(date, out d)) break;
                        }
                        certificate.Tawo.DeathDate = d;
                    }

                    certificate.Tawo.Save();
                    doc.Replace("[DEATH_DATE]", certificate.Tawo.DeathDate?.ToString("d") ?? "_______", true, true);
                    doc.Replace("[AGE]", certificate.Tawo.Age?.ToString() ?? "____", true, true);
                }

                
                doc.SaveToFile(path, FileFormat.PDF);
                
            }

            return path;
            
        }
    }
}
