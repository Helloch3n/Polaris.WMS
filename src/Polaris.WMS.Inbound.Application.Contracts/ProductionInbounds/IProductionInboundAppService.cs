using Polaris.WMS.Inbound.Application.Contracts.ProductionInbounds.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Inbound.Application.Contracts.ProductionInbounds
{
    public interface IProductionInboundAppService : IApplicationService
    {
        /// <summary>
        /// 创建入库单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<ProductionInboundDto> CreateAsync(CreateProductionInboundDto input);

        /// <summary>
        /// 审核入库单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task ApproveAndExecuteAsync(Guid id);

        /// <summary>
        /// 获取入库单信息
        /// </summary>
        /// <returns></returns>
        public Task<ProductionInboundDto> GetAsync(Guid orderId);

        /// <summary>
        /// 删除入库单及明细
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Task DeleteAsync(Guid orderId);

        /// <summary>
        /// 获取分页清单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PagedResultDto<ProductionInboundDto>> GetListAsync(GetProductionInboundListDto input);

        /// <summary>
        /// 更新入库单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<ProductionInboundDto> UpdateAsync(ProductionInboundDto input);

    }
}
