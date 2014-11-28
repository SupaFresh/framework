// This file is part of Mystery Dungeon eXtended.

// Mystery Dungeon eXtended is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Mystery Dungeon eXtended is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with Mystery Dungeon eXtended.  If not, see <http://www.gnu.org/licenses/>.

// From: http://stackoverflow.com/questions/754233/is-it-there-any-lru-implementation-of-idictionary
namespace PMU.Core
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class LRUCache<K, V>
    {
        #region Fields

        Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> cacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
        int capacity;
        LinkedList<LRUCacheItem<K, V>> lruList = new LinkedList<LRUCacheItem<K, V>>();
        Object lockObject = new object();

        #endregion Fields

        #region Constructors

        public LRUCache(int capacity) {
            this.capacity = capacity;
        }

        #endregion Constructors

        #region Methods

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(K key, V val) {
            lock (lockObject) {
                if (cacheMap.Count >= capacity) {
                    RemoveFirst();
                }
                LRUCacheItem<K, V> cacheItem = new LRUCacheItem<K, V>(key, val);
                LinkedListNode<LRUCacheItem<K, V>> node = new LinkedListNode<LRUCacheItem<K, V>>(cacheItem);
                lruList.AddLast(node);
                cacheMap.Add(key, node);
            }
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public V Get(K key) {
            lock (lockObject) {
                LinkedListNode<LRUCacheItem<K, V>> node;
                if (cacheMap.TryGetValue(key, out node)) {
                    //System.Console.WriteLine("Cache HIT " + key);
                    V value = node.Value.value;

                    lruList.Remove(node);
                    lruList.AddLast(node);
                    return value;
                }
                //System.Console.WriteLine("Cache MISS " + key);
                return default(V);
            }
        }

        protected void RemoveFirst() {
            // Remove from LRUPriority
            LinkedListNode<LRUCacheItem<K, V>> node = lruList.First;
            lruList.RemoveFirst();
            // Remove from cache
            cacheMap.Remove(node.Value.key);
        }

        #endregion Methods
    }

    internal class LRUCacheItem<K, V>
    {
        #region Fields

        public K key;
        public V value;

        #endregion Fields

        #region Constructors

        public LRUCacheItem(K k, V v) {
            key = k;
            value = v;
        }

        #endregion Constructors
    }
}