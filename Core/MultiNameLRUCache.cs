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

using System;
using System.Collections.Generic;
using System.Text;

namespace PMU.Core {
    class MultiNameLRUCache<K, V> {

        #region Fields

        Dictionary<K, ValueItem<K, V>> names = new Dictionary<K, ValueItem<K, V>>();
        LinkedList<ValueItem<K, V>> values = new LinkedList<ValueItem<K, V>>();
        int capacity;
        Object lockObject = new object();

        #endregion Fields

        #region Constructors

        public MultiNameLRUCache(int capacity) {
            this.capacity = capacity;
        }

        #endregion Constructors

        #region Methods

        public void Add(K key, V val) {
            lock (lockObject) {
                //remove excesses
                if (values.Count >= capacity) {
                    RemoveFirst();
                }
                //add to cache as normal
                ValueItem<K, V> cacheItem = new ValueItem<K, V>(val);
                cacheItem.keys.AddLast(key);
                values.AddLast(cacheItem);
                names.Add(key, cacheItem);
            }
        }

        public void AddAlias(K newKey, K oldKey) {
            lock (lockObject) {
                ValueItem<K, V> node;
                //make sure key exists
                if (names.TryGetValue(oldKey, out node)) {
                    node.keys.AddLast(newKey);
                    names.Add(newKey, node);
                }
            }
        }

        public bool ContainsKey(K key) {
            lock (lockObject) {
                return names.ContainsKey(key);
            }
        }

        public V Get(K key) {
            lock (lockObject) {
                ValueItem<K, V> node;
                if (names.TryGetValue(key, out node)) {
                    
                    V value = node.value;

                    values.Remove(node);
                    values.AddLast(node);
                    return value;
                }
                return default(V);
            }
        }

        protected void RemoveFirst() {
            // Remove from values
            LinkedListNode<ValueItem<K, V>> node = values.First;
            values.RemoveFirst();
            // Remove from keys
            foreach (K key in node.Value.keys) {
                names.Remove(key);
            }
        }

        #endregion Methods

    }

    
    internal class NameItem<K, V> {
        #region Fields

        public K key;
        public ValueItem<K, V> value;

        #endregion Fields

        #region Constructors

        public NameItem(K k, ValueItem<K, V> v) {
            key = k;
            value = v;
        }

        #endregion Constructors
    }

    internal class ValueItem<K, V> {
        #region Fields

        public V value;
        public LinkedList<K> keys;

        #endregion Fields

        #region Constructors

        public ValueItem(V v) {
            value = v;
            keys = new LinkedList<K>();
        }

        #endregion Constructors
    }
}
