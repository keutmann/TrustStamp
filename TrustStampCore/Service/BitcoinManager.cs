﻿using NBitcoin;
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
    public class BitcoinManager
    {
        public string WIF { get; }
        public Key SourceKey { get; }
        public BitcoinPubKeyAddress SourceAddress { get; }
        public bool NoKey { get; }
        public IBlockchainRepository Repository { get; }
        public Network Network { get; }

        public BitcoinManager(string wif, IBlockchainRepository repository, Network network)
        {
            Network = network;
            WIF = wif;
            var secret = new BitcoinSecret(WIF);
            SourceKey = secret.PrivateKey;
            SourceAddress = SourceKey.PubKey.GetAddress(Network);
            Repository = repository;
            Network = network;
        }

        public Tuple<Transaction, Transaction> Send(byte[] batchHash)
        {
            Key batchKey = GetKey(batchHash);

            var unspent = Repository.GetUnspentAsync(SourceKey.PubKey.GetAddress(Network).ToWif());
            unspent.Wait();
            IEnumerable<Coin> coins = unspent.Result;
            if (coins.Count() == 0)
                throw new ApplicationException("No coins to spend");

            var fee = Repository.GetEstimatedFee().FeePerK;
            var totalAmount = coins.Sum(c => c.Amount.Satoshi);
            if (fee.Satoshi * 2 > totalAmount)
                throw new ApplicationException("Not enough coin to spend.");


            var sourceTx = new TransactionBuilder()
                .AddCoins(coins)
                .AddKeys(SourceKey)
                .Send(batchKey.PubKey.GetAddress(Network), fee) // Send to Batch address
                .SendFees(fee)
                .SetChange(SourceAddress)
                .BuildTransaction(true);

            Repository.BroadcastAsync(sourceTx);

            var txNota = new TransactionBuilder()
                .AddCoins(sourceTx.Outputs.AsCoins())
                .SendOP_Return(batchHash) // Put batch root on the OP_Return out tx
                .AddKeys(batchKey)
                .SendFees(fee)
                .BuildTransaction(true);
            
            Repository.BroadcastAsync(txNota);
           
            return Tuple.Create(sourceTx, txNota);
        }

        public static Key GetKey(byte[] data)
        {
            Key key = (data.Length == 32) ? new Key(data, data.Length, true) :
                new Key(Hashes.SHA256(data), 32, true);

            return key;
        }

    }
}
