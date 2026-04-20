using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.MasterData.Domain.CostCenters;

public class CostCenterManager(ICostCenterRepository costCenterRepository) : DomainService
{
    public async Task<CostCenter> CreateAsync(
        string code,
        string name,
        string departmentCode,
        string departmentName,
        string companyCode)
    {
        var existing = await costCenterRepository.GetByCodeAsync(code);
        if (existing != null)
        {
            throw new BusinessException("WMS:CostCenterCodeAlreadyExists")
                .WithData("Code", code);
        }

        var costCenter = new CostCenter(
            GuidGenerator.Create(),
            code,
            name,
            departmentCode,
            departmentName,
            companyCode);

        return await costCenterRepository.InsertAsync(costCenter);
    }
}

