using Standard.Licensing;
using Standard.Licensing.Validation;
using System.Xml.Serialization;

namespace Cyclone.LicenseManagement.Client;

public class LicenseValidator
{
    public static License Read(string path)
    {
        var licenseContent = File.ReadAllText(path);
        var license = License.Load(licenseContent);
        return license;
    }

    public static ValidationResult Validate(License license, Func<LicenseAttributes, ValidationResult> additionalValidator = null)
    {
        var result = new ValidationResult();
        try
        {
            //先执行自定义的验证
            if (additionalValidator != null)
            {
                result = additionalValidator.Invoke(license.AdditionalAttributes);
                if (result != null && !result.IsValid)
                {
                    return result;
                }
            }

            // 提取公共密钥
            var publicKey = license.AdditionalAttributes.Get("PublicKey");

            // 验证许可证签名
            var errors = license.Validate()
                                .ExpirationDate()
                                .When(i => i.Type == LicenseType.Standard)
                                .And()
                                .Signature(publicKey)
                                .AssertValidLicense();

            // 检查验证结果
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    result.ErrorMessage += $"{error.Message} \n";
                }
                return result;
            }

            result.IsValid = true;

            return result;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            return result;
        }
    }
}