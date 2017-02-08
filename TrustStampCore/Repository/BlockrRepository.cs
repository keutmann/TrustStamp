using NBitcoin;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrustStampCore.Service;
using TrustStampCore.Extensions;
using Newtonsoft.Json.Linq;

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
        public Task<Transaction> GetTransactionAsync(uint256 txId)
        {
            return Blockr.GetAsync(txId);
        }

        public JObject GetAddressInfo(string address)
        {
          
            return null;
            //return Blockr.GetAsync(txId);
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
