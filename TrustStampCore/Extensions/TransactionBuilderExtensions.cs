using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Extensions
{
    public static class TransactionBuilderExtensions
    {
        public static TransactionBuilder SendOP_Return(this TransactionBuilder tb, byte[] data)
        {
            var message = Encoding.UTF8.GetBytes("trust").Concat(data).ToArray();
            tb.Send(TxNullDataTemplate.Instance.GenerateScriptPubKey(message), Money.Zero);
            return tb;
        }


    }
}
