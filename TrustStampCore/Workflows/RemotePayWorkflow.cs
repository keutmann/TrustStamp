﻿using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;
using System;
using Newtonsoft.Json.Linq;

namespace TrustStampCore.Workflows
{
    public class RemotePayWorkflow : BlockchainWorkflowBatch
    {
        public JProperty Retry { get; set; }

        public override bool Initialize()
        {
            base.Initialize();
            if (!EnsureDependencies())
                return false;

            var remotepay = CurrentBatch["state"]["remotepay"].EnsureObject();
            Retry = remotepay.EnsureProperty("retry", 0);
            return true;
        }

        public override void Execute()
        {
            WriteLog("Waiting for payment on Batch root");

            var rootKey = BitcoinManager.GetKey(Root);
            var rootAddress = rootKey.PubKey.GetAddress(BlockchainFactory.GetBitcoinNetwork());
            var info = BlockchainRepository.GetAddressInfo(rootAddress.ToWif());


            if(info != null && info["totalreceived"] != null || info["totalreceived"].ToInteger() > 0)
            {
                // payment has been made!
                WriteLog("Payment has been made on Batch root");
                Push(new SuccessWorkflow());
                return;
            }

            // Wait some time to see if someone pays for the Batch root!
            Retry.Value = Retry.Value.ToInteger() + 1;
            if (Retry.Value.ToInteger() >= 3)
                Push(new FailedWorkflow("Failed 3 times waiting for payment on Root."));
            else
                Push(new SleepWorkflow(DateTime.Now.AddHours(3), Name)); // Sleep and retry this workflow

            Update();
        }

        private bool VerifyEndpoint(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                return false;

            return true;
        }

    }
}
