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
using Cyclone.LicenseManagement.Client;
using System.Collections.ObjectModel;
using Cyclone.LicenseManagement.Server.Properties;

namespace Cyclone.LicenseManagement.Server.ViewModels
{
    public partial class LicenseDetailViewModel : ObservableValidator
    {
        [ObservableProperty]
        private bool _isGeneration;

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
        private int _quantity;

        [ObservableProperty]
        private string _signature;

        private LicenseKeypair _licenseKeypair;

        private License _license;

        [ObservableProperty]
        private bool? _isLicenseValid;

        public ObservableCollection<AdditionalFeatureViewModel> AdditionalFeatures { get; private set; }

        [Required]
        public string Passphrase
        {
            get => _passphrase;
            set
            {
                SetProperty(ref _passphrase, value, true);
                GenerateLicenseKeypairCommand.NotifyCanExecuteChanged();
            }
        }

        [Required]
        public string CustomerName
        {
            get => _customerName;
            set
            {
                SetProperty(ref _customerName, value, true);
                GenerateLicenseKeypairCommand.NotifyCanExecuteChanged();
            }
        }

        [Required]
        [EmailAddress]
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
                        _ => LicenseType.Trial,
                    };
                };
            }
        }

        public LicenseDetailViewModel()
        {
            Init();
        }

        private void Init()
        {
            IsGeneration = true;
            SelectedLicenseIndex = 1;
            UniqueIdentifier = Guid.NewGuid();
            LicenseType = LicenseType.Trial;
            CustomerName = string.Empty;
            CustomerEmail = string.Empty;
            PublicKey = string.Empty;
            Passphrase = string.Empty;
            Signature = string.Empty;
            Quantity = 1;
            ActivationDays = 30;
            ExpirationDate = DateTime.Now.AddDays(ActivationDays);

            AdditionalFeatures = new();
            AdditionalFeatures.Add(new AdditionalFeatureViewModel() { Key = "Hardware", Value = string.Empty });
        }

        [RelayCommand]
        private void RemoveAdditionalFeature(AdditionalFeatureViewModel additionalFeature)
        {
            if (additionalFeature != null)
            {
                AdditionalFeatures.Remove(additionalFeature);
            }
        }

        [RelayCommand]
        private void AddAdditionalFeature()
        {
            AdditionalFeatures.Add(new AdditionalFeatureViewModel());
        }

        [RelayCommand]
        private void GenerateUniqueIdentifier()
        {
            UniqueIdentifier = Guid.NewGuid();
        }

        private bool CanGenerateLicenseKeypair() => !HasErrors;

        [RelayCommand(CanExecute = nameof(CanGenerateLicenseKeypair))]
        private void GenerateLicenseKeypair()
        {
            _license = LicenseGenerator.Generate(this.Adapt<LicenseDetail>(), out _licenseKeypair);

            Signature = _license.Signature;
            PublicKey = _licenseKeypair.PublicKey;
        }

        private bool CanSaveLicenseFile()
        {
            return !HasErrors && !string.IsNullOrWhiteSpace(PublicKey) && IsGeneration;
        }

        [RelayCommand(CanExecute = nameof(CanSaveLicenseFile))]
        private void SaveLicenseFile()
        {
            try
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void LoadLicenseFile()
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "License Files (*.lic)|*.lic";
                if (openFileDialog.ShowDialog() == true)
                {
                    _license = LicenseValidator.Read(openFileDialog.FileName);
                    CustomerName = _license.Customer.Name;
                    CustomerEmail = _license.Customer.Email;
                    UniqueIdentifier = _license.Id;
                    LicenseType = _license.Type;
                    ExpirationDate = _license.Expiration;
                    Quantity = _license.Quantity;
                    PublicKey = _license.AdditionalAttributes.Get("PublicKey");
                    ActivationDays = int.Parse(_license.AdditionalAttributes.Get("ActivationDays"));
                    Signature = _license.Signature;
                }
                IsGeneration = false;
                ValidateLicenseCommand.NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void UnloadLicenseFile()
        {
            _license = null;
            Init();
            IsGeneration = true;
            IsLicenseValid = null;
            ValidateLicenseCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanValidateLicense))]
        private void ValidateLicense()
        {
            try
            {
                IsLicenseValid = LicenseValidator.Validate(_license).IsValid;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanValidateLicense() => !IsGeneration && _license != null;
    }
}