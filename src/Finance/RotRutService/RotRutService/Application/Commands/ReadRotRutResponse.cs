using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Client;
using YourBrand.Domain;
using YourBrand.RotRutService.Domain;
using YourBrand.RotRutService.Domain.Entities;
using YourBrand.Transactions.Client;

namespace YourBrand.RotRutService.Application.Commands;

public record ReadRotRutResponse(string OrganizationId, RotRut.Beslut.BeslutFil BeslutJson) : IRequest
{
    public class Handler(IRotRutContext context, IJournalEntriesClient journalEntriesClient, ITransactionsClient transactionsClient) : IRequestHandler<ReadRotRutResponse>
    {
        public async Task Handle(ReadRotRutResponse request, CancellationToken cancellationToken)
        {
            foreach (var beslut in request.BeslutJson.Beslut)
            {
                foreach (var arende in beslut.Arenden)
                {
                    var rotRutCase = await context.RotRutCases
                        .Where(x => x.Status == RotRutCaseStatus.RequestSent)
                        .FirstOrDefaultAsync(x => x.InvoiceNo == arende.Fakturanummer, cancellationToken);

                    if (rotRutCase is null)
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

                    var verificationId2 = await journalEntriesClient.CreateJournalEntryAsync(request.OrganizationId, new CreateJournalEntry
                    {
                        Description = $"Utbetalning RUT/RUT",
                        InvoiceNo = rotRutCase.InvoiceNo,
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

            await context.SaveChangesAsync();


            //await _transactionsClient.SetTransactionStatusAsync(transaction.Id, Transactions.Client.TransactionStatus.Verified);
        }
    }
}