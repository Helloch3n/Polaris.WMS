using Polaris.WMS.InventoryManage.Application.Contracts.Reels.Dtos;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.InventoryManage.Application.Reels;

[Mapper]
public partial class ReelMappers : MapperBase<Reel, ReelDto>
{
    [MapProperty(nameof(Reel.IsLocked), nameof(ReelDto.IsLocked))]
    public override partial ReelDto Map(Reel source);

    public override partial void Map(Reel source, ReelDto destination);

    public partial void Map(CreateUpdateReelDto source, Reel destination);
}
