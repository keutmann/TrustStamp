﻿using System;
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
            var leafNodes = new List<MerkleNode>();
            for(var i = 0; i < 100; i++)
            {
                var hash = MerkleTree.HashStrategy(Encoding.Unicode.GetBytes(i.ToString()));
                var proof = new JObject(new JProperty("hash", hash));
                Console.WriteLine("Hash: {0}", ((byte[])proof["hash"]).ConvertToHex());
                leafNodes.Add(new MerkleNode(proof));
            }

            // Build
            var merkleTree = new MerkleTree(leafNodes);
            var rootNode = merkleTree.Build();

            Console.WriteLine("Root node: " + rootNode.Hash.ConvertToHex());

            foreach (var entity in leafNodes)
            {
                var calcRoot = MerkleTree.ComputeRoot(entity.Hash, entity.Path, MerkleTree.HashBytelength);
                Console.WriteLine("Entity "+entity.Hash.ConvertToHex() + " : "+entity.Path.ConvertToHex());
                Assert.AreEqual(rootNode.Hash, calcRoot);
            }
        }
    }
}
