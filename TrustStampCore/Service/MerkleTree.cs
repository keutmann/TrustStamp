using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TrustStampCore.Models;
using TrustStampCore.Extensions;

namespace TrustStampCore.Service
{
    public class MerkleTree //: IDisposable
    {
        public static HashAlgorithm InnerAlgorithm = SHA256.Create();
        public static HashAlgorithm OuterAlgorithm = RIPEMD160.Create();
        public static Func<byte[],byte[]> HashStrategy = (i) => OuterAlgorithm.ComputeHash(InnerAlgorithm.ComputeHash(i));

        public MerkleTree()
        {
        }

        public TreeEntity Build(List<TreeEntity> leafNodes)
        {
            var rootNode = BuildTree(leafNodes);
            ComputeMerkleTree(rootNode);

            // Update the path back to proof entities
            foreach (var node in leafNodes)
            {
                node.Entity["path"] = node.MerkleTree.ToHex();
            }
            return rootNode;
        }

        public TreeEntity BuildTree(List<TreeEntity> leafNodes)
        {
            var nodes = new Queue<TreeEntity>(leafNodes);
            while (nodes.Count > 1)
            {
                var parents = new Queue<TreeEntity>();
                while (nodes.Count > 0)
                {
                    var first = nodes.Dequeue();
                    var second = (nodes.Count == 0) ? first : nodes.Dequeue();

                    if (first.Hash.Compare(second.Hash) > 0)
                    {
                        var hash = HashStrategy(first.Hash.Combine(second.Hash));
                        parents.Enqueue(new TreeEntity(hash, first, second));
                    }
                    else
                    {
                        var hash = HashStrategy(second.Hash.Combine(first.Hash));
                        parents.Enqueue(new TreeEntity(hash, second, first));
                    }
                }
                nodes = parents;
            }
            return nodes.FirstOrDefault(); // root
        }

        public void ComputeMerkleTree(TreeEntity root)
        {
            var merkle = new Stack<byte[]>();
            ComputeMerkleTree(root, merkle);
        }

        public void ComputeMerkleTree(TreeEntity node, Stack<byte[]> merkle)
        {
            if (node == null)
                return;

            if (node.Left == null && node.Right == null)
            {
                var tree = new List<byte>();
                foreach (var v in merkle)
                    tree.AddRange(v);

                node.MerkleTree = tree.ToArray();
            }

            if (node.Left != null)
            {
                merkle.Push(node.Right.Hash);
                ComputeMerkleTree(node.Left, merkle);
            }

            if (node.Right != null)
            {
                merkle.Push(node.Left.Hash);
                ComputeMerkleTree(node.Right, merkle);
            }

            if (merkle.Count > 0)
                merkle.Pop();

            return;
        }

        public static byte[] ComputeRoot(byte[] hash, byte[] path, int hashLength)
        {
            for (var i = 0; i < path.Length; i += hashLength)
            {
                var merkle = new byte[hashLength];
                Array.Copy(path, i, merkle, 0, hashLength);
                if (hash.Compare(merkle) > 0)
                    hash = HashStrategy(hash.Combine(merkle));
                else
                    hash = HashStrategy(merkle.Combine(hash));
            }
            return hash;
        }
    }
}
