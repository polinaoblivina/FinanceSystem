using FInanceSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Interfaces
{
    public interface IUnitOfWork 
    {
        IAccountRepository Accounts { get; }
        ILogRepository Logs { get; }
        IDepositRepository Deposits { get; }
        ITransactionRepository Transactions { get; }
        IBankRepository Banks { get; }
        IUserRepository Users { get; }
        IEnterpriseRepository Enterprises { get; }
        IEmploymentRepository Employments { get; }
        ISalaryProjectRequestRepository SalaryRequests { get; }

        Task SaveChangesAsync();
    }
}
