using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopUpAPI.Models;

namespace TelecomProviderAPI.Core.IRepository
{
    public interface IMobileRechargeRepository
    {
        Task<IEnumerable<TopUpOption>> GetTopUpOptions();
        Task<bool> TopUpBeneficiary(int userId, int beneficiaryId, decimal amount);
    }
}
