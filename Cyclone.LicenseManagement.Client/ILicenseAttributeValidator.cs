using Standard.Licensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyclone.LicenseManagement.Client;

public interface ILicenseAttributeValidator
{
    string AttributeName { get; }

    ValidationResult Validate(string atttributeValue);
}