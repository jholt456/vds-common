﻿using System;
using System.Collections.Generic;

namespace VDS.Common.Tries
{
    /// <summary>
    /// Interface for Trie nodes, this is the node in a <see cref="ITrie"/>
    /// </summary>
    /// <typeparam name="TKeyBit">Key Bit Type</typeparam>
    /// <typeparam name="TValue">Value Type</typeparam>
    public interface ITrieNode<TKeyBit, TValue>
        where TValue : class
    {
        /// <summary>
        /// Gets all descendants of this Node
        /// </summary>
        IEnumerable<ITrieNode<TKeyBit, TValue>> Descendants { get; }

        /// <summary>
        /// Gets the immediate children of this Node
        /// </summary>
        IEnumerable<ITrieNode<TKeyBit, TValue>> Children { get; }

        /// <summary>
        /// Clears the Value (if any) stored at this node and removes all child nodes
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears the Value (if any) stored at this node
        /// </summary>
        void ClearValue();

        /// <summary>
        /// Gets the number of immediate child nodes this node has
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the number of descendant nodes this node has
        /// </summary>
        int CountAll { get; }

        /// <summary>
        /// Check whether or not this node has a value associated with it
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Check whether or not this node has any children
        /// </summary>
        bool IsLeaf { get; }

        /// <summary>
        /// Check whether this Node is the Root Node
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// Gets the key bit that is associated with this node
        /// </summary>
        TKeyBit KeyBit { get; }

        /// <summary>
        /// Get the parent of this node
        /// </summary>
        ITrieNode<TKeyBit, TValue> Parent { get; }

        /// <summary>
        /// Add a child node associated with a key to this node and return the node.
        /// </summary>
        /// <param name="key">Key to associated with the child node.</param>
        /// <returns>If given key already exists then return the existing child node, else return the new child node.</returns>
        ITrieNode<TKeyBit, TValue> MoveToChild(TKeyBit key);

        /// <summary>
        /// Tries to get a child of this node which is associated with a key bit
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="child">Child</param>
        /// <returns>True if a child was found, false otherwise</returns>
        bool TryGetChild(TKeyBit key, out ITrieNode<TKeyBit, TValue> child);

        /// <summary>
        /// Remove the child of a node associated with a key along with all its descendents.
        /// </summary>
        /// <param name="key">The key associated with the child to remove.</param>
        void RemoveChild(TKeyBit key);

        /// <summary>
        /// Removes all child nodes
        /// </summary>
        void Trim();

        /// <summary>
        /// Removes all descendant nodes which are at the given depth below the current node
        /// </summary>
        /// <param name="depth">Depth</param>
        /// <exception cref="ArgumentException">Thrown if depth is less than zero</exception>
        void Trim(int depth);

        /// <summary>
        /// Gets/Sets value stored by this node
        /// </summary>
        TValue Value { get; set; }

        /// <summary>
        /// Get an enumerable of values contained in this node and all its descendants
        /// </summary>
        IEnumerable<TValue> Values { get; }
    }
}
