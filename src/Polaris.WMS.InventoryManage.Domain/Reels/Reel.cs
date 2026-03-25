using Polaris.WMS.MasterData.Reels;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.InventoryManage.Domain.Reels
{
    public class Reel : FullAuditedAggregateRoot<Guid>
    {
        public string ReelNo { get; private set; }
        public string Name { get; private set; }
        public string Size { get; private set; }
        public decimal SelfWeight { get; private set; }
        public ReelStatus Status { get; private set; }
        public bool IsLocked { get; private set; }
        public Guid? CurrentLocationId { get; private set; }
        public ReelType ReelType { get; private set; }

        protected Reel()
        {
        }

        internal Reel(
            Guid id,
            string reelNo,
            string name,
            string size,
            decimal selfWeight,
            ReelStatus status,
            Guid? currentLocationId,
            ReelType reelType) : base(id)
        {
            ReelNo = reelNo;
            Name = name;
            Size = size;
            SelfWeight = selfWeight;
            Status = status;
            CurrentLocationId = currentLocationId;
            IsLocked = false;
            reelType = reelType;
        }

        public void SetLocation(Guid? locationId)
        {
            CurrentLocationId = locationId;
        }

        public void SetOccupied()
        {
            Status = ReelStatus.Occupied;
        }

        public void SetEmpty()
        {
            Status = ReelStatus.Empty;
        }

        public void Lock(string reason)
        {
            if (IsLocked) return;
            IsLocked = true;
        }

        public void UnLock()
        {
            IsLocked = false;
        }

        public void Update(
            string reelNo,
            string name,
            string size,
            decimal selfWeight)
        {
            ReelNo = reelNo;
            Name = name;
            Size = size;
            SelfWeight = selfWeight;
        }
    }
}