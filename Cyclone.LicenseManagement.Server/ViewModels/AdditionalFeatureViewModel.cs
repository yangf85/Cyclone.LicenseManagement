using CommunityToolkit.Mvvm.ComponentModel;
using Cyclone.LicenseManagement.Server.Core;
using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyclone.LicenseManagement.Server.ViewModels;

[AdaptTo(typeof(AdditionalFeature))]
public partial class AdditionalFeatureViewModel : ObservableValidator
{
    private string _key;

    private string _value;

    [Required]
    public string Key
    {
        get => _key;
        set => SetProperty(ref _key, value, true);
    }

    [Required]
    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value, true);
    }
}