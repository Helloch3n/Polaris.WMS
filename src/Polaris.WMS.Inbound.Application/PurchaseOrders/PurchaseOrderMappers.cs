using Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders.Dtos;
using Polaris.WMS.Inbound.Domain.PurchaseOrders;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.Inbound.Application.PurchaseOrders;

[Mapper]
public partial class PurchaseOrderMappers : MapperBase<PurchaseOrder, PurchaseOrderDto>
{
    public override partial PurchaseOrderDto Map(PurchaseOrder source);

    public override partial void Map(PurchaseOrder source, PurchaseOrderDto destination);
}