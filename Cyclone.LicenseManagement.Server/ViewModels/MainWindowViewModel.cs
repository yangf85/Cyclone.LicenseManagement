using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Mapster;
using Cyclone.LicenseManagement.Server.Core;
using Standard.Licensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;

namespace Cyclone.LicenseManagement.Server.ViewModels
{
    public partial class MainWindowViewModel : ObservableValidator
    {
        private int _langIndex = 0;

        public int LangIndex
        {
            get => _langIndex;
            set
            {
                SetProperty(ref _langIndex, value);
                UpdateLanguage();
            }
        }

        public MainWindowViewModel()
        {
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            var culture = LangIndex switch
            {
                0 => new CultureInfo("en-US"),
                _ => new CultureInfo("zh-CN"),
            };
            I18NExtension.Culture = culture;
        }
    }
}