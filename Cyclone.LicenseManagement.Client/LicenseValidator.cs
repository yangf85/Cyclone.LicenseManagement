﻿using Standard.Licensing;
using Standard.Licensing.Validation;
using System.Xml.Serialization;

namespace Cyclone.LicenseManagement.Client;

public class LicenseValidator
{
    public static License Load(string path)
    {
        var licenseContent = File.ReadAllText(path);
        var license = License.Load(licenseContent);
        return license;
    }

    public static ValidationResult Validate(License license, params ILicenseAttributeValidator[] licenseAttributeValidators)
    {
        var result = new ValidationResult();
        try
        {
            // 先执行自定义的验证
            if (licenseAttributeValidators != null)
            {
                foreach (var additionalValidator in licenseAttributeValidators)
                {
                    var value = license.AdditionalAttributes.Get(additionalValidator.AttributeName);
                    result = additionalValidator.Validate(value);
                    if (!result.IsValid)
                    {
                        return result;
                    }
                }
            }

            // 提取公共密钥
            var publicKey = license.AdditionalAttributes.Get("PublicKey");

            var date = NetworkTimer.GetTime() ?? DateTime.Now;

            // 验证许可证签名和过期时间
            var errors = license.Validate()
                                .ExpirationDate(date)
                                .And()
                                .Signature(publicKey)
                                .AssertValidLicense();

            // 检查验证结果
            if (errors.Any())
            {
                result.IsValid = false;
                result.ErrorMessage = $"{errors.First().Message}";
                return result;
            }
            result.IsValid = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"An unexpected error occurred: {ex.Message}{Environment.NewLine}{ex.StackTrace}";
            result.IsValid = false;
        }

        return result;
    }
}