// See https://aka.ms/new-console-template for more information
using Cyclone.LicenseManagement.Client;

var path = @"F:\OneDrive\桌面\A-A@B.lic";

var license = LicenseValidator.Read(path);

var result = LicenseValidator.Validate(license);

Console.WriteLine(result);