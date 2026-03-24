using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.MasterData
{
    public class Product : FullAuditedAggregateRoot<Guid>
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Unit { get; private set; }
        public string AuxUnit { get; private set; }
        public decimal ConversionRate { get; private set; }
        public bool IsBatchManagementEnabled { get; private set; }
        public int? ShelfLifeDays { get; private set; }

        protected Product() { }

        public Product(
            Guid id,
            string code,
            string name,
            string unit,
            string auxUnit,
            decimal conversionRate,
            bool isBatchManagementEnabled,
            int? shelfLifeDays) : base(id)
        {
            SetCode(code);
            SetName(name);
            SetUnit(unit);
            SetAuxUnit(auxUnit);
            SetConversionRate(conversionRate);
            SetBatchManagement(isBatchManagementEnabled, shelfLifeDays);
        }

        public void SetCode(string code)
        {
            Code = Check.NotNullOrWhiteSpace(code, nameof(code));
        }

        public void SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        }

        public void SetUnit(string unit)
        {
            Unit = unit;
        }

        public void SetAuxUnit(string auxUnit)
        {
            AuxUnit = auxUnit;
        }

        public void SetConversionRate(decimal conversionRate)
        {
            ConversionRate = conversionRate;
        }

        public void SetBatchManagement(bool enabled, int? shelfLifeDays)
        {
            IsBatchManagementEnabled = enabled;
            ShelfLifeDays = enabled ? shelfLifeDays : null;
        }
    }
}

