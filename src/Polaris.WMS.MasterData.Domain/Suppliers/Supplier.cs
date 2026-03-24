using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.MasterData
{
    public class Supplier : FullAuditedAggregateRoot<Guid>
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string ContactPerson { get; private set; }
        public string Mobile { get; private set; }
        public string Email { get; private set; }
        public string Address { get; private set; }

        protected Supplier() { }

        public Supplier(
            Guid id,
            string code,
            string name,
            string contactPerson,
            string mobile,
            string email,
            string address) : base(id)
        {
            SetCode(code);
            SetName(name);
            ContactPerson = contactPerson;
            Mobile = mobile;
            Email = email;
            Address = address;
        }

        public void SetCode(string code)
        {
            Code = Check.NotNullOrWhiteSpace(code, nameof(code));
        }

        public void SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        }

        public void UpdateContact(string contactPerson, string mobile, string email, string address)
        {
            ContactPerson = contactPerson;
            Mobile = mobile;
            Email = email;
            Address = address;
        }
    }
}

