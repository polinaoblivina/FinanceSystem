using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Enums
{
    public enum ActionType
    {
        Create,
        Delete,
        Close,
        Block,
        Unblock,
        Transfer,
        Approve,
        Reject,
        ReceiveSalary,
        AccrueInterest,
        Revert
    }
}
