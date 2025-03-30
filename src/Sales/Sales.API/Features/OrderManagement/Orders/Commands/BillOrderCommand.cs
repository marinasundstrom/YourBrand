using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Client;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Commands;

public record BillOrderCommand(string OrganizationId, string OrderId) : IRequest<Result<BillOrderResponse>>
{
    public class Handler(IOrderRepository orderRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IInvoicesClient invoicesClient) : IRequestHandler<BillOrderCommand, Result<BillOrderResponse>>
    {
        public async Task<Result<BillOrderResponse>> Handle(BillOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                            .GetAll()
                            .InOrganization(request.OrganizationId)
                            .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            List<CreateInvoiceItem> items = new List<CreateInvoiceItem>();

            foreach (var item in order.Items)
            {
                var options = item.Options.Select(x => new CreateInvoiceItemOption
                {
                    Description = x.Description,
                    Price = x.Price,
                    ItemId = x.ItemId,
                    Name = x.Name,
                    ProductId = x.ProductId,
                    Value = x.Value
                });

                items.Add(
                    new CreateInvoiceItem
                    {
                        ProductType = (ProductType)item.ProductType,
                        ProductId = item.ProductId,
                        Description = item.Description,
                        UnitPrice = item.Price,
                        Unit = item.Unit ?? "",
                        VatRate = item.VatRate.GetValueOrDefault(),
                        Quantity = item.Quantity,
                        Options = options.ToList(),
                        Discounts = []
                    });
            }

            var invoice = await invoicesClient.CreateInvoiceAsync(new CreateInvoice()
            {
                OrganizationId = request.OrganizationId,
                Date = DateTime.Now,
                Customer = order.Customer is not null ? new Invoicing.Client.SetCustomer
                {
                    Id = order.Customer.Id,
                    Name = order.Customer.Name
                } : null,
                Items = items
            });

            return Result.SuccessWith(new BillOrderResponse(invoice.Id, invoice.InvoiceNo));
        }
    }
}

public record BillOrderResponse(string InvoiceId, int? InvoiceNo);