using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MahApps.Metro.Controls.Dialogs;
using Mapster;
using Microsoft.Win32;
using Cyclone.LicenseManagement.Server.Core;
using Standard.Licensing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cyclone.LicenseManagement.Server.ViewModels
{
    public partial class LicenseDetailViewModel : ObservableValidator
    {
        [ObservableProperty]
        private Guid _uniqueIdentifier;

        [ObservableProperty]
        private LicenseType _licenseType;

        private string _passphrase;

        private string _customerName;

        private string _customerEmail;

        private int _activationDays;

        private int _selectedLicenseIndex;

        [ObservableProperty]
        private DateTime _expirationDate;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveLicenseFileCommand))]
        private string _publicKey;

        [ObservableProperty]
        private int _maximumUtilization;

        [ObservableProperty]
        private string _signature;

        private LicenseKeypair _licenseKeypair;

        private License _license;

        [Required(ErrorMessage = "密码口令是必填项。")]
        [StringLength(36, MinimumLength = 12, ErrorMessage = "密码口令的长度必须在 12 到 36 个字符之间。")]
        public string Passphrase
        {
            get => _passphrase;
            set
            {
                SetProperty(ref _passphrase, value, true);
                GenerateLicenseKeypairCommand.NotifyCanExecuteChanged();
            }
        }

        [Required(ErrorMessage = "客户名称是必填项。")]
        public string CustomerName
        {
            get => _customerName;
            set
            {
                SetProperty(ref _customerName, value, true);
                GenerateLicenseKeypairCommand.NotifyCanExecuteChanged();
            }
        }

        [Required(ErrorMessage = "客户电子邮件是必填项。")]
        [EmailAddress(ErrorMessage = "请输入有效的电子邮件地址。")]
        public string CustomerEmail
        {
            get => _customerEmail;
            set
            {
                SetProperty(ref _customerEmail, value, true);
                GenerateLicenseKeypairCommand.NotifyCanExecuteChanged();
            }
        }

        public int ActivationDays
        {
            get => _activationDays;
            set
            {
                if (SetProperty(ref _activationDays, value, true))
                {
                    ExpirationDate = DateTime.Now.AddDays(ActivationDays);
                }
            }
        }

        public int SelectedLicenseIndex
        {
            get => _selectedLicenseIndex;
            set
            {
                if (SetProperty(ref _selectedLicenseIndex, value, true))
                {
                    LicenseType = SelectedLicenseIndex switch
                    {
                        0 => LicenseType.Standard,
                        1 => LicenseType.Trial,
                    };
                };
            }
        }

        public LicenseDetailViewModel()
        {
            SelectedLicenseIndex = 0;
            UniqueIdentifier = Guid.NewGuid();
            LicenseType = LicenseType.Trial;
            CustomerName = string.Empty;
            CustomerEmail = string.Empty;
            PublicKey = string.Empty;
            Passphrase = string.Empty;
            MaximumUtilization = 1;
            ActivationDays = 30;
            ExpirationDate = DateTime.Now.AddDays(ActivationDays);
        }

        [RelayCommand]
        private Task GenerateUniqueIdentifier()
        {
            UniqueIdentifier = Guid.NewGuid();
            return Task.CompletedTask;
        }

        private bool CanGenerateLicenseKeypair() => !HasErrors;

        [RelayCommand(CanExecute = nameof(CanGenerateLicenseKeypair))]
        private Task GenerateLicenseKeypair()
        {
            _license = LicenseGenerator.Generate(this.Adapt<LicenseDetail>(), out _licenseKeypair);

            Signature = _license.Signature;
            PublicKey = _licenseKeypair.PublicKey;

            return Task.CompletedTask;
        }

        private bool CanSaveLicenseFile() => !string.IsNullOrWhiteSpace(PublicKey);

        [RelayCommand(CanExecute = nameof(CanSaveLicenseFile))]
        private Task SaveLicenseFile()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "License Files (*.lic)|*.lic";
            saveFileDialog.FileName = $"{CustomerName}-{CustomerEmail}.lic";
            if (saveFileDialog.ShowDialog() == true)
            {
                var model = this.Adapt<LicenseDetail>();
                model.Salt = "this is a salt";
                LicenseGenerator.Save(saveFileDialog.FileName, model);
            }

            return Task.CompletedTask;
        }
    }
}