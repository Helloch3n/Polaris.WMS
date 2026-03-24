using System.ComponentModel.DataAnnotations;
using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.MasterData.Domain.Locations
{
    public class Location : FullAuditedAggregateRoot<Guid>
    {
        public string Code { get; private set; }
        public string Aisle { get; private set; }
        public string Rack { get; private set; }
        public string Level { get; private set; }
        public string Bin { get; private set; }
        public Guid WarehouseId { get; private set; }

        public decimal MaxWeight { get; private set; }
        public decimal MaxVolume { get; private set; }
        public int MaxReelCount { get; private set; }

        [ConcurrencyCheck]
        public LocationStatus Status { get; private set; }
        public LocationType Type { get; private set; }

        /// <summary>
        /// 是否允许混放不同物料。
        /// </summary>
        public bool AllowMixedProducts { get; private set; }

        /// <summary>
        /// 是否允许混放不同批次。
        /// </summary>
        public bool AllowMixedBatches { get; private set; }

        public Guid ZoneId { get; private set; }

        protected Location()
        {
        }

        internal Location(
            Guid id,
            Guid warehouseId,
            Guid zoneId,
            string code,
            string aisle,
            string rack,
            string level,
            string bin,
            decimal maxWeight,
            decimal maxVolume,
            int maxReelCount,
            LocationType type = LocationType.Rack,
            bool allowMixedProducts = true,
            bool allowMixedBatches = true) : base(id)
        {
            WarehouseId = warehouseId;
            ZoneId = zoneId;
            Code = code;
            Aisle = aisle;
            Rack = rack;
            Level = level;
            Bin = bin;
            MaxWeight = maxWeight;
            MaxVolume = maxVolume;
            MaxReelCount = maxReelCount;
            Type = type;
            Status = LocationStatus.Idle;
            AllowMixedProducts = allowMixedProducts;
            AllowMixedBatches = allowMixedBatches;
        }

        /// <summary>
        /// 更新库位基础信息（不包含归属关系 WarehouseId/ZoneId 和状态 Status）。
        /// </summary>
        public void UpdateBasicInfo(
            string code,
            string aisle,
            string rack,
            string level,
            string bin,
            decimal maxWeight,
            decimal maxVolume,
            int maxReelCount,
            LocationType type,
            bool allowMixedProducts,
            bool allowMixedBatches)
        {
            Code = code;
            Aisle = aisle;
            Rack = rack;
            Level = level;
            Bin = bin;
            MaxWeight = maxWeight;
            MaxVolume = maxVolume;
            MaxReelCount = maxReelCount;
            Type = type;
            AllowMixedProducts = allowMixedProducts;
            AllowMixedBatches = allowMixedBatches;
        }

        public void SetCoordinates(string aisle, string rack, string level, string bin)
        {
            Aisle = aisle;
            Rack = rack;
            Level = level;
            Bin = bin;
        }

        public void SetConstraints(decimal maxWeight, decimal maxVolume, int maxReelCount)
        {
            MaxWeight = maxWeight;
            MaxVolume = maxVolume;
            MaxReelCount = maxReelCount;
        }

        public void SetMixRules(bool allowMixedProducts, bool allowMixedBatches)
        {
            AllowMixedProducts = allowMixedProducts;
            AllowMixedBatches = allowMixedBatches;
        }

        public void ChangeStatus(LocationStatus newStatus)
        {
            Status = newStatus;
        }
    }
}

