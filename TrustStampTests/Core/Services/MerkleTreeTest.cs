using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TrustStampCore.Extensions;
using TrustStampCore.Models;
using TrustStampCore.Service;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace TrustStampTests.Core.Services
{
    [TestFixture]
    public class MerkleTreeTest
    {
        [Test]
        public void TestRoot()
        {
            // Setup variables
            var leafNodes = new List<TreeEntity>();
            for(var i = 0; i < 11111; i++)
            {
                var hash = MerkleTree.HashStrategy(Encoding.Unicode.GetBytes(i.ToString())).ToHex();
                var proof = new JObject(new JProperty("hash", hash));
                Console.WriteLine("Hash: {0}", proof["hash"]);
                leafNodes.Add(new TreeEntity(proof));
            }

            // Build
            var merkleTree = new MerkleTree();
            var rootNode = merkleTree.Build(leafNodes);

            Console.WriteLine("Root node: " + rootNode.Hash.ToHex());

            foreach (var entity in leafNodes)
            {
                var calcRoot = MerkleTree.ComputeRoot(entity.Hash, entity.MerkleTree, MerkleTree.OuterAlgorithm.HashSize / 8);
                Console.WriteLine("Entity "+entity.Hash.ToHex() + " : "+entity.MerkleTree.ToHex());
                Assert.AreEqual(rootNode.Hash, calcRoot);
            }
        }
    }
}
