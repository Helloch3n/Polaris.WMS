using Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts;

public interface IPurchaseReceiptAppService : IApplicationService
{
    /// <summary>
    /// 创建采购收货单。
    /// </summary>
    Task<PurchaseReceiptDto> CreateAsync(CreatePurchaseReceiptDto input);

    /// <summary>
    /// 获取采购收货单详情。
    /// </summary>
    Task<PurchaseReceiptDto> GetAsync(Guid id);

    /// <summary>
    /// 分页查询采购收货单。
    /// </summary>
    Task<PagedResultDto<PurchaseReceiptDto>> GetListAsync(PurchaseReceiptSearchDto input);

    /// <summary>
    /// 更新采购收货明细 ERP 同步状态。
    /// </summary>
    Task ChangeDetailErpSyncStatusAsync(ChangePurchaseReceiptDetailErpSyncStatusDto input);
}

