using System;
using System.Collections.Generic;

namespace YourBrand.Orders.Contracts
{
    public class CreateCustomFieldDetails
    {
        public string CustomFieldId { get; set; } = null!;

        public string Value { get; set; } = null!;
    }

    public class AddCustomFieldToOrderCommand
    {
        public AddCustomFieldToOrderCommand(int orderNo, CreateCustomFieldDetails customFieldDetails)
        {
            OrderNo = orderNo;
            CreateCustomFieldDetails = customFieldDetails;
        }

        public int OrderNo { get; }

        public CreateCustomFieldDetails CreateCustomFieldDetails { get; }
    }

    public class AddCustomFieldToOrderCommandResponse
    {

    }

    public class RemoveCustomFieldFromOrderCommand
    {
        public RemoveCustomFieldFromOrderCommand(
            int orderNo,
            string customFieldId
        )
        {
            OrderNo = orderNo;
            CustomFieldId = customFieldId;
        }

        public int OrderNo { get; }

        public string CustomFieldId { get; }
    }

    public class RemoveCustomFieldFromOrderCommandResponse
    {

    }

    public class AddCustomFieldToOrderItemCommand
    {
        public AddCustomFieldToOrderItemCommand(int orderNo, Guid orderItemId, CreateCustomFieldDetails customFieldDetails)
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
            CreateCustomFieldDetails = customFieldDetails;
        }

        public int OrderNo { get; }

        public Guid OrderItemId { get; }

        public CreateCustomFieldDetails CreateCustomFieldDetails { get; }
    }

    public class AddCustomFieldToOrderItemCommandResponse
    {

    }

    public class RemoveCustomFieldFromOrderItemCommand
    {
        public RemoveCustomFieldFromOrderItemCommand(
            int orderNo,
            Guid orderItemId,
            string customFieldId
        )
        {
            OrderNo = orderNo;
            OrderItemId = orderItemId;
            CustomFieldId = customFieldId;
        }

        public int OrderNo { get; }

        public Guid OrderItemId { get; }

        public string CustomFieldId { get; }
    }

    public class RemoveCustomFieldFromOrderItemCommandResponse
    {

    }
}