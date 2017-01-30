using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace TrustStampCore.Service
{
    public class BitcoinManager
    {
        public Network CurrentNetwork = Network.TestNet;

        private Key key32 = null;
        private BitcoinPubKeyAddress adr32 = null;


        public BitcoinManager()
        {
            var crypt = SHA256.Create();

            key32 = new Key(crypt.ComputeHash(Encoding.Unicode.GetBytes("Carsten Keutmann")), 32, true);
            adr32 = key32.PubKey.GetAddress(CurrentNetwork);
        }


        public BlockrTxPutResult Send(byte[] hash, Transaction sourceTx = null)
        {
            var keyPoolHash = new Key(hash, 32, true);

            var blockr = new BlockrTransactionRepository();
            blockr.Network = CurrentNetwork;
            IEnumerable<Coin> coins;
            if (sourceTx != null)
                coins = sourceTx.Outputs.AsCoins(); // Load unspent coins from previous tx, made by this application.
            else {
                // Load from main source
                var unspent = blockr.GetUnspentAsync(key32.PubKey.GetAddress(CurrentNetwork).ToString());
                unspent.Wait();
                coins = unspent.Result;
            }

            //urceTx.Outputs.

            var txBuilder = new TransactionBuilder();
            var tx = txBuilder
                .AddCoins(coins)
                .AddKeys(key32)
                .Send(keyPoolHash.PubKey.GetAddress(CurrentNetwork), "0.0002")
                .SendFees("0.0001")
                .SetChange(adr32)
                .BuildTransaction(true);

            Console.WriteLine("Verify: " + txBuilder.Verify(tx));

            Console.WriteLine("Sending Tx!");

            var txPoolBuilder = new TransactionBuilder();

            // Build Pool Nota
            var txNota = txPoolBuilder
                .AddCoins(tx.Outputs.AsCoins())
                .AddKeys(keyPoolHash)
                .Send(adr32, "0.0001")
                .SendFees("0.0001")
                .BuildTransaction(false);

            var message = Encoding.UTF8.GetBytes("NOTA5").Concat(hash).ToArray();
            txNota.Outputs.Add(new TxOut()
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(message) // Message
            });
            txNota = txPoolBuilder.SignTransaction(txNota);

            Console.WriteLine("Verify Tx Nota: " + txPoolBuilder.Verify(txNota));
            Console.WriteLine(string.Format("Address Root {0}", keyPoolHash.PubKey.GetAddress(CurrentNetwork)));

            var txResult = BlockrPut(tx, CurrentNetwork);
            if (txResult.code != 200)
                return txResult;

            Console.WriteLine("Tx Spend Code: " + txResult.code);

            var txPoolResult = BlockrPut(txNota, CurrentNetwork);
            Console.WriteLine(string.Format("Pool Result Tx:{0}", txPoolResult.data));

            txPoolResult.Tx = tx;
            
            return txPoolResult;
        }

        public static string CyperPost(string hexString)
        {
            return Post("https://api.blockcypher.com/v1/btc/test3/txs/push", "tx", hexString);
        }


        public static BlockrTxPutResult BlockrPut(Transaction tx, Network network)
        {
            var url = (network == Network.Main) ? "http://btc.blockr.io/api/v1/tx/push" : "http://tbtc.blockr.io/api/v1/tx/push";
            var result = Post(url, "hex", tx.ToHex());
            var txResult = JsonConvert.DeserializeObject<BlockrTxPutResult>(result);
            return txResult;
        }


        public static string Post(string RequestURL, string Post1, string hexString)
        {
            using (var wb = new WebClient())
            {
                wb.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var data = "{\"" + Post1 + "\":\"" + hexString + "\"}";
                return wb.UploadString(RequestURL, "POST", data);
            }
        }

        //public static string GetTx(string tx)
        //{
        //    var requestURL = "";
        //    using (var wb = new WebClient())
        //    {
        //        wb.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        //        //var data = "{\"" + Post1 + "\":\"" + hexString + "\"}";
        //        return wb.DownloadString();
        //    }
        //}

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }

    /// <summary>
    /// Result
    /// { "status":"success","data":"abcdb99533088999a25c32a4bea91593d2234c546c0caa02af25380ea4e7b6fc","code":200,"message":""}
    /// </summary>
    public class BlockrTxPutResult
    {
        public string status { get; set; }
        public string data { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public Transaction Tx { get; set; }
    }


}
