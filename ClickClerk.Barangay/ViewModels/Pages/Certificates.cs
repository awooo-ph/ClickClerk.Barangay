using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    class Certificates:PageBase
    {
        private Certificates()
        {
            CanSearch = true;
        }

        private static Certificates _instance;
        public static Certificates Instance => _instance ?? (_instance = new Certificates());

        private ObservableCollection<Certificate> _items;
        public ObservableCollection<Certificate> Items => _items ?? (_items = Certificate.GetAll());

        private ICommand _addCommand;
        public ICommand AddCommand => _addCommand ?? (_addCommand = new DelegateCommand(async d =>
        {
            var tawo = await TawoFinder.Show();
            if (tawo == null) return;

            var res = await CertificationDialog.Show(tawo);
            
            if (res == null) return;
            res.Data.TawoId = tawo.Save();
            res.Data.Save();
            Items.Add(res.Data);
            var path = GenerateCertificate(res.Data);
            if(res.PrintCertificate)
                Process.Start("winword.exe", $"\"{path}\"");
        }));

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand<CaseFile>(async d =>
        {
            if (await MessageDialog.Show("Are you sure you want to delete this certification?",
                $"Certification number {d.CaseNumber} will be removed from the database. Click DELETE to confirm action.",
                "DELETE", "CANCEL", PackIconKind.DeleteForever))
            {
                d.Delete();
            }
        }));

        private string ToNth(int number)
        {
            if (number == 1) return "1ST";
            if (number == 2) return "2ND";
            if (number == 3) return "3RD";
            return $"{number}TH";
        }

        public string GenerateCertificate(Certificate certificate)
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
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var path = Path.Combine(".", "Certifications");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, $"{certificate.ControlNumber} [{certificate.CertificateType}] {certificate.Tawo.Fullname}.docx");

            using(var stream = new MemoryStream(resource))
            using (var doc = DocX.Load(stream))
            {
                doc.ReplaceText("[CN]", certificate.ControlNumber.ToUpper() ?? "-");
                doc.ReplaceText("[DATE]", certificate.DateIssued.ToString("d").ToUpper());
                doc.ReplaceText("[MONTH]", certificate.DateIssued.ToString("MMMM").ToUpper());
                doc.ReplaceText("[DAY]",ToNth(certificate.DateIssued.Day));
                doc.ReplaceText("[YEAR]",certificate.DateIssued.Year.ToString());
                doc.ReplaceText("[NAME]",certificate.Tawo.Fullname.ToUpper());
                doc.ReplaceText("[AGE]",certificate.Tawo.Age.ToString()??"");
                doc.ReplaceText("[BIRHTDATE]",certificate.Tawo.BirthDate?.ToString("D")??"");
                doc.ReplaceText("[ADDRESS]",certificate.Tawo.Address??"");
                doc.ReplaceText("[CTC#]",certificate.ControlNumber??"");
                doc.ReplaceText("[CTC_DATE]",certificate.CtcIssued.ToString("D"));
                doc.ReplaceText("[CTC_PLACE]",certificate.CtcPlace??"");
                doc.ReplaceText("[OR_NUMBER]",certificate.OrNumber??"");
                doc.ReplaceText("[PURPOSE]",certificate.Purpose??"");
                doc.ReplaceText("[REMARKS]",certificate.Remarks??"");
                
                
                doc.SaveAs(path);
            }

            return path;
            
        }
    }
}
