using Standard.Licensing;
using Standard.Licensing.Security.Cryptography;
using Standard.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cyclone.LicenseManagement.Server.Core;

public class LicenseGenerator
{
    public static void Save(string path, LicenseDetail model)
    {
        var license = Generate(model, out var licenseKeypair);

        File.WriteAllText(path, license.ToString());
        SaveKeypair(path, licenseKeypair);
    }

    public static License Generate(LicenseDetail model, out LicenseKeypair licenseKeypair)
    {
        licenseKeypair = new LicenseKeypair();

        var keyGenerator = KeyGenerator.Create();
        var keyPair = keyGenerator.GenerateKeyPair();
        var password = $"{model.Passphrase}-{model.Salt}";
        licenseKeypair.PrivateKey = keyPair.ToEncryptedPrivateKeyString(password);
        licenseKeypair.PublicKey = keyPair.ToPublicKeyString();
        licenseKeypair.Passphrase = model.Passphrase;
        licenseKeypair.UniqueIdentifier = model.UniqueIdentifier;
        licenseKeypair.LicenseType = model.LicenseType;
        licenseKeypair.ActivationDate = model.ActivationDate;
        licenseKeypair.ActivationDays = model.ActivationDays;
        licenseKeypair.ExpirationDate = model.ExpirationDate;
        licenseKeypair.MaximumUtilization = model.MaximumUtilization;
        licenseKeypair.Salt = model.Salt;

        var license = License.New()
            .WithUniqueIdentifier(model.UniqueIdentifier)
            .As(model.LicenseType)
            .ExpiresAt(model.ExpirationDate)
            .WithMaximumUtilization(1)
            .WithProductFeatures(new Dictionary<string, string>()
            {
                { "PublicKey", model.PublicKey },
                { "ActivationDate", model.ActivationDate.ToString("yyyy-MM-dd") },
                { "ActivationDays", model.ActivationDays.ToString() }
            })
            .LicensedTo(model.CustomerName, model.CustomerEmail)
            .CreateAndSignWithPrivateKey(licenseKeypair.PrivateKey, password);

        return license;
    }

    private static void SaveKeypair(string path, LicenseKeypair licenseKeypair)
    {
        var serializer = new XmlSerializer(typeof(LicenseKeypair));
        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty); // 去掉命名空间
        using var writer = new StringWriter();
        serializer.Serialize(writer, licenseKeypair, namespaces);
        File.WriteAllText(Path.ChangeExtension(path, ".key"), writer.ToString());
    }

    private static string HashPassword(string password, string salt, int iterations = 10000)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using (var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
        {
            return Convert.ToBase64String(rfc2898.GetBytes(32)); // 返回32字节的哈希结果
        }
    }

    private static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt, int iterations = 10000)
    {
        string hashedPassword = HashPassword(enteredPassword, storedSalt, iterations);
        return hashedPassword == storedHash;
    }
}