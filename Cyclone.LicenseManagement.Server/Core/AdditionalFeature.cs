using Cyclone.LicenseManagement.Server.ViewModels;
using Mapster;
using System.Xml.Serialization;

namespace Cyclone.LicenseManagement.Server.Core;

[AdaptTo(typeof(AdditionalFeatureViewModel))]
public class AdditionalFeature
{
    [XmlAttribute(nameof(Key))]
    public string Key { get; set; }

    [XmlAttribute(nameof(Value))]
    public string Value { get; set; }
}