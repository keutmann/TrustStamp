using NBitcoin;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

namespace TrustStampCore.Repository
{
    public class BlockrRepository : IBlockchainRepository
    {
        public Network Network { get; set; }
        public BlockrTransactionRepository Blockr { get; set; }

        public BlockrRepository(Network network)
        {
            Network = network;
            Blockr = new BlockrTransactionRepository(network);
        }

        public Task BroadcastAsync(Transaction tx)
        {
            return Blockr.BroadcastAsync(tx);
        }
        public Task<Transaction> GetAsync(uint256 txId)
        {
            return Blockr.GetAsync(txId);
        }
        
        public Task<List<Coin>> GetUnspentAsync(string Address)
        {
            return Blockr.GetUnspentAsync(Address);
        }

        public FeeRate GetEstimatedFee()
        {
            return new FeeRate(App.Config["fee"].ToStringValue("0.0001"));
        }
    }
}
