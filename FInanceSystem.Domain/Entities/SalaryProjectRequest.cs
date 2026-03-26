using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public class SalaryProjectRequest
    {
        public Guid Id { get; private set; }
        public string? Number { get; private set; }
        public Guid ClientId { get; private set; }
        public Guid EnterpriseId { get; private set; }
        public RequestStatus Status { get; private set; }

        private SalaryProjectRequest() { }
        private SalaryProjectRequest(Guid id, string number, Guid clientId, Guid enterpriseId, RequestStatus status)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid application id", nameof(id));
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Invalid account number", nameof(number));
            if (clientId == Guid.Empty)
                throw new ArgumentException("Invalid client id", nameof(clientId));
            if (enterpriseId == Guid.Empty)
                throw new ArgumentException("Invalid enterprise id", nameof(enterpriseId));

            Id = id;
            Number = number;
            ClientId = clientId;
            EnterpriseId = enterpriseId;
            Status = status;
        }
        public static SalaryProjectRequest CreateNew(Guid clientId, Guid enterpriseId)
        {
            return new SalaryProjectRequest(Guid.NewGuid(), FInanceSystem.Domain.Utils.Utils.GenerateRequestNumber(), clientId, enterpriseId, RequestStatus.Pending);
        }

        public static SalaryProjectRequest Restore(Guid id, string number, Guid clientId, Guid enterpriseId, RequestStatus status)
        {
            return new SalaryProjectRequest(id, number, clientId, enterpriseId, status);
        }
        public void Approve()
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Only pending applications can be approved");
            Status = RequestStatus.Accepted;
        }

        public void Reject()
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Only pending applications can be rejected");

            Status = RequestStatus.Rejected;
        }
    }
}
