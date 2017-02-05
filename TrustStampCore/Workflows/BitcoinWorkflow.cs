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
            using (var db = TimeStampDatabase.Open())
            {
                WriteLog("Stated", db);

                if (TimeStampBatch(db))
                {
                    Push(new SuccessWorkflow());
                }
                else
                {
                    CurrentBatch["state"]["retry"] = CurrentBatch["state"]["retry"].ToInteger() + 1;

                    if (CurrentBatch["state"]["retry"].ToInteger() == 3)
                        Push(new FailedWorkflow());
                    else
                        Push(new SleepWorkflow(DateTime.Now.AddHours(2), Name));
                }

                db.BatchTable.Update(CurrentBatch);
            }
        }

        private bool TimeStampBatch(TimeStampDatabase db)
        {
            Transaction previousTx = null;
            var btc = new BitcoinManager();

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

            if(btc.NoKey)
            {
                WriteLog("No wif key - no timestamp!", db);
                return false;
            }

            var result = btc.Send(hash, previousTx);
            if (result.status == "success")
            {
                // Not working
                //previousTx = result.Tx; // Save the current tx for later use in the next spent (support for unconfirmed spending!)
                var tx = (JArray)CurrentBatch["blockchain"];
                tx.Add(new JObject(
                    new JProperty("type", "btc-testnet"),
                    new JProperty("tx", result.data)
                    ));

                WriteLog("Success", db);
                return true;
            }
            else
            {
                WriteLog("Failed: " + result.status, db);
                return false;
            }
        }
    }
}
