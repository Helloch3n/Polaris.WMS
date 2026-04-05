using Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts.Dtos;
using Polaris.WMS.Inbound.Domain.PurchaseReceipts;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.Inbound.Application.PurchaseReceipts;

[Mapper]
public partial class PurchaseReceiptMappers : MapperBase<PurchaseReceipt, PurchaseReceiptDto>
{
    public override partial PurchaseReceiptDto Map(PurchaseReceipt source);

    public override partial void Map(PurchaseReceipt source, PurchaseReceiptDto destination);
}

