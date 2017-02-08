using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;

namespace TrustStampCore.Repository
{
    public interface IBlockchainRepository
    {
        Task BroadcastAsync(Transaction tx);
        Task<Transaction> GetAsync(uint256 txId);
        Task<List<Coin>> GetUnspentAsync(string Address);
        FeeRate GetEstimatedFee();
    }
}