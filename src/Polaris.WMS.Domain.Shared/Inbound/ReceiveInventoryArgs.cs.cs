using System;
using System.Collections.Generic;
using System.Text;

namespace Polaris.WMS.Inbound
{
    public class ReceiveInventoryArgs
    {
        public Guid ReceiptId { get; }
        public Guid DetailId { get; }
        public decimal ActualQuantity { get; }
        public decimal ActualWeight { get; }
        public Guid LocationId { get; }
        public string? SN { get; }
        public decimal Weight { get; }

        public ReceiveInventoryArgs(
            Guid receiptId,
            Guid detailId,
            decimal actualQuantity,
            decimal actualWeight,
            Guid locationId,
            string? sn = null,
            decimal weight = 0)
        {
            ReceiptId = receiptId;
            DetailId = detailId;
            ActualQuantity = actualQuantity;
            ActualWeight = actualWeight;
            LocationId = locationId;
            SN = sn;
            Weight = weight;
        }
    }
}

