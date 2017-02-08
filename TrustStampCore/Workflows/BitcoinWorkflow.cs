using NBitcoin;
using Newtonsoft.Json.Linq;
using System;
using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

namespace TrustStampCore.Workflows
{
    public class BitcoinWorkflow : WorkflowBatch
    {
        public override void SetState()
        {
            CurrentBatch["state"] = new JObject(
                new JProperty("state", Name),
                new JProperty("retry", 0)
                );
        }

        public override void Execute()
        {
            var wif = App.Config["btcwif"].ToStringValue();
            if (string.IsNullOrEmpty(wif)) // No WIF key, then try to stamp remotely
            {
                Push(new RemoteStampWorkflow());
                return;
            }

            using (var db = TrustStampDatabase.Open())
            {
                var blockchainRepositoryName = App.Config["blockchainprovider"].ToStringValue("blockr");
                var blockchainRepository = BlockchainFactory.GetRepository(blockchainRepositoryName, BlockchainFactory.GetBitcoinNetwork());
                if(blockchainRepository == null)
                {
                    WriteLog("No blockchain provider found", db); // No comment!
                    return;
                }

                if (TimeStampBatch(db, wif, blockchainRepository, BlockchainFactory.GetBitcoinNetwork()))
                {
                    Push(new SuccessWorkflow());
                }
                else
                {
                    CurrentBatch["state"]["retry"] = CurrentBatch["state"]["retry"].ToInteger() + 1;

                    if (CurrentBatch["state"]["retry"].ToInteger() == 3)
                        Push(new FailedWorkflow("Failed 3 times creating a blockchain Transaction."));
                    else
                        Push(new SleepWorkflow(DateTime.Now.AddHours(2), Name)); // Sleep to 2 hours and retry this workflow
                }

                db.BatchTable.Update(CurrentBatch);
            }
        }

        public bool TimeStampBatch(TrustStampDatabase db, string wif, IBlockchainRepository repository, Network network)
        {
            var btc = new BitcoinManager(wif, repository, network);

            var hash = (byte[])CurrentBatch["root"];
            if (hash.Length == 0)
            {
                WriteLog("No root to timestamp!", db);
                return true;
            }

            if(App.Config["test"].ToBoolean())
            {
                var tx = (JArray)CurrentBatch["blockchain"];
                tx.Add(new JObject(
                    new JProperty("type", "btc-testnet"),
                    new JProperty("tx", "No transaction (Demo)")
                    ));

                WriteLog("Success", db);
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

                WriteLog("Success", db);
                return true;

            }
            catch (Exception ex)
            {
                WriteLog("Failed: " + ex.Message, db);
                return false;
            }
        }
    }
}
