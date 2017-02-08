using NBitcoin;
using System;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

namespace TrustStampCore.Repository
{
    public class BlockchainFactory
    {
        public static IBlockchainRepository GetRepository(string name, Network network)
        {
            if (String.IsNullOrEmpty(name))
                throw new ApplicationException("Name cannot be null or empty");

            switch(name.ToLower())
            {
                case "blockr": return new BlockrRepository(network); 
            }
            return null;
        }

        public static Network GetBitcoinNetwork()
        {
            if (Network.Main.Name.Equals(App.Config["network"].ToStringValue(""), StringComparison.OrdinalIgnoreCase))
                return Network.Main;
            else
                return
                    Network.TestNet;
        }
    }
}
