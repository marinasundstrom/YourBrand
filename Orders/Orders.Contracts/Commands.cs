using System;

namespace YourBrand.Orders.Contracts
{
    public class CreateOrderCommand
    {
        public CreateOrderCommand()
        {
        }

        public CreateOrderCommand(CreateOrderDto? dto)
        {
            Dto = dto;
        }

        public CreateOrderDto? Dto { get; set; }
    }

    public class CreateOrderCommandResponse
    {
        public CreateOrderCommandResponse()
        {
        }

        public CreateOrderCommandResponse(int orderNo)
        {
            OrderNo = orderNo;
        }

        public int OrderNo { get; set; }
    }

    public class AddOrderItemCommand
    {
        public AddOrderItemCommand(int orderNo, string? description, string? itemId, string? unit, double quantity)
        {
            OrderNo = orderNo;
            Description = description;
            ItemId = itemId;
            Unit = unit;
            Quantity = quantity;
        }

        public int OrderNo { get; }

        public string? Description { get; }

        public string? ItemId { get; }

        public string? Unit { get; }

        public double Quantity { get; }
    }

    public class AddOrderItemCommandResponse
    {
        public AddOrderItemCommandResponse()
        {
        }

        public AddOrderItemCommandResponse(int orderNo, Guid orderItemId)
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
        }

        public int OrderNo { get; set; }

        public Guid OrderItemId { get; set; }
    }

    public class ClearOrderCommand
    {
        public ClearOrderCommand(int orderNo)
        {
            OrderNo = orderNo;
        }

        public int OrderNo { get; }
    }

    public class ClearOrderCommandResponse
    {
        public ClearOrderCommandResponse()
        {
        }

        public ClearOrderCommandResponse(int orderNo)
        {
            OrderNo = orderNo;
        }

        public int OrderNo { get; set; }
    }

    public class RemoveOrderItemCommand
    {
        public RemoveOrderItemCommand(int orderNo, Guid orderItemId)
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
        }

        public int OrderNo { get; }

        public Guid OrderItemId { get; }
    }

    public class RemoveOrderItemCommandResponse
    {
        public RemoveOrderItemCommandResponse()
        {
        }

        public RemoveOrderItemCommandResponse(int orderNo, Guid orderItemId)
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
        }

        public int OrderNo { get; set; }

        public Guid OrderItemId { get; set; }
    }

    public class UpdateOrderItemQuantityCommand
    {
        public UpdateOrderItemQuantityCommand(int orderNo, Guid orderItemId, double quantity)
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
            Quantity = quantity;
        }

        public int OrderNo { get; }

        public Guid OrderItemId { get; }

        public double Quantity { get; }
    }

    public class UpdateOrderItemQuantityCommandResponse
    {
        public UpdateOrderItemQuantityCommandResponse()
        {
        }

        public UpdateOrderItemQuantityCommandResponse(int orderNo, Guid orderItemId)
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
        }

        public int OrderNo { get; set; }

        public Guid OrderItemId { get; set; }
    }

    public class PlaceOrderCommand
    {
        public PlaceOrderCommand(int orderNo)
        {
            OrderNo = orderNo;
        }

        public int OrderNo { get; }
    }

    public class PlaceOrderCommandResponse
    {

    }

    public class UpdateOrderStatusCommand
    {
        public UpdateOrderStatusCommand(int orderNo, string orderStatusId)
        {
            OrderNo = orderNo;
            OrderStatusId = orderStatusId;
        }

        public int OrderNo { get; }

        public string OrderStatusId { get; }
    }

    public class UpdateOrderStatusCommandResponse
    {

    }

    public class DiscountDetails
    {

        public decimal? Amount { get; set; }

        public double? Percent { get; set; }

        public decimal? Total { get; set; }

        public string? Description { get; set; }

        public Guid? DiscountId { get; set; }
    }

    public class AddDiscountToOrderCommand
    {
        public AddDiscountToOrderCommand(int orderNo, DiscountDetails discountDetails)
        {
            OrderNo = orderNo;
            DiscountDetails = discountDetails;
        }

        public int OrderNo { get; }

        public DiscountDetails DiscountDetails { get; }
    }

    public class AddDiscountToOrderCommandResponse
    {

    }

    public class RemoveDiscountFromOrderCommand
    {
        public RemoveDiscountFromOrderCommand(
            int orderNo,
            Guid discountId
        )
        {
            OrderNo = orderNo;
            DiscountId = discountId;
        }

        public int OrderNo { get; }

        public Guid DiscountId { get; }
    }

    public class RemoveDiscountFromOrderCommandResponse
    {

    }

    public class AddDiscountToOrderItemCommand
    {
        public AddDiscountToOrderItemCommand(int orderNo, Guid orderItemId, DiscountDetails discountDetails)
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
            DiscountDetails = discountDetails;
        }

        public int OrderNo { get; }

        public Guid OrderItemId { get; }

        public DiscountDetails DiscountDetails { get; }
    }

    public class AddDiscountToOrderItemCommandResponse
    {

    }

    public class RemoveDiscountFromOrderItemCommand
    {
        public RemoveDiscountFromOrderItemCommand(
            int orderNo,
            Guid orderItemId,
            Guid discountId
        )
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
            DiscountId = discountId;
        }

        public int OrderNo { get; }

        public Guid OrderItemId { get; }

        public Guid DiscountId { get; }
    }

    public class RemoveDiscountFromOrderItemCommandResponse
    {

    }

    public class ChargeDetails
    {

        public decimal? Amount { get; set; }

        public double? Percent { get; set; }

        public decimal? Total { get; set; }

        public string? Description { get; set; }

        public Guid? ChargeId { get; set; }
    }

    public class AddChargeToOrderCommand
    {
        public AddChargeToOrderCommand(int orderNo, ChargeDetails chargeDetails)
        {
            OrderNo = orderNo;
            ChargeDetails = chargeDetails;
        }

        public int OrderNo { get; }

        public ChargeDetails ChargeDetails { get; }
    }

    public class AddChargeToOrderCommandResponse
    {

    }

    public class RemoveChargeFromOrderCommand
    {
        public RemoveChargeFromOrderCommand(
            int orderNo,
            Guid chargeId
        )
        {
            OrderNo = orderNo;
            ChargeId = chargeId;
        }

        public int OrderNo { get; }

        public Guid ChargeId { get; }
    }

    public class RemoveChargeFromOrderCommandResponse
    {

    }

    public class AddChargeToOrderItemCommand
    {
        public AddChargeToOrderItemCommand(int orderNo, Guid orderItemId, ChargeDetails chargeDetails)
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
            ChargeDetails = chargeDetails;
        }

        public int OrderNo { get; }

        public Guid OrderItemId { get; }

        public ChargeDetails ChargeDetails { get; }
    }

    public class AddChargeToOrderItemCommandResponse
    {

    }

    public class RemoveChargeFromOrderItemCommand
    {
        public RemoveChargeFromOrderItemCommand(
            int orderNo,
            Guid orderItemId,
            Guid chargeId
        )
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
            ChargeId = chargeId;
        }

        public int OrderNo { get; }

        public Guid OrderItemId { get; }

        public Guid ChargeId { get; }
    }

    public class RemoveChargeFromOrderItemCommandResponse
    {

    }
}