using YourBrand.RotRutService.Domain;
using YourBrand.RotRutService.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Accounting.Client;
using YourBrand.Transactions.Client;
using YourBrand.RotRutService.Domain.Entities;
using Unit = MediatR.Unit;
using RotRut;

namespace YourBrand.RotRutService.Application.Commands;

public record ReadRotRutResponse(RotRut.Beslut.BeslutFil BeslutJson) : IRequest
{
    public class Handler : IRequestHandler<ReadRotRutResponse>
    {
        private readonly IRotRutContext _context;
        private readonly IVerificationsClient _verificationsClient;
        private ITransactionsClient _transactionsClient;

        public Handler(IRotRutContext context, IVerificationsClient verificationsClient, ITransactionsClient transactionsClient)
        {
            _context = context;
            _verificationsClient = verificationsClient;
            _transactionsClient = transactionsClient;
        }

        public async Task Handle(ReadRotRutResponse request, CancellationToken cancellationToken)
        {
            foreach(var beslut in request.BeslutJson.Beslut) 
            {
                foreach(var arende in beslut.Arenden) 
                {
                    var rotRutCase = await _context.RotRutCases
                        .Where(x => x.Status ==  RotRutCaseStatus.RequestSent)
                        .FirstOrDefaultAsync(x => x.InvoiceId == arende.Fakturanummer, cancellationToken);

                    if(rotRutCase is null) 
                    {
                        continue;
                    }

                    var entries2 = new List<CreateEntry>
                    {
                        new CreateEntry
                        {
                            AccountNo = 1930,
                            Description = string.Empty,
                            Credit = rotRutCase.PaidAmount
                        }
                    };

                    entries2.Insert(0, new CreateEntry
                    {
                        AccountNo = 1513,
                        Description = "ROT-avdrag",
                        Credit = rotRutCase.RequestedAmount
                    });

                    var verificationId2 = await _verificationsClient.CreateVerificationAsync(new CreateVerification
                    {
                        Description = $"Utbetalning RUT/RUT",
                        InvoiceId = rotRutCase.InvoiceId,
                        Entries = entries2
                    }, cancellationToken);
                    
                    rotRutCase.ReceivedAmount = Convert.ToDecimal(arende.GodkantBelopp);
                    rotRutCase.Status = RotRutCaseStatus.RequestConfirmed;

                    if (rotRutCase.RequestedAmount > rotRutCase.RequestedAmount) 
                    {
                        // Kräv pengat tillbaka!
                    }
                }
            }
            
            await _context.SaveChangesAsync();


            //await _transactionsClient.SetTransactionStatusAsync(transaction.Id, Transactions.Client.TransactionStatus.Verified);
        }
    }
}