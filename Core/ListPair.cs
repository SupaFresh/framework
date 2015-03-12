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

namespace PMDCP.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ListPair<TKey, TValue> : IEnumerable<TValue>
    {
        #region Fields

        List<TKey> keys;
        List<TValue> values;

        #endregion Fields

        #region Constructors

        public ListPair() {
            keys = new List<TKey>();
            values = new List<TValue>();
        }

        #endregion Constructors

        #region Properties

        public int Count {
            get { return keys.Count; }
        }

        public List<TKey> Keys {
            get { return keys; }
        }

        public List<TValue> Values {
            get { return values; }
        }

        #endregion Properties

        #region Indexers

        public TKey this[TValue val] {
            get {
                int index = values.IndexOf(val);
                return keys[index];
            }
            set {
                int index = values.IndexOf(val);
                keys[index] = value;
            }
        }

        public TValue this[TKey key] {
            get {
                int index = keys.IndexOf(key);
                return values[index];
            }
            set {
                int index = keys.IndexOf(key);
                values[index] = value;
            }
        }

        #endregion Indexers

        #region Methods

        public void Add(TKey key, TValue value) {
            if (key != null) {
                if (keys.Contains(key) == false) {
                    keys.Add(key);
                    values.Add(value);
                } else {
                    throw (new ArgumentException("A key with the same value has already been added"));
                }
            } else {
                throw (new ArgumentNullException("key"));
            }
        }

        public void Clear() {
            keys.Clear();
            values.Clear();
        }

        public bool ContainsKey(TKey key) {
            return keys.Contains(key);
        }

        public bool ContainsValue(TValue value) {
            return values.Contains(value);
        }

        public ListPair<TKey, TValue> Copy() {
            lock (this) {
                ListPair<TKey, TValue> copy = new ListPair<TKey, TValue>();
                for (int i = 0; i < keys.Count; i++) {
                    copy.Add(keys[i], values[i]);
                }
                return copy;
            }
        }

        public IEnumerator<TValue> GetEnumerator() {
            return values.GetEnumerator();
        }

        public TKey GetKey(TValue value) {
            int index = values.IndexOf(value);
            if (index > -1) {
                return keys[index];
            } else {
                return default(TKey);
            }
        }

        public TValue GetValue(TKey key) {
            int index = keys.IndexOf(key);
            if (index > -1) {
                return values[index];
            } else {
                return default(TValue);
            }
        }

        public int IndexOfKey(TKey key) {
            return keys.IndexOf(key);
        }

        public int IndexOfValue(TValue value) {
            return values.IndexOf(value);
        }

        public TKey KeyByIndex(int index) {
            return keys[index];
        }

        public void RemoveAt(int index) {
            keys.RemoveAt(index);
            values.RemoveAt(index);
        }

        public void RemoveAtKey(TKey key) {
            if (key != null) {
                if (keys.Contains(key)) {
                    int index = keys.IndexOf(key);
                    keys.RemoveAt(index);
                    values.RemoveAt(index);
                } else {
                    throw (new KeyNotFoundException());
                }
            } else {
                throw (new ArgumentNullException("key"));
            }
        }

        public void RemoveAtValue(TValue value) {
            if (value != null) {
                if (values.Contains(value)) {
                    int index = values.IndexOf(value);
                    keys.RemoveAt(index);
                    values.RemoveAt(index);
                } else {
                    throw (new KeyNotFoundException());
                }
            } else {
                throw (new ArgumentNullException("value"));
            }
        }

        public void SetKey(TValue value, TKey newKey) {
            int index = values.IndexOf(value);
            keys[index] = newKey;
        }

        public void SetValue(TKey key, TValue value) {
            int index = keys.IndexOf(key);
            values[index] = value;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return (IEnumerator<TValue>)values.GetEnumerator();
        }

        public TValue ValueByIndex(int index) {
            return values[index];
        }

        #endregion Methods
    }
}