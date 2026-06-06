
using System.Numerics;
using Microsoft.VisualBasic;
using static Utility;

public partial class DDMatrix
{
    /// <summary>
    /// A helper class to organize DDNodes, mainly to make reducing DDNodes easier.
    /// </summary>
    public class DDNodeList
    {
        // DDNodes are divided by their height in the tree with 0 being the lowest layer with the DDNode.one node
        private readonly Dictionary<int, List<DDNode>> data = new(); 

        /// <summary>
        /// Returns all DDNodes in one Enumerator.
        /// </summary>
        public IEnumerable<DDNode> GetAll()
        {
            var list = new List<DDNode>();
            foreach (var (_, n) in data)
            {
                list.AddRange(n);
            }
            return list;
        }

        /// <summary>
        /// Returns all DDNodes in the given layer. 
        /// <br/><br/>
        /// Note that this won't work for layer 0 (DDNode.one) and
        /// layer -1 (DDNode.zero), as those are shared between DDMatrixes and thus not considered part of
        /// this DDMatrix. This is to avoid bugs, where DDNode.one and DDNode.zero are changed accidentally in
        /// one matrix, which would then affect other matrices.
        /// </summary>
        public List<DDNode> GetLayer(int layer)
        {
            return data[layer];
        }

        /// <summary>
        /// Returns the node with the given parameters or returns null if it hasn't been created yet.
        /// <br/><br/>
        /// Note that the weights must be normalized.
        /// </summary>
        public DDNode? GetOrNull(int layer, Complex[] weights, DDNode[] next)
        {
            var list = data[layer];
            if (layer <= 0) // zero or one
            {
                return null;
            }
            foreach (var t in list)
            {
                bool isNotEqual = false;
                for (int i = 0; i<4;i++)
                {
                    if (!CompareComplex(t.weights[i], weights[i]) || t.next[i] != next[i])
                    {
                        isNotEqual = true;
                        break;
                    }
                }
                if (isNotEqual)
                {
                    continue;
                } else // the two nodes are equal
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the node that has the same parameters as the given node or returns null 
        /// if it hasn't been created yet.
        /// <br/><br/>
        /// Note that the node must be normalized.
        /// </summary>
        public DDNode? GetOrNull(DDNode node)
        {
            return GetOrNull(node.layer, node.weights, node.next);
        }

        /// <summary>
        /// Returns the node with the given parameters if it is in the list. Otherwise creates said node and
        /// adds it to the list (and returns the newly created node).
        /// <br/><br/>
        /// If DDNode.zero or DDNode.one are given, they are returned without being put into the list.
        /// <br/><br/>
        /// Note that the weights must be normalized.
        /// </summary>
        public DDNode GetOrCreate(int layer, Complex[] weights, DDNode[] next)
        {
            if (layer == 0) {return DDNode.one;}
            if (layer == -1) {return DDNode.zero;}
            var t = GetOrNull(layer, weights, next);
            if (t != null)
            {
                return t;
            }
            t = new DDNode(this, layer, weights, next);
            if (!data.ContainsKey(layer))
            {
                data.Add(layer, [t]);
            } else
            {
                data[layer].Add(t);
            }
            return t;
        }

        /// <summary>
        /// Returns the node with the same parameters if it is in the list. Otherwise copies said node and
        /// adds it to the list (and returns the copy).
        /// <br/><br/>
        /// Note that the node must be normalized.
        /// </summary>
        public DDNode GetOrCreate(DDNode node)
        {
            return GetOrCreate(node.layer, node.weights, node.next);
        }

        public int Count()
        {
            int c = 0;
            foreach (var (a,b) in data)
            {
                c += data.Count;
            }
            return c;
        }
    }
}