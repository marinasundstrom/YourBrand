using YourBrand.Invoices.Domain;
using YourBrand.Invoices.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;
using RotRut.Begaran;
using RotRut.Begaran.Rut;
using RotRut;
using System.Text;

namespace YourBrand.Invoices.Application.Commands;

public record CreateRutFile() : IRequest<string>
{
    public class Handler : IRequestHandler<CreateRutFile, string>
    {
        private readonly IInvoicesContext _context;

        public Handler(IInvoicesContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateRutFile request, CancellationToken cancellationToken)
        {
            var invoices = await _context.Invoices
                .Include(x => x.Items)
                .Where(x => x.Status == InvoiceStatus.Paid)
                .ToArrayAsync(cancellationToken);

            BegaranFil begaranFil = new BegaranFil()
            {
                NamnPaBegaran = "dsfsd"
            };

            begaranFil.HushallBegaran = new RotRut.Begaran.Rut.HushallBegaran();

            List<HushallBegaranArenden> arenden = new List<HushallBegaranArenden>();

            foreach (var invoice in invoices)
            {
                var itemsByType = invoice.Items
                    .Where(i => i.ProductType == ProductType.Service && i.DomesticService is not null)
                    .GroupBy(x => x.DomesticService!.Kind);

                if (itemsByType.Any())
                {
                    var arende = new RotRut.Begaran.Rut.HushallBegaranArenden();
                    arende.FakturaNr = invoice.Id.ToString();
                    arende.BetalningsDatum = DateTime.Now;
                    arende.Kopare = "Test";

                    arende.UtfortArbete = new RotRut.Begaran.Rut.HushallBegaranArendenUtfortArbete();

                    foreach (var itemGroup in itemsByType)
                    {
                        var item = itemGroup.First();

                        /*
                        switch(item.DomesticService!.Hushallsarbete) 
                        {
                            case Domain.Entities.HouseholdServiceType.Cleaning:
                                arende.UtfortArbete.Stadning = new RotRut.Begaran.Rut.HushallBegaranArendenUtfortArbeteStadning
                                {
                                    AntalTimmar = item.DomesticService!.Hours,
                                    Materialkostnad = item.DomesticService.MaterialCost,
                                };
                                break;
                        }
                        */

                        /*
                        arende.PrisForArbete = (int)Math.Round(invoice.Total - item.DomesticService.MaterialCost - item.DomesticService.OtherCosts);
                        arende.OvrigKostnad = (int)Math.Round(itemGroup.Sum(item => item.DomesticService.OtherCosts));
                        arende.BetaltBelopp = (int)Math.Round(invoice.Paid.GetValueOrDefault());
                        arende.BegartBelopp = (int)Math.Round(arende.PrisForArbete * 0.5);
                        */
                        //arenden.Add(arende);
                    }

                }
            }

            begaranFil.HushallBegaran.Arenden = arenden.ToArray();

            using var ms = new MemoryStream();
            RotRutBegaran.Serialize(ms, begaranFil);

            return Encoding.UTF8.GetString(ms.GetBuffer());
        }
    }

    //public record Foo(Hushallsarbete Hushallsarbete, double AntalTimmar, decimal Materialkostnad, );
}