using Standard.Licensing;
using System.Xml.Serialization;

namespace Cyclone.LicenseManagement.Client;

public class LicenseValidator
{
    public static bool Validate(string licenseFilePath)
    {
        License license = null;

        try
        {
            // 读取许可证文件
            var licenseContent = File.ReadAllText(licenseFilePath);
            license = License.Load(licenseContent);

            // 提取公共密钥
            var publicKey = license.ProductFeatures.Get("PublicKey").ToString();

            // 验证许可证签名
            if (!license.VerifySignature(publicKey))
            {
                return false;
            }

            // 验证许可证的有效期
            if (license.Expiration < DateTime.Now)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}