﻿using NBitcoin;
using Newtonsoft.Json.Linq;
using System;
using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

namespace TrustStampCore.Workflows
{
    public class BitcoinWorkflow : WorkflowBatch
    {
        private JObject _bitcoinState = null;
        public JObject BitcoinState {
            get
            {
                return _bitcoinState ?? (_bitcoinState = CurrentBatch["state"].EnsureObject("bitcoin"));
            }
        }

        private JValue _retry = null;
        public JValue Retry {
            get
            {
                return _retry ?? (_retry = BitcoinState.EnsureProperty("retry", 0));
            }
        }

        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;
            return true;
        }

        public override void Execute()
        {
            var wif = App.Config["btcwif"].ToStringValue();
            if (string.IsNullOrEmpty(wif)) // No WIF key, then try to stamp remotely
            {
                Push(new RemoteStampWorkflow());
                return;
            }

            var blockchainRepositoryName = App.Config["blockchainprovider"].ToStringValue("blockr");
            var blockchainRepository = BlockchainFactory.GetRepository(blockchainRepositoryName, BlockchainFactory.GetBitcoinNetwork());
            if(blockchainRepository == null)
            {
                WriteLog("No blockchain provider found"); // No comment!
                return;
            }

            if (TimeStampBatch(wif, blockchainRepository, BlockchainFactory.GetBitcoinNetwork()))
            {
                Push(new SuccessWorkflow());
            }
            else
            {
                Retry.Value = (int)Retry + 1;

                if ((int)Retry >= 3)
                    Push(new FailedWorkflow("Failed 3 times creating a blockchain Transaction."));
                else
                    Push(new SleepWorkflow(DateTime.Now.AddHours(3), Name)); // Sleep to 3 hours and retry this workflow
            }

            Update();
        }

        public bool TimeStampBatch(string wif, IBlockchainRepository repository, Network network)
        {
            var btc = new BitcoinManager(wif, repository, network);

            var hash = (byte[])CurrentBatch["root"];
            if (hash.Length == 0)
            {
                WriteLog("No root to timestamp!");
                return true;
            }

            if(App.Config["test"].ToBoolean())
            {
                var tx = (JArray)CurrentBatch["blockchain"];
                tx.Add(new JObject(
                    new JProperty("type", "btc-testnet"),
                    new JProperty("tx", "No transaction (Demo)")
                    ));

                WriteLog("Success");
                return true;
            }

            try
            {
                var txs = btc.Send(hash);

                var blockchainNode = (JArray)CurrentBatch["blockchain"];
                blockchainNode.Add(new JObject(
                    new JProperty("type", "btc-testnet"),
                    new JProperty("sourcetx", txs.Item1.ToHex()),
                    new JProperty("batchtx", txs.Item2.ToHex())
                    ));

                WriteLog("Success");
                return true;

            }
            catch (Exception ex)
            {
                WriteLog("Failed: " + ex.Message);
                return false;
            }
        }
    }
}
