using FinanceSystem.Infrastructure.Repositories;
using FInanceSystem.Domain.Repositories;
using FInanceSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FinanceDbContext _dbContext;
        private IAccountRepository? _accounts;
        private ILogRepository? _logs;
        private IDepositRepository? _deposits;
        private ITransactionRepository? _transactions;
        private IBankRepository? _banks;
        private IUserRepository? _users;
        private IEnterpriseRepository? _enterprises;
        private IEmploymentRepository? _employments;
        private ISalaryProjectRequestRepository? _salaryRequests;
        public UnitOfWork(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IAccountRepository Accounts
        {
            get
            {
                if(_accounts == null)
                    _accounts = new AccountRepository(_dbContext);

                return _accounts;
            }
        }
        public ILogRepository Logs
        {
            get
            {
                if (_logs == null)
                    _logs = new LogRepository(_dbContext);

                return _logs;
            }
        }

        public IDepositRepository Deposits
        {
            get
            {
                if (_deposits == null)
                    _deposits = new DepositRepository(_dbContext);

                return _deposits;
            }
        }

        public ITransactionRepository Transactions
        {
            get
            {
                if (_transactions == null)
                    _transactions = new TransactionRepository(_dbContext);

                return _transactions;
            }
        }

        public IBankRepository Banks
        {
            get
            {
                if (_banks == null)
                    _banks = new BankRepository(_dbContext);

                return _banks;
            }
        }

        public IUserRepository Users
        {
            get
            {
                if (_users == null)
                    _users = new UserRepository(_dbContext);

                return _users;
            }
        }
        public IEnterpriseRepository Enterprises
        {
            get
            {
                if (_enterprises == null)
                    _enterprises = new EnterpriseRepository(_dbContext);

                return _enterprises;
            }
        }
        public IEmploymentRepository Employments
        {
            get
            {
                if (_employments == null)
                    _employments = new EmploymentRepository(_dbContext);

                return _employments;
            }
        }
        public ISalaryProjectRequestRepository SalaryRequests
        {
            get
            {
                if(_salaryRequests  == null)
                    _salaryRequests = new SalaryProjectRequestRepository(_dbContext);
                return _salaryRequests;
            }
        }
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
