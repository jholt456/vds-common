/*
VDS.Common is licensed under the MIT License

Copyright (c) 2009-2012 Rob Vesse

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace VDS.Common.Tries
{
    /// <summary>
    /// Node of a Trie
    /// </summary>
    /// <typeparam name="TKeyBit">Key Bit Type</typeparam>
    /// <typeparam name="TValue">Value Type</typeparam>
    /// <remarks>
    /// <para>
    /// Original code taken from <a href="http://code.google.com/p/typocalypse/source/browse/#hg/Trie">Typocolypse</a> but has been heavily rewritten to be much more generic and LINQ friendly
    /// </para>
    /// </remarks>
    public class TrieNode<TKeyBit, TValue>
        where TValue : class
    {
        private Dictionary<TKeyBit, TrieNode<TKeyBit, TValue>> _children;

        /// <summary>
        /// Create an empty node with no children and null value
        /// </summary>
        /// <param name="parent">Parent node of this node</param>
        /// <param name="key">Key Bit</param>
        public TrieNode(TrieNode<TKeyBit, TValue> parent, TKeyBit key)
        {
            this.KeyBit = key;
            this.Value = null;
            this.Parent = parent;
            this._children = new Dictionary<TKeyBit, TrieNode<TKeyBit, TValue>>();
        }

        /// <summary>
        /// Gets/Sets value stored by this node
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Gets the key bit that is associated with this node
        /// </summary>
        public TKeyBit KeyBit { get; private set; }

        /// <summary>
        /// Get the parent of this node
        /// </summary>
        public TrieNode<TKeyBit, TValue> Parent { get; private set; }

        /// <summary>
        /// Get a child of this node which is associated with a key bit
        /// </summary>
        /// <param name="key">Key associated with the child of interest</param>
        /// <returns>The child or null if no child is associated with the given key</returns>
        internal TrieNode<TKeyBit, TValue> GetChild(TKeyBit key)
        {
            TrieNode<TKeyBit, TValue> child;
            lock (this._children)
            {
                if (this._children.TryGetValue(key, out child)) return child;
            }
            return null;
        }

        /// <summary>
        /// Tries to get a child of this node which is associated with a key bit
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="child">Child</param>
        /// <returns></returns>
        internal bool TryGetChild(TKeyBit key, out TrieNode<TKeyBit, TValue> child)
        {
            lock (this._children)
            {
                return this._children.TryGetValue(key, out child);
            }
        }

        /// <summary>
        /// Check whether this Node is the Root Node
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return this.Parent == null;
            }
        }

        /// <summary>
        /// Check whether or not this node has a value associated with it
        /// </summary>
        public bool HasValue
        {
            get
            {
                return this.Value != null;
            }
        }

        /// <summary>
        /// Gets the number of immediate child nodes this node has
        /// </summary>
        public int Count
        {
            get
            {
                lock (this._children)
                {
                    return this._children.Count;
                }
            }
        }

        /// <summary>
        /// Gets the number of descendant child nodes this node has
        /// </summary>
        public int CountAll
        {
            get
            {
                return this.AllChildren.Count();
            }
        }

        /// <summary>
        /// Check whether or not this node has any children.
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                lock (this._children)
                {
                    return this._children.Count == 0;
                }
            }
        }

        /// <summary>
        /// Check whether or not one of the children of this node uses the given key bit
        /// </summary>
        /// <param name="key">The key bit to check for</param>
        /// <returns>True if a child with given key exists, false otherwise</returns>
        internal bool ContainsKey(TKeyBit key)
        {
            lock (this._children)
            {
                return this._children.ContainsKey(key);
            }
        }

        /// <summary>
        /// Clears the Value (if any) stored at this node
        /// </summary>
        public void ClearValue()
        {
            this.Value = null;
        }

        /// <summary>
        /// Clears the Value (if any) stored at this node and removes all child nodes
        /// </summary>
        public void Clear()
        {
            this.ClearValue();
            this.Trim();
        }

        /// <summary>
        /// Removes all child nodes
        /// </summary>
        public void Trim()
        {
            this.Trim(0);
        }

        /// <summary>
        /// Removes all ancestor nodes which are at the given depth below the current node
        /// </summary>
        /// <param name="depth">Depth</param>
        /// <exception cref="ArgumentException">Thrown if depth is less than zero</exception>
        public void Trim(int depth)
        {
            if (depth == 0)
            {
                lock (this._children)
                {
                    this._children.Clear();
                }
            }
            else if (depth > 0)
            {
                lock (this._children)
                {
                    foreach (TrieNode<TKeyBit, TValue> node in this._children.Values)
                    {
                        node.Trim(depth - 1);
                    }
                }
            }
            else
            {
                throw new ArgumentException("Depth must be >= 0");
            }
        }

        /// <summary>
        /// Add a child node associated with a key to this node and return the node.
        /// </summary>
        /// <param name="key">Key to associated with the child node.</param>
        /// <returns>If given key already exists then return the existing child node, else return the new child node.</returns>
        internal TrieNode<TKeyBit, TValue> AddChild(TKeyBit key)
        {
            TrieNode<TKeyBit, TValue> child;
            lock (this._children)
            {
                if (this._children.TryGetValue(key, out child))
                {
                    return child;
                }
                else
                {
                    child = new TrieNode<TKeyBit, TValue>(this, key);
                    this._children.Add(key, child);
                    return child;
                }
            }
        }

        /// <summary>
        /// Remove the child of a node associated with a key along with all its descendents.
        /// </summary>
        /// <param name="key">The key associated with the child to remove.</param>
        internal void RemoveChild(TKeyBit key)
        {
            lock (this._children)
            {
                this._children.Remove(key);
            }
        }

        /// <summary>
        /// Gets the immediate children of this Node
        /// </summary>
        public IEnumerable<TrieNode<TKeyBit, TValue>> Children
        {
            get
            {
                return this._children.Values.ToList();
            }
        }

        /// <summary>
        /// Gets all descended children of this Node
        /// </summary>
        public IEnumerable<TrieNode<TKeyBit, TValue>> AllChildren
        {
            get
            {
                if (this.IsLeaf)
                {
                    return Enumerable.Empty<TrieNode<TKeyBit, TValue>>();
                }
                else
                {
                    lock (this._children)
                    {
                        IEnumerable<TrieNode<TKeyBit, TValue>> cs = from n in this._children.Values
                                                                    from c in n.AllChildren
                                                                    select c;
                        cs = this._children.Values.Concat(this._children.Values);
                        return cs.ToList();
                    }
                }
            }
        }

        /// <summary>
        /// Get an enumerable of values contained in this node and all its descendants
        /// </summary>
        public virtual IEnumerable<TValue> Values
        {
            get
            {
                if (this.IsLeaf)
                {
                    if (this.HasValue)
                    {
                        return new TValue[] { this.Value };
                    }
                    else
                    {
                        return Enumerable.Empty<TValue>();
                    }
                }
                else
                {
                    lock (this._children)
                    {
                        IEnumerable<TValue> vs = from n in this._children.Values
                                                 from v in n.Values
                                                 select v;
                        if (this.Value != null && !this.IsRoot) vs = vs.Concat(new TValue[] { this.Value });
                        return vs.ToList();
                    }
                }
            }
        }
    }
}