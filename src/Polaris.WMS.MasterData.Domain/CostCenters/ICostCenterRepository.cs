using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Domain.CostCenters;

public interface ICostCenterRepository : IRepository<CostCenter, Guid>
{
    Task<CostCenter?> GetByCodeAsync(string code);
}

