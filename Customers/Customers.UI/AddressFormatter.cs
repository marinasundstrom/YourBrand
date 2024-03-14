using System;
namespace YourBrand.Customers;
using YourBrand.Customers.Client;

public static class AddressFormatter
{
    public static string ToAddressString(this Address address) => $"{address.Thoroughfare} {address.Premises} {address.SubPremises}, {address.PostalCode} {address.Locality} {address.Country}";
}
