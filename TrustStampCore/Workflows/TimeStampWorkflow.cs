using NBitcoin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;
using TrustStampCore.Service;

namespace TrustStampCore.Workflows
{
    public class TimeStampWorkflow : WorkflowBatch
    {
        public const string Name = "BTC-Timestamp";

        public override string StateName
        {
            get
            {
                return Name;
            }
        }

        static TimeStampWorkflow()
        {
            WorkflowEngine.WorkflowTypes.Add(Name, typeof(TimeStampWorkflow));
        }

        public override void SetState()
        {
            CurrentBatch["state"] = new JObject(
                new JProperty("state", StateName),
                new JProperty("retry", 0)
                );
        }

        public override void Execute()
        {
            using (var db = TimeStampDatabase.Open())
            {
                WriteLog("Stated", db);

                if (TimeStampBatch(db))
                    Push(SuccessWorkflow.Name);
                else
                    CurrentBatch["state"]["retry"] = ((int)CurrentBatch["state"]["retry"]) + 1;

                if(CurrentBatch["state"].Contains("retry") && (int)CurrentBatch["state"]["retry"] == 3)
                    Push(FailedWorkflow.Name);

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

            if(TimeStampDatabase.IsMemoryDatabase)
            {
                var tx = (JArray)CurrentBatch["blockchain"];
                tx.Add(new JObject(
                    new JProperty("type", "btc-testnet"),
                    new JProperty("tx", "No transaction (Demo)")
                    ));

                WriteLog("Success", db);
                return true;
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
