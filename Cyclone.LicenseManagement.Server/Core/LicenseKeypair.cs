using Standard.Licensing;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Cyclone.LicenseManagement.Server.Core;

[XmlRoot(nameof(LicenseKeypair))]
public class LicenseKeypair
{
    [XmlElement(nameof(PublicKey), Order = 0)]
    public string PublicKey { get; set; }

    [XmlElement(nameof(PrivateKey), Order = 1)]
    public string PrivateKey { get; set; }

    [XmlElement(nameof(Passphrase), Order = 2)]
    public string Passphrase { get; set; }

    [XmlElement(nameof(Salt), Order = 3)]
    public string Salt { get; set; }

    [XmlElement(nameof(CustomerName), Order = 4)]
    public string CustomerName { get; set; }

    [XmlElement(nameof(CustomerEmail), Order = 5)]
    public string CustomerEmail { get; set; }

    [XmlElement(nameof(UniqueIdentifier), Order = 6)]
    public Guid UniqueIdentifier { get; set; }

    [XmlElement(nameof(LicenseType), Order = 7)]
    public LicenseType LicenseType { get; set; }

    [XmlElement(nameof(Quantity), Order = 8)]
    public int Quantity { get; set; }

    [XmlElement(nameof(ActivationDate), Order = 9)]
    public DateTime ActivationDate { get; set; }

    [XmlElement(nameof(ActivationDays), Order = 10)]
    public int ActivationDays { get; set; }

    [XmlElement(nameof(ExpirationDate), Order = 11)]
    public DateTime ExpirationDate { get; set; }

    [XmlArray(nameof(AdditionalFeatures), Order = 12), XmlArrayItem("Feature")]
    public List<AdditionalFeature> AdditionalFeatures { get; set; } = [];
}