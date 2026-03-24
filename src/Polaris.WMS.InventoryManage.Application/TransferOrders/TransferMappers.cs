using Polaris.WMS.InventoryManage.Application.Contracts.TransferOrders.Dtos;
using Polaris.WMS.InventoryManage.Domain.TransferOrders;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.InventoryManage.Application.TransferOrders
{
    /// <summary>
    /// 调拨单对象映射。
    /// </summary>
    [Mapper]
    public partial class TransferMappers : MapperBase<TransferOrder, TransferDto>
    {
        public override partial TransferDto Map(TransferOrder source);

        public override partial void Map(TransferOrder source, TransferDto destination);

        private partial TransferDetailDto MapDetail(TransferOrderDetail source);
    }
}
