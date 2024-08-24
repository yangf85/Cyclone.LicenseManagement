using Standard.Licensing;

namespace Cyclone.LicenseManagement.Server.Core;

public class LicenseDetail
{
    public Guid UniqueIdentifier { get; set; }

    public LicenseType LicenseType { get; set; }

    public string CustomerName { get; set; }

    public string CustomerEmail { get; set; }

    public DateTime ExpirationDate { get; set; }

    public DateTime ActivationDate { get; set; }

    public int Quantity { get; set; }

    public string Passphrase { get; set; }

    public int ActivationDays { get; set; }

    public string Salt { get; set; }

    public LicenseDetail()
    {
        UniqueIdentifier = Guid.NewGuid();
        LicenseType = LicenseType.Trial;
        CustomerName = string.Empty;
        CustomerEmail = string.Empty;
        Passphrase = string.Empty;
        Quantity = 1;
        ActivationDays = 30;
        ActivationDate = DateTime.Now;
        ExpirationDate = ActivationDate.AddDays(ActivationDays);
        Salt = "LicenseSalt";
    }
}