using Polaris.WMS.Inventories.Containers;
using Polaris.WMS.MasterData.Containers;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.InventoryManage.Domain.Containers
{
    public class Container : FullAuditedAggregateRoot<Guid>
    {
        public string ContainerCode { get; private set; }
        public string Name { get; private set; }
        public string Size { get; private set; }
        public decimal SelfWeight { get; private set; }
        public ContainerStatus Status { get; private set; }
        public bool IsLocked { get; private set; }
        public Guid? CurrentLocationId { get; private set; }
        public ContainerType ContainerType { get; private set; }

        protected Container()
        {
        }

        internal Container(
            Guid id,
            string containerCode,
            string name,
            string size,
            decimal selfWeight,
            ContainerStatus status,
            Guid? currentLocationId,
            ContainerType containerType) : base(id)
        {
            ContainerCode = containerCode;
            Name = name;
            Size = size;
            SelfWeight = selfWeight;
            Status = status;
            CurrentLocationId = currentLocationId;
            IsLocked = false;
            ContainerType = containerType;
        }

        public void SetLocation(Guid? locationId)
        {
            CurrentLocationId = locationId;
        }

        public void SetOccupied()
        {
            Status = ContainerStatus.Occupied;
        }

        public void SetEmpty()
        {
            Status = ContainerStatus.Empty;
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
            string containerCode,
            string name,
            string size,
            decimal selfWeight)
        {
            ContainerCode = containerCode;
            Name = name;
            Size = size;
            SelfWeight = selfWeight;
        }
    }
}