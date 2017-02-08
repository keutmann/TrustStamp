using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json.Linq;

namespace TrustStampCore.Repository
{
    public interface IBlockchainRepository
    {
        Task BroadcastAsync(Transaction tx);
        Task<Transaction> GetTransactionAsync(uint256 txId);
        JObject GetAddressInfo(string address);
        Task<List<Coin>> GetUnspentAsync(string Address);
        FeeRate GetEstimatedFee();
    }
}