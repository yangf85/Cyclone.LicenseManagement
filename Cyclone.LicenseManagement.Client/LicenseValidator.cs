using Standard.Licensing;
using Standard.Licensing.Validation;
using System.Xml.Serialization;

namespace Cyclone.LicenseManagement.Client;

public class LicenseValidator
{
    public static async Task<License> LoadAsync(string path)
    {
        var licenseContent = await File.ReadAllTextAsync(path);
        var license = License.Load(licenseContent);
        return license;
    }

    public static async Task<ValidationResult> ValidateAsync(License license, params ILicenseAttributeValidator[] licenseAttributeValidator)
    {
        var result = new ValidationResult();
        try
        {
            // 先执行自定义的验证
            if (licenseAttributeValidator != null)
            {
                foreach (var additionalValidator in licenseAttributeValidator)
                {
                    var value = license.AdditionalAttributes.Get(additionalValidator.AttributeName);
                    result = additionalValidator.Validate(value);
                    return result;
                }
            }

            // 提取公共密钥
            var publicKey = license.AdditionalAttributes.Get("PublicKey");

            var date = await NetworkTimer.GetTimeAsync() ?? DateTime.Now;

            // 验证许可证签名和过期时间
            var errors = license.Validate()
                                .ExpirationDate(date)
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
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
        }

        return result;
    }
}