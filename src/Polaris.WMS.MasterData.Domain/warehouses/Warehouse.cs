using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.MasterData.Domain.warehouses
{
    public class Warehouse : FullAuditedAggregateRoot<Guid>
    {
        public string Code { get; private set; }
        public string Name { get; private set; }

        protected Warehouse()
        {
        }

        public Warehouse(
            Guid id,
            string code,
            string name) : base(id)
        {
            Code = code;
            Name = name;
        }

        public void Update(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}

