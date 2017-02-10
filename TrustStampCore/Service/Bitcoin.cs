using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.Policy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using TrustStampCore.Extensions;
using TrustStampCore.Repository;

namespace TrustStampCore.Service
{
    public class Bitcoin
    {
        public string WIF { get; }
        public Key SourceKey { get; }
        public BitcoinPubKeyAddress SourceAddress { get; }
        public bool NoKey { get; }
        public IBlockchainRepository Blockchain { get; }
        public Network Network { get; }

        public Bitcoin(string wif, IBlockchainRepository blockchain, Network network)
        {
            Network = network;
            WIF = wif;
            var secret = new BitcoinSecret(WIF);
            SourceKey = secret.PrivateKey;
            SourceAddress = SourceKey.PubKey.GetAddress(Network);
            Blockchain = blockchain;
            Network = network;
        }

        public Tuple<Transaction, Transaction> Send(byte[] batchHash, Transaction previousTx)
        {
            Key batchKey = GetKey(batchHash);
            
            var fee = Blockchain.GetEstimatedFee().FeePerK;

            var coins = GetCoins(previousTx, fee);

            var sourceTx = new TransactionBuilder()
                .AddCoins(coins)
                .AddKeys(SourceKey)
                .Send(batchKey.PubKey.GetAddress(Network), fee) // Send to Batch address
                .SendFees(fee)
                .SetChange(SourceAddress)
                .BuildTransaction(true);

            Blockchain.BroadcastAsync(sourceTx);

            var txNota = new TransactionBuilder()
                .AddCoins(sourceTx.Outputs.AsCoins())
                .SendOP_Return(batchHash) // Put batch root on the OP_Return out tx
                .AddKeys(batchKey)
                .SendFees(fee)
                .BuildTransaction(true);

            Blockchain.BroadcastAsync(txNota);

            return Tuple.Create(sourceTx, txNota);
        }

        public IEnumerable<Coin> GetCoins(Transaction previousTx, Money fee)
        {
            IEnumerable<Coin> coins = null;
            long sumOfCoins = 0;
            if (previousTx != null)
            {
                coins = previousTx.Outputs.AsCoins().Where(c => c.ScriptPubKey.GetDestinationAddress(Network) == SourceAddress);
                sumOfCoins = coins.Sum(c => c.Amount.Satoshi);
            }

            if (fee.Satoshi * 2 > sumOfCoins)
            {
                var unspent = Blockchain.GetUnspentAsync(SourceKey.PubKey.GetAddress(Network).ToWif());
                unspent.Wait();
                coins = unspent.Result;
                if (coins.Count() == 0)
                    throw new ApplicationException("No coins to spend");

                sumOfCoins = coins.Sum(c => c.Amount.Satoshi);
                if (fee.Satoshi * 2 > sumOfCoins)
                    throw new ApplicationException("Not enough coin to spend.");
            }

            return coins;
        }

        public static Key GetKey(byte[] data)
        {
            Key key = (data.Length == 32) ? new Key(data, data.Length, true) :
                new Key(Hashes.SHA256(data), 32, true);

            return key;
        }

    }
}
