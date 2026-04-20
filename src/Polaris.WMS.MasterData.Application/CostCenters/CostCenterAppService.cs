using Polaris.WMS.MasterData.Application.Contracts.CostCenters;
using Polaris.WMS.MasterData.Application.Contracts.CostCenters.Dtos;
using Polaris.WMS.MasterData.Domain.CostCenters;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.CostCenters;

public class CostCenterAppService(
    IRepository<CostCenter, Guid> costCenterRepository,
    CostCenterManager costCenterManager)
    : ApplicationService, ICostCenterAppService
{
    /// <inheritdoc />
    public async Task<CostCenterDto> CreateAsync(CreateCostCenterDto input)
    {
        var entity = await costCenterManager.CreateAsync(
            input.Code,
            input.Name,
            input.DepartmentCode,
            input.DepartmentName,
            input.CompanyCode);

        return ObjectMapper.Map<CostCenter, CostCenterDto>(entity);
    }

    /// <inheritdoc />
    public async Task<CostCenterDto> GetAsync(Guid id)
    {
        var entity = await costCenterRepository.GetAsync(id);
        return ObjectMapper.Map<CostCenter, CostCenterDto>(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<CostCenterDto>> GetListAsync(CostCenterSearchDto input)
    {
        var query = await costCenterRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Code))
        {
            query = query.Where(x => x.Code.Contains(input.Code));
        }

        if (!string.IsNullOrWhiteSpace(input.Name))
        {
            query = query.Where(x => x.Name.Contains(input.Name));
        }

        if (!string.IsNullOrWhiteSpace(input.DepartmentCode))
        {
            query = query.Where(x => x.DepartmentCode.Contains(input.DepartmentCode));
        }

        if (!string.IsNullOrWhiteSpace(input.DepartmentName))
        {
            query = query.Where(x => x.DepartmentName.Contains(input.DepartmentName));
        }

        if (!string.IsNullOrWhiteSpace(input.CompanyCode))
        {
            query = query.Where(x => x.CompanyCode.Contains(input.CompanyCode));
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query
            .OrderBy(x => x.Code)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var entities = await AsyncExecuter.ToListAsync(query);
        var items = entities.Select(ObjectMapper.Map<CostCenter, CostCenterDto>).ToList();

        return new PagedResultDto<CostCenterDto>(totalCount, items);
    }
}

