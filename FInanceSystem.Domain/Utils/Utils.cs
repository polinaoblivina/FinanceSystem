using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Utils
{
    public static class Utils
    {
        private static readonly Random _random = new Random();

        public static string GenerateAccountNumber()
        {
            return _random.Next(100000, 999999).ToString();
        }

        public static string GenerateDepositNumber()
        {
            return _random.Next(100000, 999999).ToString();
        }

        public static string GenerateTransactionNumber()
        {
            return _random.Next(10000000, 99999999).ToString();
        }

        public static string GenerateRequestNumber()
        {
            return _random.Next(10000000, 99999999).ToString();
        }

        public static string GenerateLogNumber()
        {
            return _random.Next(10000000, 99999999).ToString();
        }
    }
}
