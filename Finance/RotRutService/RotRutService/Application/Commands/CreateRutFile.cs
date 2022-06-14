using YourBrand.RotRutService.Domain;
using YourBrand.RotRutService.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;
using RotRut.Begaran;
using RotRut.Begaran.Rut;
using RotRut;
using System.Text;

namespace YourBrand.RotRutService.Application.Commands;

public record CreateRutFile(string? NamnPaBegaran) : IRequest<string>
{
    public class Handler : IRequestHandler<CreateRutFile, string>
    {
        private readonly IRotRutContext _context;

        public Handler(IRotRutContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateRutFile request, CancellationToken cancellationToken)
        {
            var rotRutCases = await _context.RotRutCases
                .Where(x => x.Status == Domain.Entities.RotRutCaseStatus.InvoicePaid)
                .ToArrayAsync(cancellationToken);

            BegaranFil begaranFil = new BegaranFil()
            {
                NamnPaBegaran = request.NamnPaBegaran ?? "dsfsd"
            };

            begaranFil.HushallBegaran = new RotRut.Begaran.Rut.HushallBegaran();

            List<HushallBegaranArenden> arenden = new List<HushallBegaranArenden>();

            foreach (var rotRutCase in rotRutCases)
            {
                var arende = new RotRut.Begaran.Rut.HushallBegaranArenden()
                {
                    FakturaNr = rotRutCase.InvoiceId.ToString(),
                    BetalningsDatum = rotRutCase.PaymentDate,
                    Kopare = rotRutCase.Buyer,
                    PrisForArbete = (int)Math.Round(rotRutCase.LaborCost),
                    BetaltBelopp = (int)Math.Round(rotRutCase.PaidAmount),
                    BegartBelopp = (int)Math.Round(rotRutCase.RequestedAmount),
                    OvrigKostnad = (int)Math.Round(rotRutCase.OtherCosts),
                };

                arende.UtfortArbete = new RotRut.Begaran.Rut.HushallBegaranArendenUtfortArbete();

                if (rotRutCase.Kind == DomesticServiceKind.HouseholdService)
                {
                    switch (rotRutCase.Rut!.ServiceType)
                    {
                        case HouseholdServiceType.Cleaning:
                            arende.UtfortArbete.Stadning = new RotRut.Begaran.Rut.HushallBegaranArendenUtfortArbeteStadning
                            {
                                AntalTimmar = rotRutCase.Hours,
                                Materialkostnad = rotRutCase.MaterialCost,
                            };
                            break;
                    }
                }

                arenden.Add(arende);
            }

            begaranFil.HushallBegaran.Arenden = arenden.ToArray();

            using var ms = new MemoryStream();
            RotRutBegaran.Serialize(ms, begaranFil);

            return Encoding.UTF8.GetString(ms.GetBuffer());
        }
    }
}