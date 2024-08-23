using Standard.Licensing;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Cyclone.LicenseManagement.Server.Core;

[Serializable]
public class LicenseKeypair : IXmlSerializable
{
    public string Passphrase { get; set; }

    public string Salt { get; set; }

    public string PublicKey { get; set; }

    public string PrivateKey { get; set; }

    public Guid UniqueIdentifier { get; set; }

    public LicenseType LicenseType { get; set; }

    public int MaximumUtilization { get; set; }

    public DateTime ActivationDate { get; set; }

    public int ActivationDays { get; set; }

    public DateTime ExpirationDate { get; set; }

    public string CustomerName { get; set; }

    public string CustomerEmail { get; set; }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();

        Passphrase = reader.ReadElementContentAsString(nameof(Passphrase), "");
        Salt = reader.ReadElementContentAsString(nameof(Salt), "");
        PublicKey = reader.ReadElementContentAsString(nameof(PublicKey), "");
        PrivateKey = reader.ReadElementContentAsString(nameof(PrivateKey), "");
        UniqueIdentifier = new Guid(reader.ReadElementContentAsString(nameof(UniqueIdentifier), ""));
        LicenseType = (LicenseType)Enum.Parse(typeof(LicenseType), reader.ReadElementContentAsString(nameof(LicenseType), ""));
        MaximumUtilization = reader.ReadElementContentAsInt(nameof(MaximumUtilization), "");
        ActivationDate = DateTime.ParseExact(reader.ReadElementContentAsString(nameof(ActivationDate), ""), "yyyy-MM-dd", null);
        ActivationDays = reader.ReadElementContentAsInt(nameof(ActivationDays), "");
        ExpirationDate = DateTime.ParseExact(reader.ReadElementContentAsString(nameof(ExpirationDate), ""), "yyyy-MM-dd", null);
        CustomerName = reader.ReadElementContentAsString(nameof(CustomerName), "");
        CustomerEmail = reader.ReadElementContentAsString(nameof(CustomerEmail), "");
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString(nameof(Passphrase), Passphrase);
        writer.WriteElementString(nameof(Salt), Salt);
        writer.WriteElementString(nameof(PublicKey), PublicKey);
        writer.WriteElementString(nameof(PrivateKey), PrivateKey);
        writer.WriteElementString(nameof(UniqueIdentifier), UniqueIdentifier.ToString());
        writer.WriteElementString(nameof(LicenseType), LicenseType.ToString());
        writer.WriteElementString(nameof(MaximumUtilization), MaximumUtilization.ToString());
        writer.WriteElementString(nameof(ActivationDate), ActivationDate.ToString("yyyy-MM-dd"));
        writer.WriteElementString(nameof(ActivationDays), ActivationDays.ToString());
        writer.WriteElementString(nameof(ExpirationDate), ExpirationDate.ToString("yyyy-MM-dd"));
        writer.WriteElementString(nameof(CustomerName), CustomerName);
        writer.WriteElementString(nameof(CustomerEmail), CustomerEmail);
    }
}