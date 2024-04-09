using System.Text.Json;
using System.Text.Json.Serialization;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Application.Services;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Entities;

namespace YourBrand.Customers.Features.Customers.Import;

public record CustomerImportResult(IEnumerable<string> Diagnostics);

public sealed record ImportCustomers(Stream Stream) : IRequest<Result<CustomerImportResult>>
{

    public sealed class Handler(ICustomersContext context, IBlobStorageService blobStorageService) : IRequestHandler<ImportCustomers, Result<CustomerImportResult>>
    {
        public async Task<Result<CustomerImportResult>> Handle(ImportCustomers request, CancellationToken cancellationToken)
        {
            List<string> diagnostics = new();

            var customerRecords = JsonSerializer.Deserialize<CustomerRecord[]>(request.Stream, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = {
                    new JsonStringEnumConverter()
                }
            })!;

            foreach (CustomerRecord customerRecord in customerRecords)
            {
                Customer? customer = null;

                if (customerRecord is OrganizationCustomerRecord org)
                {
                    var exists = await context.Organizations.AnyAsync(x => x.OrganizationNo == org.OrgNo, cancellationToken);

                    if (exists)
                    {
                        diagnostics.Add($"Customer with Org No {org.OrgNo} already exists");
                        continue;
                    }

                    var organization = new Organization(customerRecord.Name!, org.OrgNo!, org.VatNo!);

                    customer = organization;
                }
                else if (customerRecord is IndividualCustomerRecord ind)
                {
                    var exists = await context.Persons.AnyAsync(x => x.Ssn == ind.Ssn, cancellationToken);

                    if (exists)
                    {
                        diagnostics.Add($"Customer with SSN {ind.Ssn} already exists");
                        continue;
                    }

                    var person = new Person(ind.FirstName!, ind.LastName!, ind.Ssn!);

                    customer = person;
                }

                if (customer is null) continue;

                customer.Name = customerRecord.Name!;
                customer.Phone = customerRecord.Phone;
                customer.PhoneMobile = customerRecord.PhoneMobile;
                customer.Email = customerRecord.Email;

                foreach (var address in customerRecord.Addresses)
                {
                    var a = new Address
                    {
                        Type = (Domain.Enums.AddressType)address.Type,
                        Thoroughfare = address.Thoroughfare,
                        Premises = address.Premises,
                        SubPremises = address.SubPremises,
                        PostalCode = address.PostalCode,
                        Locality = address.Locality,
                        SubAdministrativeArea = address.SubAdministrativeArea,
                        AdministrativeArea = address.AdministrativeArea,
                        Country = address.Country
                    };
                    customer.AddAddress(a);
                }

                context.Customers.Add(customer);
            }

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success(new CustomerImportResult(diagnostics));
        }


        [JsonPolymorphic(TypeDiscriminatorPropertyName = "customerType")]
        [JsonDerivedType(typeof(IndividualCustomerRecord), typeDiscriminator: "Individual")]
        [JsonDerivedType(typeof(OrganizationCustomerRecord), typeDiscriminator: "Organization")]
        public class CustomerRecord
        {
            public CustomerTypeDto CustomerType { get; set; }
            public string? Name { get; set; }
            public string? Phone { get; set; }
            public string PhoneMobile { get; set; } = default!;
            public string Email { get; set; } = default!;
            public IEnumerable<CustomerAddressRecord> Addresses { get; set; } = default!;
        }

        public class OrganizationCustomerRecord : CustomerRecord
        {
            public string? OrgNo { get; set; }
            public string? VatNo { get; set; }
        }

        public class IndividualCustomerRecord : CustomerRecord
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Ssn { get; set; }
        }

        public record class CustomerAddressRecord(
            AddressTypeDto Type,

            // Street
            string Thoroughfare,

            // Street number
            string? Premises,

            // Suite
            string? SubPremises,

            string PostalCode,

            // Town or City
            string Locality,

            // County
            string SubAdministrativeArea,

            // State
            string AdministrativeArea,

            string Country
        );

        public enum CustomerTypeDto
        {
            Individual = 1,
            Organization = 2
        }

        public enum AddressTypeDto
        {
            Delivery = 1,
            Billing = 2
        }
    }
}